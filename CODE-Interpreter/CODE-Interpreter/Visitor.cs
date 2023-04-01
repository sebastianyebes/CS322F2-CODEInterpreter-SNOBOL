namespace CODE_Interpreter;
public class Visitor : GrammarBaseVisitor<object?>
{
    private Dictionary<string, object?> Variables { get; } = new();
    public override object? VisitAssignment(GrammarParser.AssignmentContext context)
    {
        var varName = context.VARIABLENAME().GetText();

        var value = Visit(context.value());

        Variables[varName] = value;

        return null;
    }
}