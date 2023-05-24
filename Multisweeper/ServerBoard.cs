using System;

namespace Multisweeper
{
    public class ServerBoard
    {
        private int size;
        private byte[,] board; // 255 = Bombe; 0-8 = Bomben au�enrum
        private ClientBoard clientBoard; // Clientrepr�sentation des Boards (beinhaltet Flaggen)
        private Random random = new Random();

        public ServerBoard(byte size)
        {
            board = new byte[size, size];
            initialize();
        }

        public byte checksurrounding(byte x, byte y)
        {
            byte counter = 0;
            for (byte i = (byte)Math.Max(y - 1, 0); i < Math.Min(y + 1, board.GetLength(0) - 1); i++)
            {
                for (byte a = (byte)Math.Max(x - 1, 0); a < Math.Min(x + 1, board.GetLength(0) - 1); a++)
                {
                    if (board[i, a] == 255)
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }

        public void initialize()
        {
            for (byte i = 0; i < size; i++)
            {
                for (byte a = 0; a < size; a++)
                {
                    if (!(x > i - 2 && x < i + 2 && y > a - 2 && y < a + 2))
                    {  //Feld um den ersten Klick muss null sein
                        int nr = random.Next(1, 7);
                        if (nr == 1)
                        {
                            board[i, a] = 255;
                        }
                    }
                }
            }
            for (byte i = 0; i < size; i++)
            {
                for (byte a = 0; a < size; a++)
                {
                    if (!(board[i, a] == 255))
                    {
                        board[i, a] = checksurrounding(i, a);
                    }
                }
            }
        }

        public void reveal(int x, int y)
        {
            for (byte i = (byte)Math.Max(y - 1, 0); i < Math.Min(y + 1, size - 1); i++)
            {
                for (byte a = (byte)Math.Max(x - 1, 0); a < Math.Min(x + 1, size - 1); a++)
                {
                    clientBoard.SetFieldState(i, a, (FieldState)board[i, a]);
                    if (board[i, a] == 0)
                    {
                        reveal(i, a);
                    }
                }
            }
        }
    }
}