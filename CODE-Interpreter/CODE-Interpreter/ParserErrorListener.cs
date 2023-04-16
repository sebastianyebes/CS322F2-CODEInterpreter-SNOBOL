using Antlr4.Runtime;
namespace CODE_Interpreter;

public class ParserErrorListener : BaseErrorListener, IAntlrErrorListener<IToken>
{ 
    public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        if (offendingSymbol == null)
        {
            // Handle the case where the offending token is null
            Console.Error.WriteLine($"Token recognition error at line {line}:{charPositionInLine} - {msg}");
        }
        else
        {
            string tokenName = recognizer.Vocabulary.GetDisplayName(offendingSymbol.Type);
            string errorMessage = $"Token recognition error at line {line}:{charPositionInLine} - {msg}";
            Console.Error.WriteLine(errorMessage);
        }
    }
}