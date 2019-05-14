using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveStar.src
{
    internal enum PlayerRole
    {
        White = 0,
        Black = 1
    }

    internal enum PointState
    {
        None = 0,
        Border = 1,
        Black = 2,
        White = 3
    }

    internal class Postion
    {
        public int X = -1;
        public int Y = -1;
        public Postion(int x, int y)
        {
            X = x;
            Y = y; 
        }
    }

    internal class PPoint
    {
        public int x;
        public int y;
        public int pri;
    };

    internal enum PlayingState
    { 
        Ready = 0,
        SelectingCandiate = 1,
        ConfirmChoice = 2,
        OverWon = 3,
        OverLost = 4
    }
}
