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
            for (byte y = 0; y < size; y++)
            {
                for (byte x = 0; x < size; x++)
                {
                    board[y, x] = FieldState.Unrevealed;
                }
            }
        }

        public byte[] Serialize()
        {
            byte[] payload = new byte[board.Length * board.Length];
            for (byte y = 0; y < size; y++)
                for (byte x = 0; x < size; x++)
                    payload[y * size + x] = (byte) board[y, x];
            return payload;
        }

        public void Deserialize(byte[] payload)
        {
            for (byte y = 0; y < size; y++)
                for (byte x = 0; x < size; x++)
                    board[y, x] = (FieldState)Enum.ToObject(typeof(FieldState), payload[y * size + x]);
        }
    }
}
