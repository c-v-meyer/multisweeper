using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multisweeper
{
    class ClientMessage
    {
        public ClientMessageType messageType { get; init; }
        public byte x { get; init; }
        public byte y { get; init; }

        public ClientMessage(byte[] payload)
        {
            messageType = (ClientMessageType) Enum.ToObject(typeof(ClientMessageType), payload[0]);
            x = payload[1];
            y = payload[2];
        }

        public byte[] Serialize()
        {
            return new byte[] { (byte) messageType, x, y };
        }
    }
}
