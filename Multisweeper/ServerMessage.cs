using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multisweeper
{
    // Nachricht vom Server an den Client
    class ServerMessage
    {
        public ServerMessageType messageType { get; } // Art der Nachricht
        public bool ownTurn; // Ist der empfangende Client am Zug?
        public ClientBoard clientBoard { get; } // Aktuelles Spielfeld

        // ServerMessage aus rohem Datenstrom zusammenschustern
        public ServerMessage(byte[] payload)
        {
            messageType = (ServerMessageType) Enum.ToObject(typeof(ServerMessageType), payload[0]);
            ownTurn = payload[1] == 1;
            switch (messageType)
            {
                case ServerMessageType.BoardUpdate:
                    byte size = payload[2];
                    clientBoard = new ClientBoard(size);
                    clientBoard.Deserialize(payload.Skip(3).ToArray());
                    break;
            }
        }

        // "Normaler" Kosntruktor
        public ServerMessage(ClientBoard clientBoard, bool ownTurn)
        {
            messageType = ServerMessageType.BoardUpdate;
            this.clientBoard = clientBoard;
            this.ownTurn = ownTurn;
        }

        // ServerMessage in rohen Datenstrom aufdröseln
        public byte[] Serialize()
        {
            switch (messageType)
            {
                case ServerMessageType.BoardUpdate:
                    byte[] serializedClientBoard = clientBoard.Serialize();
                    byte[] payload = new byte[serializedClientBoard.Length + 3];
                    payload[0] = (byte) messageType;
                    payload[1] = (byte) (ownTurn ? 1 : 0);
                    payload[2] = clientBoard.size;
                    Buffer.BlockCopy(serializedClientBoard, 0, payload, 3, serializedClientBoard.Length);
                    return payload;
                default:
                    return null;
            }
        }
    }
}
