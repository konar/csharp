using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    public enum PlayerRole
    {
        White = 0,
        Black = 1
    }

    public enum PointState
    {
        None = 0,
        Border = 1,
        Black = 2,
        White = 3
    }

    public class Position
    {
        public int X = -1;
        public int Y = -1;
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public string Key
        {
            get { return string.Format("{0}_{1}", X, Y); }
        }
    }

    public class PPoint
    {
        public int x;
        public int y;
        public int pri;
    };

    public enum PlayingState
    {
        Ready = 0,
        SelectingCandiate = 1,
        ConfirmChoice = 2,
        OverWon = 3,
        OverLost = 4
    }

    public enum ChessDirection
    {
        Horizontal = 0,
        Vertical = 1,
        BackSlash = 2,
        Slash = 3
    }
}
