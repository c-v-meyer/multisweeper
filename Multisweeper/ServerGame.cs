using System;

namespace Multisweeper
{
    public class ServerGame{

        private int pointsA;
        private int pointsB;


        public ServerGame(){

        }

        public void addpointsA(){
            pointsA++;
        }

        public void addpointsB(){
            pointsB++;
        }


        public void subpointsA(){
            pointsA - 2;
        }
        public void subpointsB(){
            pointsB - 2;
        }

        public int getpointsA(){
            return pointsA;

        }

        public int getpointsB(){
            return pointsB;

        }

}}