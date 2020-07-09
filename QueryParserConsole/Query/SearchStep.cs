using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParserConsole.Query
{
    public class SearchStep : IPlanStep
    {
        #region Private Fields
        StatementPart _part;
        #endregion

        #region Public Properties
        public Guid Id { get; set; }
        public int Level { get; set; }
        #endregion

        #region Constructors
        public SearchStep() { }
        public SearchStep(StatementPart part)
        {
            _part = part;
        }
        #endregion

        #region Public Methods
        public PlanResult GetResult()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
