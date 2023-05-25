using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Multisweeper
{
    class Server
    {
        Socket serverSocket;

        public Server() {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 43612));
            serverSocket.Listen(128);
            serverSocket.BeginAccept(null, 0, OnAccept, null);
        }
    }
}