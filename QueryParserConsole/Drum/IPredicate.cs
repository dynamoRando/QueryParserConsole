using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParserConsole.Drum
{
    interface IPredicate
    {
        Interval Interval { get; }
        public int Id { get; }
    }
}
