using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParserConsole.Query
{
    public class JoinStep : IPlanStep
    {
        #region Public Properties
        public Guid Id { get; set; }
        public int Level { get; set; }
        #endregion

        #region Constructors
        #endregion

        #region Public Methods
        public PlanResult GetResult()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
