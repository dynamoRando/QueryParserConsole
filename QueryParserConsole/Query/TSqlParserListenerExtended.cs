using QueryParserConsole.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParserConsole
{
    public class TSqlParserListenerExtended : TSqlParserBaseListener
    {
        #region Private Fields
        IStatement _statement;
        #endregion

        #region Constructors
        public TSqlParserListenerExtended() { }
        public TSqlParserListenerExtended(IStatement statement)
        {
            _statement = statement;
        }
        #endregion

        #region Public Properties
        #endregion

        #region Public Methods
        #endregion

        #region Private Properties
        #endregion
    }
}
