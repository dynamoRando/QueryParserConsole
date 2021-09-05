using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParserConsole.Drum
{
    class Predicate : IPredicate
    {
        private Interval _interval;
        private int _id;

        public Interval Interval => _interval;
        public int Id => _id;
        public string Text { get; set; }

        public Predicate(int id)
        {
            _id = id;
            _interval = new Interval { A = 0, B = 0 };
        }

        public void SetInterval(Interval interval)
        {
            _interval = interval;
        }

        public void SetA(int a)
        {
            _interval.A = a;
        }

        public void SetB(int b)
        { 
            _interval.B = b;
        }
    }
}
