using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParserConsole.Query
{
    public class StatementPart
    {
        public string Text { get; set; }
        public List<string> StatementElements { get; set; }

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
