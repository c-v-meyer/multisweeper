using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Multisweeper {
    class Server {
        public delegate void ConnectCallback();

        private const int port = 43612;
        private IPEndPoint ipEndPoint;
        private TcpListener tcpListener;
        private TcpClient tcpClientA, tcpClientB, nextTcpClient;
        private NetworkStream nwStreamA, nwStreamB, nextNwStream;
        private Thread serverThread;

        private ServerBoard board;

        private ConnectCallback connectCallback; // Wird nach dem Verbinden aufgerufen

        public Server(byte size, ConnectCallback connectCallback)
        {
            board = new ServerBoard(size); // Neues ServerBoard erstellen
            this.connectCallback = connectCallback;
        }

        public string Serve()
        {
            // Allen IP-Adressen horchen
            ipEndPoint = new IPEndPoint(IPAddress.Any, port);
            tcpListener = new TcpListener(ipEndPoint);
            // Eigene IP-Adresse bestimmen
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string ipAddress;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("1.1.1.1", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
            }
            // Nebenläufigen Server-Thread starten
            if (serverThread == null)
            {
                serverThread = new Thread(new ThreadStart(ServerThreadTask));
                serverThread.Start();
            }
            return ipAddress;
        }

        private void ServerThreadTask()
        {
            try
            {
                // Verbindungsanfragen beider Spieler akzeptieren
                tcpListener.Start();
                connectCallback();
                tcpClientA = tcpListener.AcceptTcpClient();
                nwStreamA = tcpClientA.GetStream();
                tcpClientB = tcpListener.AcceptTcpClient();
                nwStreamB = tcpClientB.GetStream();
                nextTcpClient = tcpClientB; // Spieler, dem als nächstes "zugehört" wird
                nextNwStream = nwStreamB; // s.o.

                while (true)
                {
                    byte[] buffer = new byte[nextTcpClient.ReceiveBufferSize];

                    int bytesRead = nextNwStream.Read(buffer, 0, nextTcpClient.ReceiveBufferSize);
                    if (ProcessMessage(buffer)) // Wenn der Spieler gewechselt werden soll...
                    {
                        // Spieler wechseln
                        if (nextTcpClient == tcpClientA)
                        {
                            nextTcpClient = tcpClientB;
                            nextNwStream = nwStreamB;
                        }
                        else
                        {
                            nextTcpClient = tcpClientA;
                            nextNwStream = nwStreamA;
                        }
                    }
                }
            }
            finally
            {
                // Verbindungen schließen
                tcpClientA.Close();
                tcpClientB.Close();
                tcpListener.Stop();
            }
        }

        // Returns: switch player
        private bool ProcessMessage(byte[] buffer)
        {
            // ClientMessage aus rohem Datenstrom zusammenbauen
            ClientMessage clientMessage = new ClientMessage(buffer);
            if (clientMessage.messageType == ClientMessageType.Reveal)
            {
                // Spielfeld entweder initialisieren oder ein Feld aufdecken
                if (board.isInitialized)
                    board.reveal(clientMessage.x, clientMessage.y);
                else
                    board.initialize(clientMessage.x, clientMessage.y);
                // Neue ServerMessage als Antwort an den Client zusammenbauen
                // nextNwStream != nwStreamA: Nach Aufdecken ist der gleiche Spieler nicht mehr am Zug, daher ungleich
                ServerMessage serverMessage = new ServerMessage(board.clientBoard, nextNwStream != nwStreamA);
                // Nachricht in rohen Datenstrom umwandeln und dann Abfahrt
                byte[] response = serverMessage.Serialize();
                nwStreamA.Write(response, 0, response.Length);
                // s.o.
                serverMessage = new ServerMessage(board.clientBoard, nextNwStream != nwStreamB);
                response = serverMessage.Serialize();
                nwStreamB.Write(response, 0, response.Length);
                return true;
            }
            else
            {
                //TODO
                return false;
            }
        }

        public void Stop()
        {
            // Server-Thread stoppen
            if (serverThread != null)
                serverThread.Interrupt();
        }
    }
}