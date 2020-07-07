using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using QueryParserConsole.Query;
using System;

namespace QueryParserConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string defaultInput = "SELECT NAME, AGE, RANK FROM EMPLOYEE WHERE ((NAME LIKE '%RANDY%' AND RANK = 2 OR NAME = 'MEGAN') AND AGE > 32) OR (NAME = 'BRIAN')";
            
            Console.WriteLine("QueryParserConsole. Used to test how Antlr will parse queries.");
            Console.WriteLine("Enter a query to parse or (d) for default.");
            var input = Console.ReadLine();
            if (input.Equals("d"))
            {
                input = defaultInput;
            }

            ParseInput(input);
            Console.WriteLine("Finished. Press any key to exit.");
            Console.ReadLine();
        }

        static void ParseInput(string input)
        {
            AntlrInputStream inputStream = new AntlrInputStream(input);
            TSqlLexer lexer = new TSqlLexer(inputStream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            TSqlParser parser = new TSqlParser(tokens);
            var parseTree = parser.dml_clause();
            ParseTreeWalker walker = new ParseTreeWalker();
            TSqlParserListenerExtended loader = new TSqlParserListenerExtended(new SelectStatement());
            walker.Walk(loader, parseTree);
            Console.WriteLine("Parse Tree:");
            Console.WriteLine(parseTree.ToStringTree(parser));
        }
    }
}
