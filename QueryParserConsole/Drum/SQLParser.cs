using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParserConsole.Drum
{
    class SQLParser : TSqlParserListenerExtended
    {
        #region Private Fields
        private PredicateBucket _bucket;
        #endregion

        #region Public Properties
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

            }
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

        #region Private Methods
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
        #endregion

    }
}
