using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneChessEngine
{
    public class ChessEngine
    {
        private CheckerBoard _checkerBoard = new CheckerBoard();

        private Stack<Position> _humanHistory;
        private Stack<Position> _computerHistory;

        private PointState _currentPlayer;

        public string Debuginformation { get; private set; }

        public void Initialize(PointState role)
        {
            _checkerBoard.Initialize();
            _humanHistory = new Stack<Position>();
            _computerHistory = new Stack<Position>();
            _currentPlayer = role;
        }

        public Position Place()
        {
            Position bestPosition = GetBestNextPosition();

            Place(bestPosition);

            return bestPosition;
        }

        public void Place(Position position)
        {
            _computerHistory.Push(position);

            _checkerBoard[position] = _currentPlayer;
        }

        public void Take(Position position)
        {
            _humanHistory.Push(position);
            _checkerBoard[position] = _currentPlayer == PointState.White ? PointState.Black : PointState.White;
        }

        public Position StepbackH()
        {
            return Stepback(_humanHistory);
        }

        public Position StepbackC()
        {
            return Stepback(_computerHistory);
        }

        public Position LastH
        {
            get { return _humanHistory.Count > 0 ? _humanHistory.Peek() : null; }
        }

        public Position LastC
        {
            get { return _computerHistory.Count > 0 ? _computerHistory.Peek() : null; }
        }

        public PointState this[Position position]
        {
            get
            {
                return _checkerBoard[position];
            }

            set
            {
                _checkerBoard[position] = value;
            }
        }

        public bool IsGameOver(Position position)
        {
            List<Path> paths = Utilities.GetLongestPaths(position, _checkerBoard);

            return paths.Any(path => path.Count >= 5);
        }

        public int StepCount()
        {
            return new List<int> { _humanHistory.Count, _computerHistory.Count }.Max();
        }

        private Position Stepback(Stack<Position> history)
        {
            Position position = null;

            if (history.Count > 0)
            {
                position = history.Pop();

                _checkerBoard[position] = PointState.None;
            }

            return position;
        }

        private Position GetBestNextPosition()
        {
            Position bestNextPosition = null;

            if (_computerHistory.Count > 0)
            {
                var computerCandidates = Utilities.GetAllCandidadteWithScores(_computerHistory.ToList(), _checkerBoard);

                var superCandidate = computerCandidates.Where(candidate => candidate.Score >= Constants.SuperScore).FirstOrDefault();
                if (superCandidate != null)
                {
                    return superCandidate.Position;
                }
                else
                {
                    var humanCandidates = Utilities.GetAllCandidadteWithScores(_humanHistory.ToList(), _checkerBoard);

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

                    UpdateDebugInformation(mergedCandidates.ToDictionary(candidate => candidate.Key), bestNextPosition);
                }
            }
            else
            {
                bestNextPosition = GetFirstComputerChessPosition();
            }

            return bestNextPosition;
        }

        private void UpdateDebugInformation(Dictionary<string, CandidatePosition> candidates, Position bestPosition)
        {
            StringBuilder debugInformation = new StringBuilder();

            for (int i = 0; i < Constants.Rows; i++)
            {
                StringBuilder line = new StringBuilder();

                for (int j = 0; j < Constants.Cols; j++)
                {
                    string state = string.Empty;

                    Position position = new Position(i, j);

                    if (_checkerBoard[position] == PointState.White)
                    {
                        state = "W";
                    }
                    else if (_checkerBoard[position] == PointState.Black)
                    {
                        state = "B";
                    }
                    else if (bestPosition.Equals(position))
                    {
                        state = string.Format("{0}_{1}_Best", candidates[position.Key].Score, candidates[position.Key].Strategy);
                    }
                    else if (candidates.ContainsKey(position.Key))
                    {
                        state = string.Format("{0}_{1}", candidates[position.Key].Score, candidates[position.Key].Strategy);
                    }

                    line.Append(string.Format("({0}, {1}, {2})\t", i, j, state));
                }

                debugInformation.AppendLine(line.ToString());
            }

            Debuginformation = debugInformation.ToString();
        }

        private Position GetFirstComputerChessPosition()
        {
            if (_humanHistory.Count == 0)
            {
                return new Position(Constants.Rows / 2, Constants.Cols / 2);
            }
            else
            {
                return Utilities.GetSecondPosition(_humanHistory.Last());
            }
        }
    }
}
