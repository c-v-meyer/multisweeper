using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Multisweeper
{
    class Client
    {
        private const int port = 43612;
        private TcpClient tcpClient;
        private NetworkStream nwStream;
        private Thread listeningThread;
        private SemaphoreSlim mainToThreadSem, threadToMainSem;

        private ClientBoard board;
        private Action listenerCallback;

        public Client(string ip, ClientBoard board, Action listenerCallback)
        {
            this.board = board;
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
                mainToThreadSem.Wait();
                threadToMainSem.Wait();
                byte[] payload = new byte[tcpClient.ReceiveBufferSize];
                int bytesRead = nwStream.Read(payload, 0, tcpClient.ReceiveBufferSize);
                ServerMessage serverMessage = new ServerMessage(payload);
                System.Diagnostics.Trace.WriteLine("Received a message!!1!!11!111!!111!!!!!!!!");
                switch (serverMessage.messageType)
                {
                    case ServerMessageType.BoardUpdate:
                        board = serverMessage.clientBoard;
                        break;
                }
                listenerCallback();
                threadToMainSem.Release();
            }
            finally
            {
                tcpClient.Close();
            }
        }

        public void Send(ClientMessageType messageType, byte x, byte y)
        {
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
