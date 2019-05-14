using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    public class Constants
    {
        public const int Rows = 15;
        public const int Cols = 9;

        public const long SuperScore = 999999999999999999;

        public const long PathClosedScore = 0;

        public const long FiveConnectedScore = 18000000;

        public const long FourConnectedOneOpenScore = 500000;
        public const long FourConnectedBothOpenScore = 10000000;

        public const long ThreeConnectedOneOpenScore = 10000;
        public const long ThreeConnectedBothOpenScore = 500000;

        public const long TwoConnectedOneOpenScore = 100;
        public const long TwoConnectedBothOpenScore = 10000;

        public const long OneConnectedOneOpenScore = 1;
        public const long OneConnectedBothOpenScore = 100;

        public const long OffensiveFactor = 250000;
    }
}
