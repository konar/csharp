using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    public class Path
    {
        public ChessDirection Direction { get; set; }
        public Position BeginPosition { get; set; }
        public Position EndPosition { get; set; }
        public int Count { get; set; }

        public string Key
        {
            get { return BeginPosition == null ? string.Empty : string.Format("{0}_{1}_{2}", Direction, BeginPosition.X, BeginPosition.Y); }
        }

        public ExtendLevel ExtendLevel { get; set; }

    }
}
