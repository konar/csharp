using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneChessEngine
{
    public class CandidatePosition
    {
        public long Score { get; set; }
        public Position Position { get; set; }

        public Strategy Strategy { get; set; }

        public string Key
        { get { return Position.Key; } }

        public Dictionary<string, CandidateVoteFactor> VoteFactors { get; set; }

    }
}
