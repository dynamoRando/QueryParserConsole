using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QueryParserConsole
{
    public readonly struct SyntaxError
    {
        public readonly IRecognizer Recognizer;
        public readonly IToken OffendingSymbol;
        public readonly int Line;
        public readonly int CharPositionInLine;
        public readonly string Message;
        public readonly RecognitionException Exception;

        public SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line,
                           int charPositionInLine, string message, RecognitionException exception)
        {
            Recognizer = recognizer;
            OffendingSymbol = offendingSymbol;
            Line = line;
            CharPositionInLine = charPositionInLine;
            Message = message;
            Exception = exception;
        }
    }

    public class SyntaxErrorListener : BaseErrorListener
    {
        public readonly List<SyntaxError> Errors = new List<SyntaxError>();

        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
            Errors.Add(new SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e));
        }
    }
}
