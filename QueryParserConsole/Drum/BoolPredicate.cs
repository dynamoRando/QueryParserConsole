using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParserConsole.Drum
{
    class BoolPredicate : IPredicate
    {
        private int _id;

        public Interval Interval => GetInterval();
        public IPredicate Left { get; set; }
        public IPredicate Right { get; set; }
        public int Id => _id;   

        public BoolPredicate(int id)
        {
            _id = id;
        }

        private Interval GetInterval()
        {
            var interval = new Interval { A = 0, B = 0 };

            if (Left is not null)
            {
                interval.A = Left.Interval.A;
            }

            if (Right is not null)
            {
                interval.B = Right.Interval.B;
            }

            return interval;
        }

    }
}
