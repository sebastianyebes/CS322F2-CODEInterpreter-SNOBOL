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
            Console.Write(arg + "\n");
        }

        return null;
    }

    public void DefaultDeclaration(string varDatatype, string varName)
    {
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
        var args = context.displayvalue().Select(Visit).ToArray();

        if (!Functions.ContainsKey(name))
            throw new Exception($"Function {name} is not defined");
        if (Functions[name] is not Func<object?[], object?> func)
            throw new Exception($"Function {name} is not a function");
        
        return func(args);
    }

    public override object? VisitAssignmentList(GrammarParser.AssignmentListContext context)
    {
        var varName = context.value().Select(Visit).ToArray();
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
            else
            {
                var name = varName.Split('=');
                var len = varName.Split('=').Length;
                throw new Exception($"Variable {s} is not defined.");
            }
        }
        return null;
    }

    public override object? VisitVardec(GrammarParser.VardecContext context)
    {
        var declaratorList = context.declaratorlist().GetText();
        string[] variables = declaratorList.Split(',');
        var count = variables.Length;
        var varDatatype = context.DATATYPE().GetText();

        if (declaratorList.Contains('='))
        {
            for (int x = 0; x < count; x++)
            {
                string temp = variables[x];

                if (temp.Contains('='))
                {
                    string[] variable = temp.Split('=');
                    var varName = variable[0];
                    var value = variable[1];
                    int intValue;
                    float floatValue;
                    bool isNum = int.TryParse(value, out intValue), isFloat = float.TryParse(value, out floatValue);

                    for (int i = 0; i < count; i++)
                    {
                        if (i == count - 1)
                        {
                            if (varDatatype == "CHAR" && !isNum)
                            {
                                HasSameType(varName);
                                CharVar[varName] = value;
                            }
                            else if (varDatatype == "INT" && isNum)
                            {
                                HasSameType(varName);
                                IntVar[varName] = intValue;
                            }
                            else if (varDatatype == "FLOAT" && !isNum)
                            {
                                HasSameType(varName);
                                FloatVar[varName] = floatValue;
                            }
                            else if (varDatatype == "BOOL" && (value[1..^1] == "TRUE" || value[1..^1] == "FALSE"))
                            {
                                HasSameType(varName);
                                BoolVar[varName] = value[1..^1];
                            }
                            else
                            {
                                Console.WriteLine(
                                    $"Invalid value for variable {varName}: expected to be {varDatatype}");

                            }

                            break;
                        }
                    }
                }
                else
                {
                    HasSameType(variables[x]);
                    DefaultDeclaration(varDatatype, variables[x]);
                }
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                HasSameType(variables[i]);
                DefaultDeclaration(varDatatype, variables[i]);
            }
        }

        return null;
    }

    public override object? VisitVariablename(GrammarParser.VariablenameContext context)
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

    public override object? VisitDisplayvariablenameExpression(GrammarParser.DisplayvariablenameExpressionContext context)
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

    public override object? VisitValuevariablenameExpression(GrammarParser.ValuevariablenameExpressionContext context)
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

    public override object? VisitStringvalExpression(GrammarParser.StringvalExpressionContext context)
    {
        if (context.STRINGVAL() is { } c)
        {
            return c.GetText()[1..^1];
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

        if (context.CHARVAL() is { } c)
        {
            return c.GetText()[1..^1];
        }

        if (context.BOOLVAL() is { } b)
        {
            return b.GetText()[1..^1];
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
        var left = Visit(context.displayvalue(0))?.ToString();
        var right = Visit(context.displayvalue(1))?.ToString();
        var op = context.concOp().GetText();

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