namespace CODE_Interpreter;
public class Visitor : GrammarBaseVisitor<object?>
{
    public Dictionary<string, object?> Variables { get; } = new();
    
    public Visitor()
    {
        Variables["DISPLAY"] = new Func<object?[], object?>(Display);
    }

    private object? Display(object?[] args)
    {
        foreach (var arg in args)
        {
            Console.WriteLine(arg);
        }

        return null;
    }

    public override object? VisitFunctionCall(GrammarParser.FunctionCallContext context)
    {
        var name = context.VARIABLENAME().GetText();
        var args = context.value().Select(Visit).ToArray();
        //var args = context.argList()?.value()?.Select(x => Visit(x)).ToArray();

        if (!Variables.ContainsKey(name))
        {
            throw new Exception($"Function {name} is not defined");
        }

        if (Variables[name] is not Func<object?[], object?> func)
        {
            throw new Exception($"Variable {name} is not a function");
        }

        return func(args);
    }

    public override object? VisitAssignment(GrammarParser.AssignmentContext context)
    {
        var varName = context.VARIABLENAME().GetText();

        var value = Visit(context.value());

        Variables[varName] = value;

        return null;
    }

    public override object? VisitVariablenameExpression(GrammarParser.VariablenameExpressionContext context)
    {
        var varName = context.VARIABLENAME().GetText();

        if (!Variables.ContainsKey(varName))
        {
            throw new Exception($"Variable {varName} is not defined");
        }

        return Variables[varName];
    }
    public override object? VisitVardec(GrammarParser.VardecContext context)
    {
        var varName = context.DATATYPE().GetText();

        var declaratorlist = context.declaratorlist();

        Variables[varName] = declaratorlist;

        return null;
    }
    public override object? VisitDeclarator(GrammarParser.DeclaratorContext context)
    {
        var varName = context.VARIABLENAME().GetText();
        
        if (Variables.ContainsValue(varName))
        {
            var value = Visit(context.value());
            
            Variables[varName] = value;
        }

        return null;
    }

    public override object? VisitConstant(GrammarParser.ConstantContext context)
    {
        if (context.INTEGERVAL() != null)
        {
            return int.Parse(context.INTEGERVAL().GetText());
        }
        if (context.FLOATVAL() != null)
        {
            return float.Parse(context.FLOATVAL().GetText());
        }
        if (context.CHARVAL() != null)
        {
            return context.CHARVAL().GetText();
        }
        if (context.BOOLVAL() != null)
        {
            return context.BOOLVAL().GetText() == "TRUE";
        }
        
        return null;
    }

    public override object? VisitAdditiveExpression(GrammarParser.AdditiveExpressionContext context)
    {
        var left = Visit(context.value(0));
        var right = Visit(context.value(1));

        var op = context.addOp().GetText();

        return op switch
        {
            "+" => Add(left, right),
            //"-" => Subtract(left, right),
            _ => throw new NotImplementedException()
        };
    }

    private object? Add(object? left, object? right)
    {
        if (left is int l && right is int r)
        {
            return l + r;
        }

        if (left is float lf && right is float rf)
        {
            return lf + rf;
        }

        if (left is int lInt && right is float rFloat)
        {
            return lInt + rFloat;
        }
        
        if (left is float lFloat && right is int rInt)
        {
            return lFloat + rInt;
        }

        if (left is string)
        {
            return $"{left}{right}";
        }

        if (left is string || right is string)
        {
            return $"{left}{right}";
        }

        throw new NotImplementedException($"Cannot add values of types {left?.GetType()} and {right?.GetType()}");
    }
}