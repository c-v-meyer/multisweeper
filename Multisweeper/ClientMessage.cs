using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multisweeper
{
    // Nachricht vom Client an den Server
    class ClientMessage
    {
        public ClientMessageType messageType { get; init; } // Art der Nachricht
        // Betroffene Koordinaten
        public byte x { get; init; }
        public byte y { get; init; }

        public ClientMessage() { }

        // ClientMessage aus rohem Datenstrom zusammenschustern
        public ClientMessage(byte[] payload)
        {
            messageType = (ClientMessageType) Enum.ToObject(typeof(ClientMessageType), payload[0]);
            x = payload[1];
            y = payload[2];
        }

        // ClientMessage in rohen Datenstrom aufdröseln
        public byte[] Serialize()
        {
            return new byte[] { (byte) messageType, x, y };
        }
    }
}
