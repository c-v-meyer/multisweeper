using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multisweeper
{
    class ClientBoard
    {
        public byte size { get; }
        public FieldState[,] board { get; set; }

        public ClientBoard(byte size)
        {
            this.size = size;
            board = new FieldState[size, size];
        }

        public byte[] Serialize()
        {
            byte[] payload = new byte[board.Length * board.Length];
            Buffer.BlockCopy(board, 0, payload, 0, payload.Length);
            return payload;
        }

        public void Deserialize(byte[] payload)
        {
            Buffer.BlockCopy(payload, 0, board, 0, payload.Length);
        }
    }
}
