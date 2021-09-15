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

            bool hasAnd = false;
            bool hasOr = false;

            string debug = context.GetText();
            var interval = context.SourceInterval;

            Console.WriteLine("EnterSearch_condition:");
            Console.WriteLine($"Text: {debug}");
            Console.WriteLine($"Interval: {interval.ToString()}");

            // if the predicate is not null
            // we can try to examine the parent's children for an AND/OR keyword
            var predicate = context.predicate();

            if (predicate != null)
            {
                var predicateText = predicate.GetText();
                var andNode = predicate.AND;
                var predicateExpression = predicate.expression();

                Console.WriteLine($"EnterSearch_condition - predicate: {predicateText}");
                var predicateInterval = predicate.SourceInterval;
                Console.WriteLine($"EnterSearch_condition - predicate interval: {predicateInterval}");

                if (context.Parent.ChildCount > 0)
                {
                    var contextParent = context.Parent as TSqlParser.Search_conditionContext;
                    foreach (var a in contextParent.children)
                    {
                        // one of these will be the current predicate
                        // the other will be the boolean operator
                        // and the final will be the other operator you need to combine
                        var aText = a.GetText();

                        Console.WriteLine($"EnterSearch_condition - predicate parent: {aText}");
                        var childInterval = a.SourceInterval;
                        Console.WriteLine($"EnterSearch_condition - predicate parent interval: {childInterval}");

                        if (aText == "AND" || aText == "OR")
                        {
                            Debug.Write("Have a boolean");
                        }
                    }

                    if (context.Parent.Parent is TSqlParser.Search_conditionContext)
                    {
                        var contextGrandParent = context.Parent.Parent as TSqlParser.Search_conditionContext;
                        foreach (var b in contextGrandParent.children)
                        {
                            var bText = b.GetText();
                            Console.WriteLine($"EnterSearch_condition - predicate grand parent: {bText}");

                            if (b is TSqlParser.Search_conditionContext)
                            {
                                var contextGreatGrandParent = b as TSqlParser.Search_conditionContext;
                                var contextGreatGrandParentPredicate = contextGreatGrandParent.predicate();

                                if (contextGreatGrandParentPredicate != null)
                                {
                                    var greatGrandParentText = contextGreatGrandParentPredicate.GetText();
                                    Console.WriteLine($"EnterSearch_condition - predicate great grandparent predicate: {greatGrandParentText}");
                                    var greatGrandParentInterval = contextGreatGrandParentPredicate.SourceInterval;
                                    Console.WriteLine($"EnterSearch_condition - predicate great grandparent predicate interval: {greatGrandParentInterval}");

                                }

                                if (contextGreatGrandParent.ChildCount > 0)
                                {
                                    foreach (var c in contextGrandParent.children)
                                    {
                                        if (c is TSqlParser.Search_conditionContext)
                                        {
                                            var greatGreatContext = c as TSqlParser.Search_conditionContext;

                                            var cText = greatGreatContext.GetText();
                                            Console.WriteLine($"EnterSearch_condition - predicate great 2x children: {cText}");
                                            var cInterval = greatGreatContext.SourceInterval;
                                            Console.WriteLine($"EnterSearch_condition - predicate great 2x interval: {cInterval}");

                                            var greatPredicate = greatGreatContext.predicate();
                                            if (greatPredicate != null)
                                            {
                                                var greatText = greatPredicate.GetText();
                                                Console.WriteLine($"EnterSearch_condition - predicate great grandparent children predicate: {greatText}");
                                                var greatInterval = greatPredicate.SourceInterval;
                                                Console.WriteLine($"EnterSearch_condition - predicate great grandparent children predicate interval: {greatInterval}");
                                            }

                                            if (greatGreatContext.ChildCount > 0)
                                            {
                                                foreach (var d in greatGreatContext.children)
                                                {
                                                    var dText = d.GetText();
                                                    Console.WriteLine($"D text: {dText}");
                                                    var dInterval = d.SourceInterval;
                                                    Console.WriteLine($"D Interval: {dInterval}");

                                                    if (d is TSqlParser.Search_conditionContext)
                                                    {
                                                        var dSearch = d as TSqlParser.Search_conditionContext;
                                                        var dPredicate = dSearch.predicate();
                                                        if (dPredicate != null)
                                                        {
                                                            var dPredicateText = dPredicate.GetText();
                                                            Console.WriteLine($"D Predicate Text: {dPredicateText}");
                                                            var dPredicateInterval = dPredicate.SourceInterval;
                                                            Console.WriteLine($"D Predicate Interval: {dPredicateInterval}");
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (context.ChildCount > 1)
            {
                // we have multiple extra predicates
                foreach (var child in context.children)
                {
                    Console.WriteLine("EnterSearch_condition - child");
                    var childText = child.GetText();
                    var childInterval = child.SourceInterval;

                    Console.WriteLine($"Child Text: {childText}");
                    Console.WriteLine($"Child Interval: {childInterval.ToString()}");

                    if (string.Equals("AND", childText, StringComparison.OrdinalIgnoreCase))
                    {
                        hasAnd = true;
                    }

                    if (string.Equals("OR", childText, StringComparison.OrdinalIgnoreCase))
                    {
                        hasOr = true;
                    }
                }
            }

            if (context.ChildCount > 1 && context.Parent != null && context.Parent is TSqlParser.Query_specificationContext)
            {
                // we are at the root level of the search parameters, i.e, the whole WHERE clause
                Debug.WriteLine("At root of where clause");
            }

            if (context.ChildCount == 1 && context.Parent != null && context.Parent is TSqlParser.Search_conditionContext)
            {
                // we are at the lowest level of a predicate
                if (context.Parent.ChildCount > 1)
                {
                    // we can look back at the parent to determine if we had an AND/OR operator previous to this one
                    var parentChildCount = context.Parent.ChildCount;
                    for (int i = 0; i < parentChildCount; i++)
                    {
                        // one of these will be an AND/OR keyword
                        // we can use this to determine how the predicates will operate together
                        var parentChild = context.Parent.GetChild(i);
                        string parentChildDebug = parentChild.GetText();

                        // if this parentChildDebug value is the same as debug (in other words, the same as the level we're at)
                        // it means the preceding predicates must be evaluated first or we've got predicates at the same level we're at that should be evaluated
                        // this is kind of hard to explain
                    }

                }
            }

            Console.WriteLine("---");
        }


        public override void EnterPredicate([NotNull] TSqlParser.PredicateContext context)
        {
            base.EnterPredicate(context);

            string debug = context.GetText();
            var sourceInterval = context.SourceInterval;

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

        public override void EnterInsert_column_id([NotNull] TSqlParser.Insert_column_idContext context)
        {
            base.EnterInsert_column_id(context);

            string debug = context.GetText();

            Console.WriteLine("EnterInsert_column_id:");
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

        public override void EnterExpression([NotNull] TSqlParser.ExpressionContext context)
        {
            base.EnterExpression(context);

            string debug = context.GetText();

            Console.WriteLine("EnterExpression:");
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
            string fullText = GetWhiteSpaceFromCurrentContext(context);

            Console.WriteLine("EnterData_type:");
            Console.WriteLine(debug);
            Console.WriteLine("---");

        }

        public override void EnterSTRINGAGG([NotNull] TSqlParser.STRINGAGGContext context)
        {
            base.EnterSTRINGAGG(context);
        }

        public override void EnterKeyword([NotNull] TSqlParser.KeywordContext context)
        {
            base.EnterKeyword(context);

            string debug = context.GetText();
            string fullText = GetWhiteSpaceFromCurrentContext(context);

            Console.WriteLine("EnterKeyword:");
            Console.WriteLine(debug);
            Console.WriteLine("---");

        }

        public override void EnterConstant([NotNull] TSqlParser.ConstantContext context)
        {
            base.EnterConstant(context);

            string debug = context.GetText();
            string fullText = GetWhiteSpaceFromCurrentContext(context);

            Console.WriteLine("EnterConstant:");
            Console.WriteLine(debug);
            Console.WriteLine("---");
        }

        public override void EnterConstant_expression([NotNull] TSqlParser.Constant_expressionContext context)
        {
            base.EnterConstant_expression(context);

            string debug = context.GetText();
            string fullText = GetWhiteSpaceFromCurrentContext(context);

            Console.WriteLine("EnterConstant_expression:");
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

        public override void EnterDrop_database([NotNull] TSqlParser.Drop_databaseContext context)
        {
            base.EnterDrop_database(context);

            string debug = context.GetText();

            Console.WriteLine("EnterDrop_database:");
            Console.WriteLine(debug);
            Console.WriteLine("---");

        }

        // end create table functions

        // begin create database functions
        public override void EnterCreate_database([NotNull] TSqlParser.Create_databaseContext context)
        {
            base.EnterCreate_database(context);

            string debug = context.GetText();
            string debug2 = GetWhiteSpaceFromCurrentContext(context);

            Console.WriteLine("EnterCreate_database:");
            Console.WriteLine(debug);
            Console.WriteLine("---");


        }

        public override void EnterId_([NotNull] TSqlParser.Id_Context context)
        {
            base.EnterId_(context);

            string debug = context.GetText();
            string debug2 = GetWhiteSpaceFromCurrentContext(context);

            Console.WriteLine("EnterId_:");
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

        private string GetWhiteSpaceFromCurrentContext(ParserRuleContext context)
        {
            int a = context.Start.StartIndex;
            int b = context.Stop.StopIndex;
            Interval interval = new Interval(a, b);
            _charStream = context.Start.InputStream;
            return _charStream.GetText(interval);
        }
        #endregion
    }
}
