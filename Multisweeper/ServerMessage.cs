using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multisweeper
{
    class ServerMessage
    {
        public ServerMessageType messageType { get; }
        public ClientBoard clientBoard { get; }

        public ServerMessage(byte[] payload)
        {
            messageType = (ServerMessageType) Enum.ToObject(typeof(ServerMessageType), payload[0]);
            switch (messageType)
            {
                case ServerMessageType.BoardUpdate:
                    byte size = payload[1];
                    clientBoard = new ClientBoard(size);
                    clientBoard.Deserialize(payload.Skip(2).ToArray());
                    break;
            }
        }

        public ServerMessage(ClientBoard clientBoard)
        {
            messageType = ServerMessageType.BoardUpdate;
            this.clientBoard = clientBoard;
        }

        public byte[] Serialize()
        {
            switch (messageType)
            {
                case ServerMessageType.BoardUpdate:
                    byte[] serializedClientBoard = clientBoard.Serialize();
                    byte[] payload = new byte[serializedClientBoard.Length + 2];
                    payload[0] = (byte) messageType;
                    payload[1] = clientBoard.size;
                    Buffer.BlockCopy(serializedClientBoard, 0, payload, 2, serializedClientBoard.Length);
                    return payload;
                default:
                    return null;
            }
        }
    }
}
