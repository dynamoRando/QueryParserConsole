using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParserConsole.Query
{
    public class SelectStatement : IStatement
    {
        #region Public Properties
        public List<StatementPart> Statements { get; set; }
        #endregion

        #region Constructors
        public SelectStatement()
        {
            Statements = new List<StatementPart>();
        }
        #endregion
            
        #region Public Methods
        #endregion

        #region Private Properties
        #endregion
    }
}
