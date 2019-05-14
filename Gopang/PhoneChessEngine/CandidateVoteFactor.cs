using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneChessEngine
{
    public class CandidateVoteFactor
    {
        public Path Path { get; set; }
        public bool IsIsolated { get; set; }
        public ExtendLevel CandidateExtendLevel{ get; set; }
        public ExtendLevel PathExtendLevel { get; set; }
        public string Key
        {
            get { return Path.Key; }
        }
    }
}
