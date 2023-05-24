using System;

public class Board{

    private byte[][] board;
    private FieldState[][] fieldstates;
    Random random = new Random();
    



    public Board(byte size){
        
        board = new board[size][size];
        

    }

    public byte checksurrounding(byte x, byte y){
        byte counter = 0;
        for(int i = Math.Max(x-1, 0); i < Math.Min(x+1, board.GetLength(0)-1); i++){
            for (int a = Math.Max(y-1, 0); a < Math.Min(y+1, board.GetLength(0)-1); a++){
                if (board[i][a] == -1){
                    counter++;
                }

            }
                
        }

    }

    public void initialize(byte x, byte y){
        
        for (int i = 0; i < size; i++){
            for (int a = 0; a < size; a++){
                if (x > i-2 && x < i+2 && y > a-2 && y < a+2){  //Feld um den ersten Klick muss null sein
                    continue;
                }
                else{
                    int nr = random.Next(1, 7);
                    if(nr == 1){
                        board[i][a] = -1;
                    }
                }

            }


        }
        for (int i = 0; i < size; i++){
            for (int a = 0; a < size; a++){
                if (board[i][a] == -1){

                }
                else{
                    board[i][a] = checksurrounding(i, a);
                    }

            }


        }


    }

    public void checksurroundingForZero(byte x, byte y){
            
            for(int i = x-1; i < x+1; i++){
                for (int a = y-1; a < y+1; a++){
                    if (board[i][a] == 0){
                        checksurroundingForZero(i, a);
                        return revealsurrounding(i, a);
                    }

                }
                    
            }

        }


        

}