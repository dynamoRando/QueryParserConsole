using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TSqlParser;
using a = Antlr4.Runtime.Misc;
using d = QueryParserConsole.Drum;

namespace QueryParserConsole.Drum
{
    class SQLParser : TSqlParserBaseListener
    {
        #region Private Fields
        private PredicateBucket _bucket;
        ICharStream _charStream;
        #endregion

        #region Public Properties
        public CommonTokenStream TokenStream { get; set; }
        #endregion

        #region Constructors
        public SQLParser()
        {
            _bucket = new PredicateBucket();
        }
        #endregion

        #region Public Methods
        public override void EnterSearch_condition([NotNull] TSqlParser.Search_conditionContext context)
        {
            string debug = context.GetText();
            var interval = context.SourceInterval;

            Console.WriteLine("EnterSearch_condition:");
            Console.WriteLine($"Text: {debug}");
            Console.WriteLine($"Interval: {interval.ToString()}");

            var predicate = context.predicate();
            if (predicate != null)
            {
                string predicateText = predicate.GetText();
                var predicateInterval = predicate.SourceInterval;
                var bucketInterval = new Interval { A = predicateInterval.a, B = predicateInterval.b };

                d.Predicate newPredicate = null;

                if (!HasPredicate(bucketInterval))
                {
                    if (PredicateHasBool(context))
                    {
                        // we need to determine if we need to add a boolean predicate
                        // or regular predicate
                        WalkTree(context);
                    }
                    else
                    {
                        // just add the predicate
                        newPredicate = new d.Predicate(GetNextPredicateId());
                        newPredicate.SetInterval(bucketInterval);
                        AddPredicate(newPredicate);
                    }
                }
            }
        }

        public override void ExitSelect_statement([NotNull] Select_statementContext context)
        {
            base.ExitSelect_statement(context);
        }

        public void DebugBucket()
        {
            foreach (var predicate in _bucket.Predicates)
            {
                if (predicate is BoolPredicate)
                {
                    var bPredicate = predicate as BoolPredicate;
                    DebugBoolPredicate(bPredicate);
                }
                else
                {
                    var regularPredicate = predicate as Predicate;
                    DebugPredicate(regularPredicate);
                }
            }
        }

        #endregion
        private bool ContextHasPredicate(TSqlParser.Search_conditionContext context)
        {
            var predicate = context.predicate();
            if (predicate != null)
            {
                return true;
            }

            return false;

        }

        private bool PredicateHasBool(RuleContext context)
        {
            if (context.Parent.ChildCount > 0)
            {
                if (context.Parent is TSqlParser.Search_conditionContext)
                {
                    var parent = context.Parent as TSqlParser.Search_conditionContext;

                    bool anyChildrenHaveBools = parent.children.Any(child => string.Equals(child.GetText(), "AND", StringComparison.OrdinalIgnoreCase) || string.Equals(child.GetText(), "OR", StringComparison.OrdinalIgnoreCase));

                    if (anyChildrenHaveBools)
                    {
                        return true;
                    }
                    else
                    {
                        bool anyChildrenHaveParen = parent.children.Any(child => child.GetText().StartsWith("(") || child.GetText().StartsWith(")"));

                        if (anyChildrenHaveParen)
                        {
                            foreach (var child in parent.children)
                            {
                                var childText = child.GetText();
                                if (childText.StartsWith("(") || childText.StartsWith(")"))
                                {
                                    continue;
                                }
                                else
                                {
                                    if (child is TSqlParser.Search_conditionContext)
                                    {
                                        var recursiveChild = child as TSqlParser.Search_conditionContext;
                                        return PredicateHasBool(recursiveChild.Parent);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private d.Predicate GetDrumPredicate(TSqlParser.PredicateContext context)
        {

            var predicateInterval = context.SourceInterval;
            var bucketInterval = new Interval { A = predicateInterval.a, B = predicateInterval.b };

            var newPredicate = new d.Predicate(GetNextPredicateId());
            newPredicate.SetInterval(bucketInterval);
            newPredicate.Text = GetWhiteSpaceFromCurrentContext(context);

            return newPredicate;
        }

        private d.Predicate GetDrumPredicate(TSqlParser.Search_conditionContext context)
        {
            var predicate = context.predicate();

            if (predicate is not null)
            {
                var predicateInterval = predicate.SourceInterval;
                var bucketInterval = new Interval { A = predicateInterval.a, B = predicateInterval.b };

                var newPredicate = new d.Predicate(GetNextPredicateId());
                newPredicate.SetInterval(bucketInterval);
                newPredicate.Text = GetWhiteSpaceFromCurrentContext(predicate);

                return newPredicate;
            }

            return null;


        }

        private bool HasBooleanPredicate(d.Predicate predicate)
        {
            foreach (var item in _bucket.Predicates)
            {
                if (item is BoolPredicate)
                {
                    var foo = item as BoolPredicate;

                    if (foo.Left is Predicate)
                    {
                        if (foo.Left.Interval.A == predicate.Interval.A && foo.Left.Interval.B == predicate.Interval.B)
                        {
                            return true;
                        }
                    }

                    if (foo.Right is Predicate)
                    {
                        if (foo.Right.Interval.A == predicate.Interval.A && foo.Right.Interval.B == predicate.Interval.B)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private BoolPredicate GetBooleanPredicate(Interval interval)
        {
            var foo = _bucket.Predicates.Where(p => p.Interval.A == interval.A && p.Interval.B == interval.B).FirstOrDefault();
            if (foo is not null)
            {
                if (foo is BoolPredicate)
                {
                    return foo as BoolPredicate;
                }
            }

            return null;
        }

        private BoolPredicate GetBooleanPredicate(d.Predicate predicate)
        {
            foreach (var item in _bucket.Predicates)
            {
                if (item is BoolPredicate)
                {
                    var foo = item as BoolPredicate;

                    if (foo.Left is Predicate)
                    {
                        if (foo.Left.Interval.A == predicate.Interval.A && foo.Left.Interval.B == predicate.Interval.B)
                        {
                            return foo;
                        }
                    }

                    if (foo.Right is Predicate)
                    {
                        if (foo.Right.Interval.A == predicate.Interval.A && foo.Right.Interval.B == predicate.Interval.B)
                        {
                            return foo;
                        }
                    }
                }
            }

            return null;
        }

        private void WalkTree(RuleContext context)
        {
            bool hasBoolean = false;
            d.Predicate drumPredicate = null;
            string boolText = string.Empty;

            if (context is TSqlParser.Search_conditionContext)
            {
                string debug = context.GetText();
                var searchContext = context as TSqlParser.Search_conditionContext;
                var searchPredicate = searchContext.predicate();
                if (searchPredicate is not null)
                {
                    // do something with the predicate
                    drumPredicate = GetDrumPredicate(searchPredicate);
                }
                else
                {
                    foreach (var c in searchContext.children)
                    {
                        if (c is TSqlParser.Search_conditionContext)
                        {
                            var x = c as TSqlParser.Search_conditionContext;
                            var p = x.predicate();
                            if (p is not null)
                            {
                                drumPredicate = GetDrumPredicate(p);
                            }
                        }
                    }
                }
            }

            if (context.Parent.ChildCount > 0)
            {
                if (context.Parent is TSqlParser.Search_conditionContext)
                {
                    var parent = context.Parent as TSqlParser.Search_conditionContext;

                    bool anyChildrenHaveBool = parent.children.Any(child => string.Equals(child.GetText(), "AND", StringComparison.OrdinalIgnoreCase) || string.Equals(child.GetText(), "OR", StringComparison.OrdinalIgnoreCase));

                    if (anyChildrenHaveBool)
                    {
                        foreach (var child in parent.children)
                        {
                            var childText = child.GetText();
                            if (childText == "AND" || childText == "OR")
                            {
                                hasBoolean = true;
                                boolText = childText;
                            }
                        }

                        foreach (var child in parent.children)
                        {
                            var text = child.GetText();
                            if (!string.Equals("AND", text, StringComparison.OrdinalIgnoreCase) && !string.Equals("OR", text, StringComparison.OrdinalIgnoreCase))
                            {
                                if (child is Search_conditionContext)
                                {
                                    var c = child as Search_conditionContext;
                                    var dPredicate = GetDrumPredicate(c);

                                    if (dPredicate is null)
                                    {
                                        // do something
                                        var childInterval = child.SourceInterval;
                                        var dChildInterval = new Interval { A = childInterval.a, B = childInterval.b };

                                        var textCharacters = text.ToCharArray();
                                        foreach (var ch in textCharacters)
                                        {
                                            if (ch == Char.Parse("("))
                                            {
                                                dChildInterval.A += 1;
                                            }

                                            /*
                                            if (ch == Char.Parse(")"))
                                            {
                                                dChildInterval.B -= 1;
                                            }
                                            */
                                        }

                                        if (text.EndsWith(")"))
                                        {
                                            dChildInterval.B -= 1;
                                        }

                                        var foo = dChildInterval;

                                        if (HasPredicate(dChildInterval))
                                        {
                                            // we need to point this predicate to the previous boolean interval
                                            var existingBoolean = GetBooleanPredicate(dChildInterval);
                                            if (existingBoolean is not null)
                                            {
                                                if (drumPredicate is not null)
                                                {
                                                    var booleanPredicate = new BoolPredicate(GetNextPredicateId());
                                                    booleanPredicate.Boolean = boolText;
                                                    booleanPredicate.Left = existingBoolean;
                                                    booleanPredicate.Right = drumPredicate;
                                                    AddPredicate(booleanPredicate);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                return;
                                            }
                                        }
                                    }

                                    // we need to check to see if there is already a boolean in the bucket
                                    // for this predicate

                                    if (drumPredicate is not null)
                                    {
                                        if (HasBooleanPredicate(drumPredicate))
                                        {
                                            var existingBoolean = GetBooleanPredicate(drumPredicate);
                                            existingBoolean.Right = dPredicate;
                                        }
                                        else
                                        {
                                            var booleanPredicate = new BoolPredicate(GetNextPredicateId());
                                            booleanPredicate.Boolean = boolText;
                                            booleanPredicate.Left = drumPredicate;
                                            AddPredicate(booleanPredicate);
                                        }
                                    }


                                }
                            }

                        }

                        return;
                    }

                    // if we don't have any bools, we need to scan the child parent just in case
                    // see if we're caught in between parenthesis

                    bool anyChildrenHaveParen = parent.children.Any(child => child.GetText().StartsWith("(") || child.GetText().StartsWith(""));
                    if (anyChildrenHaveParen)
                    {
                        foreach (var child in parent.children)
                        {
                            var childText = child.GetText();
                            if (childText.StartsWith("(") || childText.StartsWith(")"))
                            {
                                continue;
                            }
                            else
                            {
                                if (child is TSqlParser.Search_conditionContext)
                                {
                                    var recursiveChild = child as TSqlParser.Search_conditionContext;

                                    string debug = recursiveChild.Parent.GetText();
                                    string debugGrandParent = recursiveChild.Parent.Parent.GetText();

                                    var grandParent = recursiveChild.Parent.Parent;

                                    if (grandParent is TSqlParser.Search_conditionContext)
                                    {
                                        var gp = grandParent as TSqlParser.Search_conditionContext;
                                        foreach (var grandChild in gp.children)
                                        {
                                            if (grandChild is TSqlParser.Search_conditionContext)
                                            {
                                                var gc = grandChild as TSqlParser.Search_conditionContext;
                                                WalkTree(gc);
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

        #region Private Methods
        private void AddPredicate(IPredicate predicate)
        {
            _bucket.Predicates.Add(predicate);
        }

        private bool HasPredicate(Interval interval)
        {
            foreach (var predicate in _bucket.Predicates)
            {
                if (predicate is BoolPredicate)
                {
                    var bp = predicate as BoolPredicate;

                    if (bp.Left is not null)
                    {
                        if (bp.Left is Predicate)
                        {
                            if (bp.Left.Interval.A == interval.A && bp.Left.Interval.B == interval.B)
                            {
                                return true;
                            }
                        }
                    }

                    if (bp.Right is not null)
                    {
                        if (bp.Right is Predicate)
                        {
                            if (bp.Right.Interval.A == interval.A && bp.Right.Interval.B == interval.B)
                            {
                                return true;
                            }
                        }
                    }

                    if (bp.Interval.A == interval.A && bp.Interval.B == interval.B)
                    {
                        return true;
                    }

                }
                else
                {
                    if (predicate.Interval.A == interval.A && predicate.Interval.B == interval.B)
                    {
                        return true;
                    }

                }
            }

            return false;
        }

        private int GetNextPredicateId()
        {
            if (_bucket.Predicates.Count == 0)
            {
                return 1;
            }

            var maxId = _bucket.Predicates.Max(p => p.Id);
            return maxId + 1;
        }

        private void DebugBoolPredicate(BoolPredicate predicate)
        {
            Console.WriteLine($"Bool Predicate Id: {predicate.Id.ToString()}");
            Console.WriteLine($"Bool Interval: {predicate.Interval.A.ToString()}:{predicate.Interval.B.ToString()}");

            if (predicate.Left is not null)
            {
                if (predicate.Left is BoolPredicate)
                {
                    Console.WriteLine($"Bool Predicate Id: {predicate.Id.ToString()} Left Is Bool, evaluating...");
                    DebugBoolPredicate(predicate.Left as BoolPredicate);
                }
                else
                {
                    DebugPredicate(predicate.Left as Predicate);
                }
            }

            if (predicate.Right is not null)
            {
                if (predicate.Right is BoolPredicate)
                {
                    Console.WriteLine($"Bool Predicate Id: {predicate.Id.ToString()} Right Is Bool, evaluating...");
                    DebugBoolPredicate(predicate.Right as BoolPredicate);
                }
                else
                {
                    DebugPredicate(predicate.Right as Predicate);
                }
            }

        }

        private void DebugPredicate(Predicate predicate)
        {
            Console.WriteLine($"Predicate Id: {predicate.Id.ToString()}");
            Console.WriteLine($"Predicate Interval: {predicate.Interval.A.ToString()}:{predicate.Interval.B.ToString()}");
            Console.WriteLine($"Predicate Text: {predicate.Text}");
        }

        private string GetWhiteSpaceFromCurrentContext(ParserRuleContext context)
        {
            int a = context.Start.StartIndex;
            int b = context.Stop.StopIndex;
            a.Interval interval = new a.Interval(a, b);
            _charStream = context.Start.InputStream;
            return _charStream.GetText(interval);
        }

        private string GetWhitespaceStringFromTokenInterval(a.Interval interval)
        {
            try
            {
                var start = TokenStream.Get(interval.a).StartIndex;
                var end = TokenStream.Get(interval.b).StopIndex;
                a.Interval i = new a.Interval(start, end);
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
