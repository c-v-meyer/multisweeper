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
        private const int port = 43612;
        private IPEndPoint ipEndPoint;
        private TcpListener tcpListener;
        private TcpClient tcpClientA, tcpClientB, nextTcpClient;
        private NetworkStream nwStreamA, nwStreamB, nextNwStream;
        private Thread serverThread;

        private ServerBoard board;

        public Server(byte size)
        {
            board = new ServerBoard(size);
        }

        public string Serve()
        {
            ipEndPoint = new IPEndPoint(IPAddress.Any, port);
            tcpListener = new TcpListener(ipEndPoint);
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string ipAddress;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("1.1.1.1", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
            }
            if (serverThread == null)
                serverThread = new Thread(new ThreadStart(ServerThreadTask));
            return ipAddress;
        }

        private void ServerThreadTask()
        {
            try
            {
                tcpListener.Start();
                tcpClientA = tcpListener.AcceptTcpClient();
                nwStreamA = tcpClientA.GetStream();
                tcpClientB = tcpListener.AcceptTcpClient();
                nwStreamB = tcpClientB.GetStream();
                nextTcpClient = tcpClientA;
                nextNwStream = nwStreamA;

                while (true)
                {
                    byte[] buffer = new byte[nextTcpClient.ReceiveBufferSize];

                    int bytesRead = nextNwStream.Read(buffer, 0, nextTcpClient.ReceiveBufferSize);
                    if (ProcessMessage(buffer))
                    {
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
                tcpClientA.Close();
                tcpClientB.Close();
                tcpListener.Stop();
            }
        }

        // Returns: switch player
        private bool ProcessMessage(byte[] buffer)
        {
            ClientMessage clientMessage = new ClientMessage(buffer);
            if (clientMessage.messageType == ClientMessageType.Reveal)
            {
                if (board.isInitialized)
                    board.reveal(clientMessage.x, clientMessage.y);
                else
                    board.initialize(clientMessage.x, clientMessage.y);
                ServerMessage serverMessage = new ServerMessage(board.clientBoard);
                byte[] response = serverMessage.Serialize();
                nextNwStream.Write(response, 0, response.Length);
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
            if (serverThread != null)
                serverThread.Interrupt();
        }
    }
}