using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneChessEngine
{
    public class CheckerBoard
    {
        private PointState[][] _checkerBoardState = new PointState[Constants.Rows][];

        public void Initialize()
        {
            for (int i = 0; i < Constants.Rows; i++)
            {
                if (_checkerBoardState[i] == null)
                {
                    _checkerBoardState[i] = new PointState[Constants.Cols];
                }

                for (int j = 0; j < Constants.Cols; j++)
                {
                    _checkerBoardState[i][j] = PointState.None;
                }
            }
        }

        public PointState this[Position position]
        {
            get
            {
                return _checkerBoardState[position.X][position.Y];
            }

            set
            {
                _checkerBoardState[position.X][position.Y] = value;
            }
        }


    }
}
