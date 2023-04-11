using Microsoft.VisualBasic.CompilerServices;
using System.Linq.Expressions;

namespace CODE_Interpreter;
public class Visitor : GrammarBaseVisitor<object?>
{
    public Dictionary<string, object?> Functions { get; } = new();
    public Dictionary<string, object?> CharVar { get; } = new();
    public Dictionary<string, object?> IntVar { get; } = new();
    public Dictionary<string, object?> FloatVar { get; } = new();
    public Dictionary<string, object?> BoolVar { get; } = new();
    
    public Visitor()
    {
        Functions["DISPLAY"] = new Func<object?[], object?>(Display);
    }

    private object? Display(object?[] args)
    {
        foreach (var arg in args)
        {
            Console.Write(arg);
        }

        return null;
    }

    public void DefaultDeclaration(string varDatatype, string varName)
    {
        HasSameType(varName);
        switch (varDatatype)
        {
            case "CHAR":
                CharVar[varName] = ' ';
                break;
            case "INT":
                IntVar[varName] = 0;
                break;
            case "FLOAT":
                FloatVar[varName] = 0.0;
                break;
            case "BOOL":
                BoolVar[varName] = null;
                break;
            default:
                throw new Exception($"Invalid assignment for variable {varName}: expected to be {varDatatype}");
        }
    }
    public void HasSameType(string varName)
    {
        bool hasSame = CharVar.ContainsKey(varName) || IntVar.ContainsKey(varName) || FloatVar.ContainsKey(varName) || BoolVar.ContainsKey(varName);

        if (hasSame)
            throw new Exception($"Multiple declaration of Variable {varName}");
    }
    
    public override object? VisitFunctionCall(GrammarParser.FunctionCallContext context)
    {
        var name = context.FUNCTIONNAME().GetText();
        var args = context.value().Select(Visit).ToArray();
        
        if(args.Length == 0)
            throw new Exception($"Display has no input");
        
        var argType = context.value(0).GetType().ToString();
        if(argType == "CODE_Interpreter.GrammarParser+ConstantExpressionContext" && (args[0] is int || args[0] is float))
            throw new Exception($"Invalid operands for concatenation");
        if (!Functions.ContainsKey(name))
            throw new Exception($"Function {name} is not defined");
        if (Functions[name] is not Func<object?[], object?> func)
            throw new Exception($"Function {name} is not a function");
        
        return func(args);
    }

    public override object? VisitAssignmentList(GrammarParser.AssignmentListContext context)
    {
        var varName = context.VARIABLENAME().Select(Visit).ToArray();
        return varName;
    }

    public override object? VisitAssignment(GrammarParser.AssignmentContext context)
    {
        var varName = context.assignmentList().GetText();
        var ass = varName.Split('=');
        var value = Visit(context.value());
        
        foreach (string s in ass)
        {
            if (CharVar.ContainsKey(s))
            {
                if (value is string | value is char)
                {
                    CharVar[s] = value;
                }
                else
                {
                    throw new Exception($"Invalid assignment for variable {varName}: expected to be CHAR");
                }
            }
            else if (IntVar.ContainsKey(s))
            {
                if (value is int)
                {
                    IntVar[s] = value;
                }
                else
                {
                    throw new Exception($"Invalid assignment for variable {varName} : expected to be INT");
                }
            }
            else if (FloatVar.ContainsKey(s))
            {
                if (value is float)
                {
                    FloatVar[s] = value;
                }
                else
                {
                    throw new Exception($"Invalid assignment for variable {varName}: expected to be FLOAT");
                }
            }
            else if (BoolVar.ContainsKey(s))
            {
                if (value is "TRUE" || value is "FALSE")
                {
                    BoolVar[s] = value;
                }
                else
                {
                    throw new Exception($"Invalid assignment for variable {varName}: expected to be BOOL");
                }
            }
        }
        return null;
    }
    public override object? VisitVardec(GrammarParser.VardecContext context)
    {
        var varDatatype = context.DATATYPE().GetText();
        var varDeclarator = context.declaratorlist().GetText();
        string[] variableList = varDeclarator.Split(',');
        int variableCount = variableList.Length;

        if (varDeclarator.Contains('='))
        {
            string[] lastVariable = variableList[variableCount - 1].Split('=');
            var varName = lastVariable[0];
            var value = lastVariable[1];
            int intValue;
            float floatValue;
            bool isNum = int.TryParse(value, out intValue), isFloat = float.TryParse(value, out floatValue);

            if (!isNum || !isFloat)
                value = value[1..^1];
            
            for (int i = 0; i < variableCount; i++)
            {
                HasSameType(varName);
                if (i == variableCount - 1)
                {
                    if (varDatatype == "CHAR" && !isNum)
                    {
                        CharVar[varName] = value;
                    }
                    else if (varDatatype == "INT" && isNum)
                    {
                        IntVar[varName] = intValue;
                    }
                    else if (varDatatype == "FLOAT" && isFloat)
                    {
                        FloatVar[varName] = floatValue;
                    }
                    else if (varDatatype == "BOOL" && (value == "TRUE" || value == "FALSE"))
                    {
                        BoolVar[varName] = value;
                    }
                    else
                    {
                        throw new Exception($"Invalid assignment for variable {varName}: expected to be {varDatatype}");
                    }
                    break;
                }
                DefaultDeclaration(varDatatype, variableList[i]);
            }
        }
        else
        {
            for (int i = 0; i < variableCount; i++)
            {
                DefaultDeclaration(varDatatype, variableList[i]);
            }
        }

        return null;
    }

    public override object? VisitVariablenameExpression(GrammarParser.VariablenameExpressionContext context)
    {
        var varName = context.VARIABLENAME().GetText();

        if (CharVar.ContainsKey(varName))
        {
            return CharVar[varName];
        }
        if (IntVar.ContainsKey(varName))
        {
            return IntVar[varName];
        }
        if (FloatVar.ContainsKey(varName))
        {
            return FloatVar[varName];
        }
        if (BoolVar.ContainsKey(varName))
        {
            return BoolVar[varName];
        }

        throw new Exception($"Variable {varName} is not defined");
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
        if (context.CHARVAL() is {} c)
        {
            return c.GetText()[1..^1];
        }
        if (context.BOOLVAL() != null)
        {
            return context.BOOLVAL().GetText() == "TRUE";
        }

        if (context.STRINGVAL() is { } s)
        {
            return s.GetText()[1..^1];
        }
        
        return null;
    }

    public override object? VisitNewlineopExpression(GrammarParser.NewlineopExpressionContext context)
    {
        if (context.NEWLINEOP() != null)
            return "\n";
        
        return null;
    }

    public override object? VisitConcatenateExpression(GrammarParser.ConcatenateExpressionContext context)
    {
        var left = Visit(context.value(0))?.ToString();
        var right = Visit(context.value(1))?.ToString();
        var op = context.concOp().GetText();

        var leftValType = Visit(context.value(0));
        var rightValType = Visit(context.value(1));
        var leftType = context.value(0).GetType().ToString();
        var rightType = context.value(1).GetType().ToString();

        if((leftType == "CODE_Interpreter.GrammarParser+ConstantExpressionContext" || rightType == "CODE_Interpreter.GrammarParser+ConstantExpressionContext") 
           && ((leftValType is int || rightValType is int) || (leftValType is float || rightValType is float)))
            throw new Exception($"Invalid operands for concatenation");
        
        if (op == "&")
        {
            if (!string.IsNullOrEmpty(left) && !string.IsNullOrEmpty(right))
            {
                return left + right;
            }
            throw new Exception($"Invalid operands for concatenation: {(string.IsNullOrEmpty(left) ? "left" : "right")} operand is null or empty.");
        }

        throw new Exception($"Invalid concatenation operator: '{op}'");
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

        throw new NotImplementedException($"Cannot add values of types {left?.GetType()} and {right?.GetType()}");
    }
}