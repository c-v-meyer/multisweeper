using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multisweeper
{
    class ClientBoard
    {
        private byte size;
        public FieldState[,] board { get; set; }

        public ClientBoard(byte size)
        {
            this.size = size;
        }

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            for (byte y = 0; y < size; y++)
                for (byte x = 0; x < size; x++)
                    sb.Append((byte)board[y, x]);
            return sb.ToString();
        }

        public void Deserialize(string str)
        {
            for (int i = 0; i < str.Length / 2; i += 2)
                board[i / size, i % size] = (FieldState) int.Parse(str.Substring(i, 1));
        }
    }
}
