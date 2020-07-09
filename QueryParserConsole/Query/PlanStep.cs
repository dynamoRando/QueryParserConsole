using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParserConsole.Query
{
    public class PlanStep : IPlanStep
    {
        public Guid Id { get; set; }
        public int Level { get; set; }

        public PlanResult GetResult()
        {
            throw new NotImplementedException();
        }
    }
}
