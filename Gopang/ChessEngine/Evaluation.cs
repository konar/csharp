using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    public class Evaluation
    {
        public static long EvaluatePositionScore(CandidatePosition candidate, CheckerBoard checkerBoard, PointState targetState)
        {
            long score = 0;

            Dictionary<string, CandidatePosition> nextStepCandidateScores = new Dictionary<string, CandidatePosition>();

            // Set first level candidate
            checkerBoard[candidate.Position] = targetState;

            List<Path> paths = Utilities.GetLongestPaths(candidate.Position, checkerBoard);
            paths.ForEach(path => 
            {
                score += EvaluateFirstLevelPathScore(path, checkerBoard, nextStepCandidateScores);
            });

            checkerBoard[candidate.Position] = PointState.None;

            return score;
        }

        private static long EvaluateFirstLevelPathScore(Path path, CheckerBoard checkerBoard, Dictionary<string, CandidatePosition> nextStepCandidateScores)
        {
            long score = 0;

            if (path.Count >= 5)
            {
                return Constants.SuperScore;
            }
            else 
            {
                if (path.ExtendLevel == ExtendLevel.None)
                {
                    return Constants.PathClosedScore;
                }

                Dictionary<string, CandidatePosition> nextStepCandidatePositions = new Dictionary<string, CandidatePosition>();

                Utilities.GetCandidatePositionByPath(path, checkerBoard, nextStepCandidatePositions, false);

                foreach (var candidatePosition in nextStepCandidatePositions.Values)
                {
                    // Try to fill the Candidate to see what's the impact to checker board.
                    checkerBoard[candidatePosition.Position] = checkerBoard[path.BeginPosition];

                    Path ExtendedPath = Utilities.GetLongestPathByDirection(candidatePosition.Position, path.Direction, checkerBoard);

                    long candidateScore = EvaluateSecondLevelPathScore(ExtendedPath, checkerBoard);

                    if (nextStepCandidateScores.ContainsKey(candidatePosition.Key))
                    {
                        if (nextStepCandidateScores[candidatePosition.Key].Score < candidateScore)
                        {
                            score += candidateScore - nextStepCandidateScores[candidatePosition.Key].Score;

                            nextStepCandidateScores[candidatePosition.Key].Score = candidateScore;
                        }
                    }
                    else
                    {
                        nextStepCandidatePositions.Add(candidatePosition.Key, new CandidatePosition {
                            Position = candidatePosition.Position,
                            Score = candidateScore
                        });

                        score += candidateScore;
                    }

                    // Revert the change
                    checkerBoard[candidatePosition.Position] = PointState.None;
                }
            }

            return score;
        }

        private static long EvaluateSecondLevelPathScore(Path path, CheckerBoard checkerBoard)
        {
            long score = 0;

            if (path.Count >= 5)
            {
                return Constants.FiveConnectedScore;
            }
            else
            {
                if (path.ExtendLevel == ExtendLevel.None)
                {
                    return Constants.PathClosedScore;
                }

                switch (path.Count)
                {
                    case 4:

                        return path.ExtendLevel == ExtendLevel.Better ? Constants.FourConnectedBothOpenScore : Constants.FourConnectedOneOpenScore;

                    case 3:

                        return path.ExtendLevel == ExtendLevel.Better ? Constants.ThreeConnectedBothOpenScore : Constants.ThreeConnectedOneOpenScore;

                    case 2:

                        return path.ExtendLevel == ExtendLevel.Better ? Constants.TwoConnectedBothOpenScore : Constants.TwoConnectedOneOpenScore;

                    case 1:

                        return path.ExtendLevel == ExtendLevel.Better ? Constants.OneConnectedBothOpenScore : Constants.OneConnectedBothOpenScore;

                    default:
                        break;
                }
            }

            return score;
        }
    }
}
