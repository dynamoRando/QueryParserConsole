using Antlr4.Runtime.Misc;
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
        public override void EnterSelect_statement([NotNull] TSqlParser.Select_statementContext context)
        {
            base.EnterSelect_statement(context);
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Properties
        #endregion
    }
}
