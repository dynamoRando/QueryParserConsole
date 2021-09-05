using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParserConsole.Drum
{
    class PredicateBucket
    {
        public List<IPredicate> Predicates { get; set; }

        public PredicateBucket() 
        {
            Predicates = new List<IPredicate>();
        }
    }
}
