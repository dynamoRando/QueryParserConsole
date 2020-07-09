using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParserConsole.Query
{
    public class PlanResult : IPlanResult
    {
        #region Public Properties
        public List<Row> Rows { get; set; }
        #endregion

        #region Constructors
        public PlanResult()
        {
            Rows = new List<Row>();
        }

        public PlanResult(List<Row> rows)
        {
            Rows = rows;
        }
        #endregion
    }
}
