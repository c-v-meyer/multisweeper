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
        private TcpClient tcpClient;
        private NetworkStream nwStream;
        private Thread listeningThread;
        private SemaphoreSlim mainToThreadSem, threadToMainSem;

        public ClientBoard board { get; set; }
        private bool ownTurn;
        private ListenerCallback listenerCallback;
        private Dispatcher callbackDispatcher;

        public Client(string ip, bool ownTurn, byte size, ListenerCallback listenerCallback, Dispatcher callbackDispatcher)
        {
            this.board = new ClientBoard(size);
            this.ownTurn = ownTurn;
            this.listenerCallback = listenerCallback;
            this.callbackDispatcher = callbackDispatcher;
            tcpClient = new TcpClient(ip, port);
            nwStream = tcpClient.GetStream();
            mainToThreadSem = new SemaphoreSlim(0);
            threadToMainSem = new SemaphoreSlim(1);
            listeningThread = new Thread(new ThreadStart(ListeningThreadTask));
            listeningThread.Start();
        }

        private void ListeningThreadTask()
        {
            try
            {
                while (true)
                {
                    mainToThreadSem.Wait();
                    threadToMainSem.Wait();
                    byte[] payload = new byte[tcpClient.ReceiveBufferSize];
                    int bytesRead = nwStream.Read(payload, 0, tcpClient.ReceiveBufferSize);
                    ServerMessage serverMessage = new ServerMessage(payload);
                    ownTurn = serverMessage.ownTurn;
                    System.Diagnostics.Trace.WriteLine("Received a message!!1!!11!111!!111!!!!!!!!");
                    switch (serverMessage.messageType)
                    {
                        case ServerMessageType.BoardUpdate:
                            board = serverMessage.clientBoard;
                            break;
                    }
                    callbackDispatcher.Invoke(listenerCallback, board);
                    threadToMainSem.Release();
                }
            }
            finally
            {
                tcpClient.Close();
            }
        }


        public void Send(ClientMessageType messageType, byte x, byte y)
        {
            if (!ownTurn)
                return;
            ClientMessage message = new ClientMessage() { messageType = messageType, x = x, y = y };
            threadToMainSem.Wait();
            byte[] payload = message.Serialize();
            nwStream.Write(payload, 0, payload.Length);
            threadToMainSem.Release();
            mainToThreadSem.Release();
        }

        public void Stop()
        {
            if (listeningThread != null)
                listeningThread.Interrupt();
        }
    }
}
