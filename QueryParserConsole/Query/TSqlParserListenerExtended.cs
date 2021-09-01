using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using ConsoleAntlrSQL;
using QueryParserConsole.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace QueryParserConsole
{
    public class TSqlParserListenerExtended : TSqlParserBaseListener
    {
        #region Private Fields
        IStatement _statement;
        ICharStream _charStream;
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
        public CommonTokenStream TokenStream { get; set; }
        #endregion

        #region Public Methods
        public override void ExitFull_table_name([NotNull] TSqlParser.Full_table_nameContext context)
        {
            base.ExitFull_table_name(context);
            string debug = context.GetText();
            Debug.WriteLine(debug);
        }
        public override void EnterTable_name([NotNull] TSqlParser.Table_nameContext context)
        {
            Console.WriteLine(context.GetText());
            base.EnterTable_name(context);

            string debug = context.GetText();
            Debug.WriteLine(debug);

            var select = GetStatementAsSelect();
            select.Tables.Add(context.GetText());
        }

        public override void ExitTable_name([NotNull] TSqlParser.Table_nameContext context)
        {
            base.ExitTable_name(context);

            string debug = context.GetText();
            Debug.WriteLine(debug);
        }

        public override void ExitTable_name_with_hint([NotNull] TSqlParser.Table_name_with_hintContext context)
        {
            base.ExitTable_name_with_hint(context);

            string debug = context.GetText();
            Debug.WriteLine(debug);
        }

        public override void EnterTable_name_with_hint([NotNull] TSqlParser.Table_name_with_hintContext context)
        {
            base.EnterTable_name_with_hint(context);

            string debug = context.GetText();
            Debug.WriteLine(debug);
        }

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

            int a = context.Start.StartIndex;
            int b = context.Stop.StopIndex;
            Interval interval = new Interval(a, b);
            _charStream = context.Start.InputStream;
            select.WhereClauseWithWhiteSpace = _charStream.GetText(interval);
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

            string debug = context.GetText();

            Console.WriteLine("EnterSearch_condition:");
            Console.WriteLine(debug);
            Console.WriteLine("---");
        }


        public override void EnterPredicate([NotNull] TSqlParser.PredicateContext context)
        {
            base.EnterPredicate(context);

            string debug = context.GetText();

            Console.WriteLine("EnterPredicate:");
            Console.WriteLine(debug);
            Console.WriteLine("---");


            var select = GetStatementAsSelect();
            var part = new StatementPart();
            part.StatementTableName = select.Tables.FirstOrDefault();
            part.Text = context.GetText();
            part.StatementOrigin = "EnterPredicate";

            int a = context.Start.StartIndex;
            int b = context.Stop.StopIndex;
            Interval interval = new Interval(a, b);
            _charStream = context.Start.InputStream;
            
            part.TextWithWhiteSpace = _charStream.GetText(interval);

            var parent = context.Parent.Parent;
            if (parent != null)
            {
                part.StatementParent = parent.GetText();
                var tokenInterval = parent.SourceInterval;
                part.StatementParentWithWhiteSpace = GetWhitespaceStringFromTokenInterval(tokenInterval);
            }

            var grandparent = context.Parent.Parent.Parent;
            if (grandparent != null)
            {
                part.StatementGrandParent = grandparent.GetText();
                var tokenInterval = grandparent.SourceInterval;
                part.StatementGrandParentWithWhiteSpace = GetWhitespaceStringFromTokenInterval(tokenInterval);
            }

            part.ParseStatementPart();
            select.Statements.Add(part);
        }
 
        // begin insert functions
        public override void EnterInsert_statement([NotNull] TSqlParser.Insert_statementContext context)
        {
            string debug = context.GetText();

            Console.WriteLine("EnterInsert_statement:");
            Console.WriteLine(debug);
            Console.WriteLine("---");
        }

        public override void EnterFull_table_name(TSqlParser.Full_table_nameContext context)
        {
            string debug = context.GetText();

            Console.WriteLine("EnterFull_table_name:");
            Console.WriteLine(debug);
            Console.WriteLine("---");
        }

        public override void EnterColumn_name_list(TSqlParser.Column_name_listContext context)
        {
            string debug = context.GetText();

            Console.WriteLine("EnterColumn_name_list:");
            Console.WriteLine(debug);
            Console.WriteLine("---");
        }
        
        public override void EnterSimple_id(TSqlParser.Simple_idContext context)
        {
            string debug = context.GetText();

            Console.WriteLine("EnterSimple_id:");
            Console.WriteLine(debug);
            Console.WriteLine("---");
        }

        public override void EnterInsert_statement_value(TSqlParser.Insert_statement_valueContext context)
        {
            string debug = context.GetText();

            Console.WriteLine("EnterInsert_statement_value:");
            Console.WriteLine(debug);
            Console.WriteLine("---");
        }

        public override void EnterExpression_list(TSqlParser.Expression_listContext context)
        {
            string debug = context.GetText();

            Console.WriteLine("EnterExpression_list:");
            Console.WriteLine(debug);
            Console.WriteLine("---");

        }

        public override void EnterPrimitive_expression(TSqlParser.Primitive_expressionContext context)
        {
            string debug = context.GetText();

            Console.WriteLine("EnterPrimitive_expression:");
            Console.WriteLine(debug);
            Console.WriteLine("---");

        }

        // end insert functions

        // begin update functions
        public override void EnterUpdate_statement(TSqlParser.Update_statementContext context)
        {
            string debug = context.GetText();

            Console.WriteLine("EnterUpdate_statement:");
            Console.WriteLine(debug);
            Console.WriteLine("---");
        }

        public override void EnterUpdate_elem(TSqlParser.Update_elemContext context)
        {
            string debug = context.GetText();

            Console.WriteLine("EnterUpdate_elem:");
            Console.WriteLine(debug);
            Console.WriteLine("---");
        }

        // end update functions

        // begin delete functions
        public override void EnterDelete_statement(TSqlParser.Delete_statementContext context)
        {
            Console.WriteLine(context.GetText());
        }

        public override void EnterDelete_statement_from(TSqlParser.Delete_statement_fromContext context)
        {
            Console.WriteLine(context.GetText());
        }
        // end delete functions

        // create table functions
        public override void EnterCreate_table([NotNull] TSqlParser.Create_tableContext context)
        {
            base.EnterCreate_table(context);


            string debug = context.GetText();

            Console.WriteLine("EnterCreate_table:");
            Console.WriteLine(debug);
            Console.WriteLine("---");

        }

        public override void EnterDml_clause([NotNull] TSqlParser.Dml_clauseContext context)
        {
            base.EnterDml_clause(context);
        }

        public override void EnterDdl_clause([NotNull] TSqlParser.Ddl_clauseContext context)
        {
            base.EnterDdl_clause(context);
        }

        public override void EnterData_type([NotNull] TSqlParser.Data_typeContext context)
        {
            base.EnterData_type(context);

            string debug = context.GetText();

            Console.WriteLine("EnterData_type:");
            Console.WriteLine(debug);
            Console.WriteLine("---");

        }

        public override void EnterColumn_definition([NotNull] TSqlParser.Column_definitionContext context)
        {
            base.EnterColumn_definition(context);
            string debug = context.GetText();

            Console.WriteLine("EnterColumn_definition:");
            Console.WriteLine(debug);
            Console.WriteLine("---");
        }

        public override void EnterNull_notnull([NotNull] TSqlParser.Null_notnullContext context)
        {
            base.EnterNull_notnull(context);

            string debug = context.GetText();

            Console.WriteLine("EnterNull_notnull:");
            Console.WriteLine(debug);
            Console.WriteLine("---");

        }

        public override void EnterNull_or_default([NotNull] TSqlParser.Null_or_defaultContext context)
        {
            base.EnterNull_or_default(context);

            string debug = context.GetText();

            Console.WriteLine("EnterNull_or_default:");
            Console.WriteLine(debug);
            Console.WriteLine("---");

        }

        // end create table functions

        // begin create database functions
        public override void EnterCreate_database([NotNull] TSqlParser.Create_databaseContext context)
        {
            base.EnterCreate_database(context);

            string debug = context.GetText();

            Console.WriteLine("EnterCreate_database:");
            Console.WriteLine(debug);
            Console.WriteLine("---");


        }

        // end create database functions
        #endregion

        #region Private Properties
        private string GetWhitespaceStringFromTokenInterval(Interval interval)
        {
            try
            {
                var start = TokenStream.Get(interval.a).StartIndex;
                var end = TokenStream.Get(interval.b).StopIndex;
                Interval i = new Interval(start, end);
                return _charStream.GetText(i);
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
