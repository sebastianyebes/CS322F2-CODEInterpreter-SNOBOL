using Antlr4.Runtime;
namespace CODE_Interpreter;

public class LexerErrorListener : BaseErrorListener, IAntlrErrorListener<int>
{
    public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        string errorMessage = $"Lexer error at line {line}:{charPositionInLine} - {msg}";
        Console.Error.WriteLine(errorMessage);
    }
}