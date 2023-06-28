﻿using System;

namespace Multisweeper
{
    class ServerBoard
    {
        private byte size;
        private byte[,] board; // 255 = Bombe; 0-8 = Bomben außenrum
        public ClientBoard clientBoard { get; private set; } // Clientrepräsentation des Boards (beinhaltet Flaggen)
        private Random random = new Random();
        public bool isInitialized { get; private set; }
        private ServerGame servergame;

        public ServerBoard(byte size)
        {
            this.size = size;
            board = new byte[size, size];
            clientBoard = new ClientBoard(size);
            isInitialized = false;
            servergame = new ServerGame();
        }

        private void UpdateClientBoard()
        {
            for (byte y = 0; y < size; y++)
            {
                for (byte x = 0; x < size; x++)
                {
                    if (board[y, x] <= 8)
                    {
                        clientBoard.board[y, x] = (FieldState) Enum.ToObject(typeof(FieldState), board[y, x]);
                    }
                }
            }
        }

        public byte checksurrounding(byte x, byte y)
        {
            byte counter = 0;
            for (int i = Math.Max(y - 1, 0); i <= Math.Min(y + 1, size - 1); i++)
            {
                for (int a = Math.Max(x - 1, 0); a <= Math.Min(x + 1, size - 1); a++)
                {
                    if (board[i, a] == 255)
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }

        public void initialize(byte x, byte y)
        {
            isInitialized = true;
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
                    if (board[i, a] != 255)
                    {
                        board[i, a] = checksurrounding(i, a);
                    }
                }
            }
            UpdateClientBoard();
        }

        public void reveal(int x, int y)
        {
            for (int i = Math.Max(y - 1, 0); i <= Math.Min(y + 1, size - 1); i++)
            {
                for (int a = Math.Max(x - 1, 0); a <= Math.Min(x + 1, size - 1); a++)
                {
                    if (board[i, a] == 255)
                        endGame();
                    if (board[i, a] != 255) // Wenn keine Bombe...
                        clientBoard.board[i, a] = (FieldState) Enum.ToObject(typeof(FieldState), board[i, a]); // Aufdecken und mit Zahl versehen
                    if (board[i, a] == 0 && clientBoard.board[i, a] >= FieldState.Unrevealed) // Feld ist null und nicht aufgedeckt
                                                                                              // (>= FieldState.Unrevealed hei�t Unrevealed oder FlaggedA oder FlaggedB)
                        reveal(i, a); // REKURSION!!!
                }
            }
            UpdateClientBoard();
        }



        public void flag(int x, int y, Player player)
        {

            if (board[x, y] == 255)
            {
                if (player == Player.PlayerA)
                {
                    servergame.addpointsA();
                    clientBoard.board[y, x] = FieldState.FlaggedA;
                }
                if (player == Player.PlayerB)
                {
                    servergame.addpointsB();
                    clientBoard.board[y, x] = FieldState.FlaggedB;
                }

            }
            else
            { //Wenn keine Bombe, -2 Punkte
                if (player == Player.PlayerA)
                    servergame.subpointsA();

                if (player == Player.PlayerB)
                    servergame.subpointsB();


            }



        }

        public void checkWinner()
        {
            if (servergame.getpointsA() == servergame.getpointsB())
                Console.WriteLine("Unentschieden!");
            if (servergame.getpointsA() > servergame.getpointsB())
                Console.WriteLine("Player A wins!");
            else
                Console.WriteLine("Player B wins!");

        }

        public void revealAllBombs() //Falls gameover, sollen alle Bomben aufgedeckt werden
        {
            for (byte i = 0; i < size; i++)
            {
                for (byte a = 0; a < size; a++)
                {
                    if (board[i, a] == 255)
                    {
                        clientBoard.board[i, a] = FieldState.RevealedBomb;
                    }
                }
            }

        }


        public void endGame()
        {

            //soll das spiel abbrechen

        }
    }
}