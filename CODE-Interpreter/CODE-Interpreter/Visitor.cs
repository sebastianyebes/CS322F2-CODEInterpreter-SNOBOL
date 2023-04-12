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

    public override object? VisitStatement(GrammarParser.StatementContext context)
    {
        var newLine = context.NEWLINE().GetText();

        if (newLine != "\n")
            throw new Exception("Invalid Code Format");
        
        return base.VisitStatement(context);
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
        var declaratorList = context.declaratorlist().GetText();
        string[] variables = declaratorList.Split(',');
        var count = variables.Length;
        var varDatatype = context.DATATYPE().GetText();

        if (declaratorList == "")
            throw new Exception("Invalid Code Format");
        
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

                    if (!isNum && !isFloat)
                    {
                        if (value.Length == 3)
                        {
                            if (value.EndsWith('\'') && value.StartsWith('\''))
                                value = value[1..^1];
                            else
                                throw new Exception($"Variable {value} format is invalid");
                        }
                        else
                        {
                            if (value == "\"TRUE\"" || value == "\"FALSE\"")
                                value = value[1..^1];
                            else
                                throw new Exception($"Variable {value} format is invalid");
                        }
                    }

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
                            else if (varDatatype == "FLOAT" && isFloat)
                            {
                                HasSameType(varName);
                                FloatVar[varName] = floatValue;
                            }
                            else if (varDatatype == "BOOL" && (value == "TRUE" || value == "FALSE"))
                            {
                                HasSameType(varName);
                                BoolVar[varName] = value;
                            }
                            else
                            {
                                Console.WriteLine($"Variable {varName} expected to be {varDatatype}");
                            //throw new Exception($"Invalid assignment for variable {varName}: expected to be {varDatatype}");
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

        if(leftType == "CODE_Interpreter.GrammarParser+ConstantExpressionContext" && (leftValType is int || leftValType is float))
            throw new Exception($"Invalid operands for concatenation");
        if(rightType == "CODE_Interpreter.GrammarParser+ConstantExpressionContext" && (rightValType is int || rightValType is float)) 
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