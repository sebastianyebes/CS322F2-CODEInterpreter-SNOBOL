using Antlr4.Runtime;
namespace CODE_Interpreter;

public class ParserErrorListener : BaseErrorListener, IAntlrErrorListener<IToken>
{ 
    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        string errorMessage = $"Token recognition error at line {line}:{charPositionInLine} - {msg}";
        Console.Error.WriteLine(errorMessage);
    }
}