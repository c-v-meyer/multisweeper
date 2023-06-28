using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace Multisweeper
{
    class Client
    {
        public delegate void ListenerCallback(ref ClientBoard board);

        private const int port = 43612;
        private TcpClient tcpClient; // Verbindung zum Server
        private NetworkStream nwStream;
        private Thread listeningThread; // Thread, der stets horcht
        private SemaphoreSlim mainToThreadSem, threadToMainSem; // Synchronisation beider Threads

        public ClientBoard board { get; set; }
        private bool ownTurn; // Bin ich am Zug?
        private ListenerCallback listenerCallback; // Wird aufgerufen, nachdem eine neue Nachricht eingeht
        private Dispatcher callbackDispatcher; // Wird benötigt, um listenerCallback im MainThread aufzurufen

        public Client(string ip, bool ownTurn, byte size, ListenerCallback listenerCallback, Dispatcher callbackDispatcher)
        {
            this.board = new ClientBoard(size);
            this.ownTurn = ownTurn;
            this.listenerCallback = listenerCallback;
            this.callbackDispatcher = callbackDispatcher;
            // Verbindung zum Server aufbauen
            tcpClient = new TcpClient(ip, port);
            nwStream = tcpClient.GetStream();
            mainToThreadSem = new SemaphoreSlim(0);
            threadToMainSem = new SemaphoreSlim(1);
            // Thread starten
            listeningThread = new Thread(new ThreadStart(ListeningThreadTask));
            listeningThread.Start();
        }

        private void ListeningThreadTask()
        {
            try
            {
                while (true)
                {
                    // Warte, bis gehört werden soll
                    mainToThreadSem.Wait();
                    threadToMainSem.Wait(); // Ressourcen reservieren
                    byte[] payload = new byte[tcpClient.ReceiveBufferSize];
                    int bytesRead = nwStream.Read(payload, 0, tcpClient.ReceiveBufferSize);
                    // Rohen Datenstrom in ServerMessage umwandeln
                    ServerMessage serverMessage = new ServerMessage(payload);
                    ownTurn = serverMessage.ownTurn;
                    System.Diagnostics.Trace.WriteLine("Received a message!!1!!11!111!!111!!!!!!!!");
                    switch (serverMessage.messageType)
                    {
                        case ServerMessageType.BoardUpdate:
                            board = serverMessage.clientBoard;
                            break;
                    }
                    callbackDispatcher.Invoke(listenerCallback, board); // Callback aufrufen
                    threadToMainSem.Release(); // Ressourcen freigeben
                }
            }
            finally
            {
                tcpClient.Close(); // Verbindung schließen
            }
        }


        // Nachricht an den Server senden
        public void Send(ClientMessageType messageType, byte x, byte y)
        {
            // Wenn ich nicht am Zug bin, nicht senden
            if (!ownTurn)
                return;
            // ClientMessage konstruieren
            ClientMessage message = new ClientMessage() { messageType = messageType, x = x, y = y };
            threadToMainSem.Wait(); // Ressourcen reservieren
            // ClientMessage in rohen Datenstrom umwandeln
            byte[] payload = message.Serialize();
            nwStream.Write(payload, 0, payload.Length);
            threadToMainSem.Release(); // Ressourcen reservieren
            mainToThreadSem.Release(); // Anweisung zum Zuhören geben
        }

        public void Stop()
        {
            // Thread stoppen
            if (listeningThread != null)
                listeningThread.Interrupt();
        }
    }
}
