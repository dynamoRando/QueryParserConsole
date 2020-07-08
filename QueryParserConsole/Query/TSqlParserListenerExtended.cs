using Antlr4.Runtime.Misc;
using QueryParserConsole.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

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
        public TSqlParserListenerExtended(SelectStatement statement)
        {
            _statement = statement;
        }
        #endregion

        #region Public Properties
        #endregion

        #region Public Methods
        public SelectStatement GetStatementAsSelect()
        {
            return _statement as SelectStatement;
        }
        public override void ExitSearch_condition([NotNull] TSqlParser.Search_conditionContext context)
        {
            base.ExitSearch_condition(context);
            // this will set the full statement on the final exit
            var select = GetStatementAsSelect();
            select.WhereClause = context.GetText();
        }
        public override void EnterSelect_statement([NotNull] TSqlParser.Select_statementContext context)
        {
            base.EnterSelect_statement(context);
            var select = GetStatementAsSelect();
            select.RawStatement = context.GetText();
        }

        public override void EnterSelect_list([NotNull] TSqlParser.Select_listContext context)
        {
            base.EnterSelect_list(context);
            var select = GetStatementAsSelect();
            select.SelectListRaw = context.GetText();
        }

        public override void EnterSelect_list_elem([NotNull] TSqlParser.Select_list_elemContext context)
        {
            base.EnterSelect_list_elem(context);
            var select = GetStatementAsSelect();
            select.SelectList.Add(context.GetText());
        }

        public override void EnterSearch_condition([NotNull] TSqlParser.Search_conditionContext context)
        {
            base.EnterSearch_condition(context);
        }

        public override void EnterPredicate([NotNull] TSqlParser.PredicateContext context)
        {
            base.EnterPredicate(context);
            Console.WriteLine(context.GetText());
            var select = GetStatementAsSelect();
            var part = new StatementPart();
            part.Text = context.GetText();
            part.StatementOrigin = "EnterPredicate";

            var parent = context.Parent.Parent;
            if (parent != null)
            {
                part.StatementParent = parent.GetText();
            }

            var grandparent = context.Parent.Parent.Parent;
            if (grandparent != null)
            {
                part.StatementGrandParent = grandparent.GetText();
            }

            select.Statements.Add(part);
        }
        #endregion

        #region Private Properties
        
        #endregion
    }
}
