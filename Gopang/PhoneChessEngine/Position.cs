using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneChessEngine
{
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

        public bool Equals(Position p)
        {
            return p != null && X == p.X && Y == p.Y;
        }
    }
}
