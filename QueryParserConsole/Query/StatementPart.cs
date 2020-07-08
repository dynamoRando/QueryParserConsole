using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParserConsole.Query
{
    public class StatementPart
    {
        public string Text { get; set; }
        // if the text can be broken down into elements
        public List<string> StatementElements { get; set; }
        // the function that the statement came from
        public string StatementOrigin { get; set; }
        // the text above the current statement
        public string StatementParent { get; set; }
        public string StatementGrandParent { get; set; }

        public StatementPart()
        {
            StatementElements = new List<string>();
        }

        public StatementPart(string text, List<string> elements)
        {
            Text = text;
            StatementElements = elements;
        }
    }
}
