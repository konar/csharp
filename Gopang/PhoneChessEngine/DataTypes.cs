using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneChessEngine
{
    public enum PointState
    {
        None = 0,
        Border = 1,
        Black = 2,
        White = 3
    }

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
