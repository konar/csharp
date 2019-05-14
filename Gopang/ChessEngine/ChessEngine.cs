using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    class ChessEngine
    {
        private CheckerBoard _checkBoard = new CheckerBoard();

        private Stack<Position> _humanHistory;
        private Stack<Position> _computerHistory;

        private PlayerRole _currentPlayer;

        public void Initialize(PlayerRole role)
        {
            _checkBoard.Initialize();
            _humanHistory = new Stack<Position>();
            _computerHistory = new Stack<Position>();
            _currentPlayer = role;
        }

        public Position Place()
        {
            Position bestPosition = GetBestNextPosition();

            _computerHistory.Push(bestPosition);

            _checkBoard[bestPosition] = _currentPlayer == PlayerRole.White ? PointState.White : PointState.Black;

            return bestPosition;
        }

        public void Take(Position position)
        {
            _humanHistory.Push(position);

            _checkBoard[position] = _currentPlayer == PlayerRole.White ? PointState.Black : PointState.White;
        }

        private Position GetBestNextPosition()
        {
            Position bestNextPosition = null;

            if (_computerHistory.Count > 0)
            {
                var computerCandidates = Utilities.GetAllCandidadteWithScores(_computerHistory.ToList(), _checkBoard);

                var superCandidate = computerCandidates.Where(candidate => candidate.Score >= Constants.SuperScore).FirstOrDefault();
                if (superCandidate != null)
                {
                    return superCandidate.Position;
                }
                else
                {
                    var humanCandidates = Utilities.GetAllCandidadteWithScores(_humanHistory.ToList(), _checkBoard);
                    var mergedCandidates = Utilities.MergeCandidates(computerCandidates, humanCandidates);

                    long maxScore = mergedCandidates.Max(candidate => candidate.Score);

                    var bestCandidates = mergedCandidates.Where(candidate => candidate.Score == maxScore).ToList();

                    if (bestCandidates.Count > 1)
                    {
                        Random random = new Random(DateTime.Now.Second);
                        bestNextPosition = bestCandidates[random.Next(0, bestCandidates.Count)].Position;
                    }
                    else
                    {
                        bestNextPosition = bestCandidates.First().Position;
                    }
                }
            }
            else
            {
                bestNextPosition = GetFirstComputerChessPosition();
            }

            return bestNextPosition;
        }

        private Position GetFirstComputerChessPosition()
        {
            if (_humanHistory.Count == 0)
            {
                return new Position(Constants.Cols / 2, Constants.Rows / 2);
            }
            else
            {
                return Utilities.GetSecondPosition(_humanHistory.Last());
            }
        }
    }
}
