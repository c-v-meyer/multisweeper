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
        private Mutex mainToThreadMutex, threadToMainMutex;

        private ClientBoard board;

        public Client(string ip, ClientBoard board)
        {
            this.board = board;
            tcpClient = new TcpClient(ip, port);
            nwStream = tcpClient.GetStream();
            mainToThreadMutex = new Mutex();
            threadToMainMutex = new Mutex();
            mainToThreadMutex.WaitOne();
            listeningThread = new Thread(new ThreadStart(ListeningThreadTask));
            listeningThread.Start();
        }

        private void ListeningThreadTask()
        {
            try
            {
                mainToThreadMutex.WaitOne();
                threadToMainMutex.WaitOne();
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
                threadToMainMutex.ReleaseMutex();
            }
            finally
            {
                tcpClient.Close();
            }
        }

        public void Send(ClientMessageType messageType, byte x, byte y)
        {
            ClientMessage message = new ClientMessage() { messageType = messageType, x = x, y = y };
            threadToMainMutex.WaitOne();
            byte[] payload = message.Serialize();
            nwStream.Write(payload, 0, payload.Length);
            threadToMainMutex.ReleaseMutex();
            mainToThreadMutex.ReleaseMutex();
        }

        public void Stop()
        {
            if (listeningThread != null)
                listeningThread.Interrupt();
        }
    }
}
