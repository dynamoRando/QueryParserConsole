using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ConsoleAntlrSQL;
using Newtonsoft.Json;
using QueryParserConsole.Query;
using System;

namespace QueryParserConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string failure = "THE QUICK BROWN FOX";
            string defaultInput = "SELECT NAME, AGE, RANK FROM EMPLOYEE WHERE ((NAME LIKE '%RANDY%' AND RANK = 2 OR NAME = 'MEGAN') AND AGE > 32) OR (NAME = 'BRIAN')";
            string defaultInput2 = "SELECT NAME, AGE, RANK FROM EMPLOYEE WHERE NAME LIKE '%RANDY%' OR RANK = 2 ";
            string defaultInput3 = "SELECT NAME, AGE FROM EMPLOYEE WHERE NAME = 'RANDY'";
            string inputA = "SELECT NAME FROM EMPLOYEE";
            string inputB = "SELECT NAME FROM EMPLOYEE WHERE (AGE > 20)";
            string insertStatement = "INSERT INTO EMPLOYEE (NAME, AGE, MANAGER) VALUES ('RANDY', 35, 'MEGAN')";
            string insertStatement2 = "INSERT INTO EMPLOYEE (NAME, AGE, MANAGER) VALUES ('RANDY', 35, 'MEGAN'), ('MEGAN', 36, 'MEGAN'), ('CAM', 38, 'MEGAN')";
            string updateStatement = "UPDATE EMPLOYEE SET NAME = 'RANDY LE', AGE = 36 WHERE NAME = 'RANDY' AND AGE = 35";
            string deleteStatement = "DELETE FROM EMPLOYEE WHERE NAME = 'JIM'";
            string createDbStatement = "CREATE DATABASE BIN1";

            string createTable = @"
            CREATE TABLE EMPLOYEE
            (
                ID INT IDENTITY(1,1),
                EMPLOYEENAME NVARCHAR(25) NOT NULL,
                HIREDATE DATETIME NOT NULL,
                TERMDATE DATETIME NULL
            );
            ";

            Console.WriteLine("QueryParserConsole. Used to test how Antlr will parse queries.");
            Console.WriteLine("Enter a query to parse or (d) for default.");
            var input = Console.ReadLine();

            if (input.Equals("default"))
            {
                input = defaultInput;
            }

            if (input.Equals("default2"))
            {
                input = defaultInput2;
            }

            if (input.Equals("default3"))
            {
                input = defaultInput3;
            }

            if (input.Equals("a"))
            {
                input = inputA;
            }

            if (input.Equals("b"))
            {
                input = inputB;
            }

            if (input.Equals("failure"))
            {
                input = failure;
            }

            if (input.Equals("i"))
            {
                input = insertStatement;
            }

            if (input.Equals("i2"))
            {
                input = insertStatement2;
            }

            if (input.Equals("u"))
            {
                input = updateStatement;
            }

            if (input.Equals("d"))
            {
                input = deleteStatement;
            }

            if (input.Equals("ct"))
            {
                input = createTable;
            }

            if (input.Equals("cdb"))
            {
                input = createDbStatement;
            }

            Console.WriteLine("Will parse the following statement:");
            Console.WriteLine(input);

            if (input.Contains("CREATE"))
            {
                ParseDDLClause(input);
            }
            else
            {
                ParseInput(input.ToUpper());
            }

            Console.WriteLine("Finished. Press any key to exit.");
            Console.ReadLine();
        }

        static void ParseDDLClause(string input)
        {
            AntlrInputStream inputStream = new AntlrInputStream(input);
            TSqlLexer lexer = new TSqlLexer(inputStream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            TSqlParser parser = new TSqlParser(tokens);

            var errorHandler = new SyntaxErrorListener();
            parser.AddErrorListener(errorHandler);

            var parseTree = parser.ddl_clause();
            ParseTreeWalker walker = new ParseTreeWalker();
            TSqlParserListenerExtended loader = new TSqlParserListenerExtended(new SelectStatement());
            loader.TokenStream = tokens;
            walker.Walk(loader, parseTree);
            Console.WriteLine("Parse Tree:");
            Console.WriteLine(parseTree.ToStringTree(parser));

            Console.WriteLine("Errors:");
            foreach (var error in errorHandler.Errors)
            {
                Console.WriteLine(error.Message + " at position " + error.Line.ToString() + ":" + error.CharPositionInLine.ToString());
            }

            Console.Write("Press enter key to continue");
            Console.ReadLine();
        }

        static void ParseInput(string input)
        {
            AntlrInputStream inputStream = new AntlrInputStream(input);
            TSqlLexer lexer = new TSqlLexer(inputStream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            TSqlParser parser = new TSqlParser(tokens);

            var errorHandler = new SyntaxErrorListener();
            parser.AddErrorListener(errorHandler);


            var parseTree = parser.dml_clause();
            ParseTreeWalker walker = new ParseTreeWalker();
            TSqlParserListenerExtended loader = new TSqlParserListenerExtended(new SelectStatement());
            loader.TokenStream = tokens;
            walker.Walk(loader, parseTree);
            Console.WriteLine("Parse Tree:");
            Console.WriteLine(parseTree.ToStringTree(parser));

            Console.WriteLine("Errors:");
            foreach (var error in errorHandler.Errors)
            {
                Console.WriteLine(error.Message + " at position " + error.Line.ToString() + ":" + error.CharPositionInLine.ToString());
            }

            if (input.Contains("SELECT"))
            {
                var selectStatement = loader.GetStatementAsSelect();
                var text = JsonConvert.SerializeObject(selectStatement);
                Console.WriteLine("Review Parse. Press any key to continue.");
                Console.ReadLine();
                Console.WriteLine(text);

                Console.WriteLine("Executing Generated Plan");
                GeneratePlan(selectStatement);
            }

            Console.Write("Press enter key to continue");
            Console.ReadLine();
        }

        static void GeneratePlan(SelectStatement statement)
        {
            var executor = new QueryPlanExecutor();
            var generator = new SelectQueryPlanGenerator();
            var plan = generator.GeneratePlan(statement);
            executor.Execute(plan);
        }
    }
}
