using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneChessEngine
{
    public static class Utilities
    {


        public static List<CandidatePosition> GetAllCandidadteWithScores(List<Position> historyPositions, CheckerBoard checkerBoard)
        {
            List<CandidatePosition> candidates = GetAllCandidatePositions(historyPositions.ToList(), checkerBoard);

            foreach (var candidate in candidates)
            {
                candidate.Score = Evaluation.EvaluatePositionScore(candidate, checkerBoard, checkerBoard[historyPositions.First()]);
            }

            return candidates;
        }

        public static List<CandidatePosition> MergeCandidates(List<CandidatePosition> computerCandidates, List<CandidatePosition> humanCandidates)
        {
            computerCandidates.ForEach(candidate => candidate.Strategy = Strategy.Offense);

            Dictionary<string, CandidatePosition> candidates = computerCandidates.ToDictionary(candidate => candidate.Key);

            humanCandidates.ForEach(candidate =>
            {
                if (candidates.ContainsKey(candidate.Key))
                {
                    candidates[candidate.Key].Strategy = Strategy.Mixed;
                    candidates[candidate.Key].Score += candidate.Score;
                }
                else
                {
                    candidate.Strategy = Strategy.Defence;
                    candidate.Score -= Constants.OffensiveFactor;
                    candidates.Add(candidate.Key, candidate);
                }
            });

            return candidates.Values.ToList();
        }

        public static List<CandidatePosition> GetAllCandidatePositions(List<Position> historyPositions, CheckerBoard checkerBoard)
        {
            Dictionary<string, CandidatePosition> candidatePositions = new Dictionary<string, CandidatePosition>();
            List<Path> paths = GetAllPaths(historyPositions, checkerBoard);

            foreach (Path path in paths)
            {
                GetCandidatePositionByPath(path, checkerBoard, candidatePositions);
            }

            return candidatePositions.Values.ToList();
        }

        public static List<Path> GetAllPaths(List<Position> historyPositions, CheckerBoard checkerBoard)
        {
            Dictionary<string, Path> paths = new Dictionary<string, Path>();
            historyPositions.ForEach(position => 
            {
                List<Path> subPaths = GetLongestPaths(position, checkerBoard);

                subPaths.ForEach(path =>
                {
                    if (!paths.ContainsKey(path.Key))
                    {
                        paths.Add(path.Key, path);
                    }
                });

            });

            return paths.Values.ToList();
        }

        public static void GetCandidatePositionByPath(Path path, CheckerBoard checkerBoard, Dictionary<string, CandidatePosition> candidatePositions, bool needCrossLook = true)
        {
            Position upPosition = GetUpPosition(path.BeginPosition, path.Direction);
            if (TryAddCandidatePosition(upPosition, path, false, true, checkerBoard, candidatePositions))
            {
                if (needCrossLook)
                {
                    // Also consider second level.
                    Position upperPosition = GetUpPosition(upPosition, path.Direction);
                    TryAddCandidatePosition(upperPosition, path, true, true, checkerBoard, candidatePositions);
                }
            }

            Position nextPosition = GetNextPosition(path.EndPosition, path.Direction);
            if (TryAddCandidatePosition(nextPosition, path, false, false, checkerBoard, candidatePositions))
            {
                if (needCrossLook)
                {
                    // Also consider second level.
                    Position nexterPosition = GetNextPosition(nextPosition, path.Direction);
                    TryAddCandidatePosition(nexterPosition, path, true, false, checkerBoard, candidatePositions);
                }
            }

        }

        public static bool TryAddCandidatePosition(Position position, Path path, bool isIsolated, bool fromUp, CheckerBoard checkerBoard, Dictionary<string, CandidatePosition> candidatePositions)
        {
            if (position != null && checkerBoard[position] == PointState.None)
            {
                CandidatePosition candidatePosition = new CandidatePosition
                {
                    Position = position,
                    VoteFactors = new Dictionary<string, CandidateVoteFactor>()
                };

                if (!candidatePositions.ContainsKey(candidatePosition.Key))
                {
                    candidatePositions.Add(candidatePosition.Key, candidatePosition);
                }

                candidatePosition = candidatePositions[candidatePosition.Key];

                CandidateVoteFactor factor = new CandidateVoteFactor
                {
                    Path = path,
                    IsIsolated = isIsolated,
                    CandidateExtendLevel = GetExtendLevelForPosition(position, path.Direction, fromUp, checkerBoard[path.BeginPosition], checkerBoard),
                    PathExtendLevel = GetExtendLevelForPosition(fromUp ? path.EndPosition : path.BeginPosition, path.Direction, !fromUp, checkerBoard[path.BeginPosition], checkerBoard)
                };

                if (!candidatePosition.VoteFactors.ContainsKey(factor.Key))
                {
                    candidatePosition.VoteFactors.Add(factor.Key, factor);
                }

                return true;
            }

            return false;
        }

        public static ExtendLevel GetExtendLevelForPosition(Position position, ChessDirection direction, bool fromUp, PointState targetState, CheckerBoard checkerBoard)
        {
            Position extendedPosition = fromUp ? GetUpPosition(position, direction) : GetNextPosition(position, direction);

            if (extendedPosition != null)
            {
                if (checkerBoard[extendedPosition] == PointState.None)
                {
                    return ExtendLevel.Good;
                }
                else if (checkerBoard[extendedPosition] == targetState)
                {
                    return ExtendLevel.Better;
                }
            }

            return ExtendLevel.None;
        }

        public static ExtendLevel GetExtendLevelForPath(Path path, CheckerBoard checkerBoard)
        {
            Position upPosition = GetUpPosition(path.BeginPosition, path.Direction);
            Position nextPosition = GetNextPosition(path.EndPosition, path.Direction);

            bool isUpPositionAvaliable = upPosition != null && checkerBoard[upPosition] == PointState.None;
            bool isNextPositionAvaliable = nextPosition != null && checkerBoard[nextPosition] == PointState.None;

            if (isUpPositionAvaliable || isNextPositionAvaliable)
            {
                if (isUpPositionAvaliable && isNextPositionAvaliable)
                {
                    return ExtendLevel.Better;
                }
                else
                {
                    return ExtendLevel.Good;
                }
            }

            return ExtendLevel.None;
        }

        public static bool IsGameOver(Position position, CheckerBoard checkerBoard)
        {
            List<Path> paths = GetLongestPaths(position, checkerBoard);

            return paths.Any(path => path.Count >= 5);
        }

        public static List<Path> GetLongestPaths(Position position, CheckerBoard checkerBoard)
        {
            List<Path> result = new List<Path>();

            List<ChessDirection> directions = new List<ChessDirection> {ChessDirection.Horizontal, ChessDirection.Vertical, ChessDirection.BackSlash, ChessDirection.Slash};

            directions.ForEach(direction => {
                Path path = GetLongestPathByDirection(position, direction, checkerBoard);         
                result.Add(path);
            });

            return result;
        }

        public static Path GetLongestPathByDirection(Position position, ChessDirection direction, CheckerBoard checkerBoard)
        {
            Path path = null;
            int count = 0;
            Position beginPosition = null;
            Position endPositon = null;

            for (Position upPosition = position;
                upPosition != null && checkerBoard[upPosition] == checkerBoard[position];
                upPosition = GetUpPosition(upPosition, direction))
            {
                beginPosition = upPosition;
            }

            for (Position nextPosition = beginPosition;
                nextPosition != null && checkerBoard[nextPosition] == checkerBoard[position];
                nextPosition = GetNextPosition(nextPosition, direction))
            {
                count++;
                endPositon = nextPosition;
            }

            path = new Path
            {
                BeginPosition = beginPosition,
                EndPosition = endPositon,
                Direction = direction,
                Count = count
            };

            path.ExtendLevel = GetExtendLevelForPath(path, checkerBoard);

            return path;
        }

        public static Position GetNextPosition(Position position, ChessDirection direction)
        {
            Position nextPosition = null;

            switch (direction)
            {
                case ChessDirection.Horizontal:
                    if (position.X < Constants.Rows - 1)
                    {
                        nextPosition = new Position(position.X + 1, position.Y);
                    }

                    break;

                case ChessDirection.Vertical:
                    if (position.Y < Constants.Cols - 1)
                    {
                        nextPosition = new Position(position.X, position.Y + 1);
                    }

                    break;

                case ChessDirection.BackSlash:
                    if (position.X < Constants.Rows - 1 && position.Y < Constants.Cols - 1)
                    {
                        nextPosition = new Position(position.X + 1, position.Y + 1);
                    }

                    break;

                case ChessDirection.Slash:
                    if (position.X > 0 && position.Y < Constants.Cols - 1)
                    {
                        nextPosition = new Position(position.X - 1, position.Y + 1);
                    }
                    break;

                default:
                    throw new NotImplementedException("Unknown direction");
            }

            return nextPosition;
        }

        public static Position GetUpPosition(Position position, ChessDirection direction)
        {
            Position nextPosition = null;

            switch (direction)
            {
                case ChessDirection.Horizontal:
                    if (position.X > 0)
                    {
                        nextPosition = new Position(position.X - 1, position.Y);
                    }

                    break;

                case ChessDirection.Vertical:
                    if (position.Y > 0)
                    {
                        nextPosition = new Position(position.X, position.Y - 1);
                    }

                    break;

                case ChessDirection.BackSlash:
                    if (position.X > 0 && position.Y > 0)
                    {
                        nextPosition = new Position(position.X - 1, position.Y - 1);
                    }

                    break;

                case ChessDirection.Slash:
                    if (position.X < Constants.Rows - 1 && position.Y > 0)
                    {
                        nextPosition = new Position(position.X + 1, position.Y - 1);
                    }
                    break;

                default:
                    throw new NotImplementedException("Unknown direction");
            }

            return nextPosition;
        }

        public static Position GetSecondPosition(Position lastHumanPosition)
        {
            List<Position> candidatePosition = new List<Position>();

            int x = lastHumanPosition.X;
            int y = lastHumanPosition.Y;

            if (y < Constants.Cols / 2)
            {
                candidatePosition.Add(new Position(x, y + 2));
            }
            else if (y > Constants.Cols / 2)
            {
                candidatePosition.Add(new Position(x, y - 2));
            }
            else
            {
                candidatePosition.Add(new Position(x, y + 2));
                candidatePosition.Add(new Position(x, y - 2));
            }

            if (x < Constants.Rows / 2)
            {
                candidatePosition.Add(new Position(x + 2, y));

                if (y < Constants.Cols / 2)
                {
                    candidatePosition.Add(new Position(x + 1, y + 1));
                }
                else if (y > Constants.Cols / 2)
                {
                    candidatePosition.Add(new Position(x + 1, y - 1));
                }
                else
                {
                    candidatePosition.Add(new Position(x + 1, y + 1));
                    candidatePosition.Add(new Position(x + 1, y - 1));
                }
            }
            else if (x > Constants.Rows / 2)
            {
                candidatePosition.Add(new Position(x - 2, y));

                if (y < Constants.Cols / 2)
                {
                    candidatePosition.Add(new Position(x - 1, y + 1));
                }
                else if (y > Constants.Cols / 2)
                {
                    candidatePosition.Add(new Position(x - 1, y - 1));
                }
                else
                {
                    candidatePosition.Add(new Position(x - 1, y + 1));
                    candidatePosition.Add(new Position(x - 1, y - 1));
                }
            }
            else
            {
                candidatePosition.Add(new Position(x + 2, y));
                candidatePosition.Add(new Position(x - 2, y));

                if (y < Constants.Cols / 2)
                {
                    candidatePosition.Add(new Position(x - 1, y + 1));
                    candidatePosition.Add(new Position(x + 1, y + 1));
                }
                else if (y > Constants.Cols / 2)
                {
                    candidatePosition.Add(new Position(x - 1, y - 1));
                    candidatePosition.Add(new Position(x + 1, y - 1));
                }
                else
                {
                    candidatePosition.Add(new Position(x + 1, y + 1));
                    candidatePosition.Add(new Position(x + 1, y - 1));
                    candidatePosition.Add(new Position(x - 1, y + 1));
                    candidatePosition.Add(new Position(x - 1, y - 1));
                }
            }

            Random random = new Random(DateTime.Now.Second);

            return candidatePosition[random.Next(0, candidatePosition.Count)];
        }
    }
}
