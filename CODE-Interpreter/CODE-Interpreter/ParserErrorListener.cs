using Antlr4.Runtime;
namespace CODE_Interpreter;

public class ParserErrorListener : BaseErrorListener, IAntlrErrorListener<IToken>
{ 
    private readonly WriteExitHelper _helper = new WriteExitHelper();
    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        string tokenName = recognizer.Vocabulary.GetDisplayName(offendingSymbol.Type);
        string errorMessage = $"Token recognition error at line {line}:{charPositionInLine} - {msg}";
        _helper.WriteLineAndExit($"{errorMessage}");
    }
}