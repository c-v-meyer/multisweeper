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
            {
                for (byte x = 0; x < size; x++)
                {
                    sb.Append((byte)board[y, x]);
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        public void Deserialize(string str)
        {
            string[] fields = str.Split(',');
            for (int i = 0; i < field.Length; i++) {
                board[i/size, i%size] = (FieldType)Byte.Parse(fields[i]);
            }
        }
    }
}
