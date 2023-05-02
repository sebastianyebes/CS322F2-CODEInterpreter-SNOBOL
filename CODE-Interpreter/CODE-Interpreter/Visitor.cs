using Microsoft.VisualBasic.CompilerServices;
using System.Linq.Expressions;

namespace CODE_Interpreter;
public class Visitor : GrammarBaseVisitor<object?>
{
    public Dictionary<string, object?> DisplayFunctions { get; } = new();
    public Dictionary<string, object?> ScanFunctions { get; } = new();
    public Dictionary<string, object?> CharVar { get; } = new();
    public Dictionary<string, object?> IntVar { get; } = new();
    public Dictionary<string, object?> FloatVar { get; } = new();
    public Dictionary<string, object?> BoolVar { get; } = new();
    
    public Visitor()
    {
        DisplayFunctions["DISPLAY"] = new Func<object?, object?>(Display);
        ScanFunctions["SCAN"] = new Func<object?[], object?>(Scan);
    }

    private object? Display(object? args)
    {
        Console.Write(args + "\n");

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
    
    private object? Scan(object?[] args)
    {
        Console.Write("SCAN: ");
        var input = Console.ReadLine() ?? throw new InvalidOperationException();
        var userVariables = input.Replace(" ", "").Split(',');
        var countVariables = 0;
        
        foreach (var arg in args)
        {
            if (CharVar.ContainsKey(arg!.ToString()!))
            {
                var userInput = Convert.ToChar(userVariables[countVariables]);
                CharVar[arg.ToString()!] = userInput;
            }else if (IntVar.ContainsKey(arg!.ToString()!))
            {
                var userInput = Convert.ToInt32(userVariables[countVariables]);
                IntVar[arg.ToString()!] = userInput;
            }else if (FloatVar.ContainsKey(arg!.ToString()!))
            {
                var userInput = float.Parse(userVariables[countVariables]);
                FloatVar[arg.ToString()!] = userInput;
            }else if (BoolVar.ContainsKey(arg!.ToString()!))
            {
                var userInput = userVariables[countVariables];
                if(userInput is "TRUE" or "FALSE")
                    BoolVar[arg.ToString()!] = userInput;
                else
                    throw new Exception("Error: Expected a boolean value.");
            }
            else
                throw new Exception("Error: Identifier is not declared.");
            
            countVariables++;
        }  
        return null;
    }
  
   
    
    public override object? VisitFunctionCall(GrammarParser.FunctionCallContext context)
    {
        /*
        var name = context.FUNCTIONNAME().GetText();
        var args = context.displayvalue().Select(Visit).ToArray();

        if (!Functions.ContainsKey(name))
            throw new Exception($"Function {name} is not defined");
        if (Functions[name] is not Func<object?[], object?> func)
            throw new Exception($"Function {name} is not a function");
        
        return func(args);
        */
        var funcName = context.DISPLAYNAME().GetText();
        var args = Visit(context.displayvalue());
        
        var argType = context.displayvalue().GetType().ToString();
        if (argType == "CODE_Interpreter.GrammarParser+ConstantExpressionContext" &&
            (args is int || args is float))
        {
            throw new Exception($"Invalid operands for concatenation");
        }

        if (!DisplayFunctions.ContainsKey(funcName))
        {
            throw new Exception($"Function {funcName} is not defined");
        }

        if (DisplayFunctions[funcName] is not Func<object?, object?> func)
        {
            throw new Exception($"{funcName} is not a function");
        }
        
        return func(args);
    }

    public override object? VisitScanCall(GrammarParser.ScanCallContext context)
    {
        var funcName = context.SCANNAME().GetText();
        var args = context.scanvalue().Select(Visit).ToArray();
        
        if(args.Length == 0)
            throw new Exception($"Scan has no input");
        
        var argType = context.scanvalue(0).GetType().ToString();
        if (argType == "CODE_Interpreter.GrammarParser+ConstantExpressionContext" &&
            (args[0] is int || args[0] is float))
        {
            throw new Exception($"Invalid operands for concatenation");
        }

        if (!ScanFunctions.ContainsKey(funcName))
        {
            throw new Exception($"Function {funcName} is not defined");
        }

        if (ScanFunctions[funcName] is not Func<object?[], object?> func)
        {
            throw new Exception($"{funcName} is not a function");
        }
        
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

                    if (!isNum || !isFloat)
                    {
                        if (value.Length == 3)
                        {
                            if (value.EndsWith('\'') && value.StartsWith('\''))
                            {
                                value = value[1..^1];
                            }
                            else
                            {
                                throw new Exception($"Value {value} is invalid");
                            }
                        }
                        else
                        {
                            if (value == "\"TRUE\"" || value == "\"FALSE\"")
                            {
                                value = value[1..^1];
                            }
                            else
                            {
                                throw new Exception($"Value {value} is invalid");
                            }
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
                            else if (varDatatype == "BOOL")
                            {
                                HasSameType(varName);
                                BoolVar[varName] = value;
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

        if (context.parent.GetChild(0).ToString() == "SCAN")
        {
            return varName;
        }

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

    public override object? VisitScanvariablenameExpression(GrammarParser.ScanvariablenameExpressionContext context)
    {
        var varName = context.VARIABLENAME().GetText();
        
        
        if (context.parent.GetChild(0).ToString() == "SCAN")
        {
            return varName;
        }
        
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
        
        
        if (context.parent.GetChild(0).ToString() == "SCAN")
        {
            return varName;
        }
        
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
            //throw new Exception($"Invalid operands for concatenation: {(string.IsNullOrEmpty(left) ? "left" : "right")} operand is null or empty.");
            
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
            "-" => Subtract(left, right),
            _ => throw new NotImplementedException()
        };
    }

    public override object? VisitAddExpression(GrammarParser.AddExpressionContext context)
    {
        var left = Visit(context.displayvalue(0));
        var right = Visit(context.displayvalue(1));

        var op = context.addOp().GetText();

        return op switch
        {
            "+" => Add(left, right),
            "-" => Subtract(left, right),
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
    
    private object? Subtract(object? left, object? right)
    {   
        if (left is int l && right is int r)
        {
            return l - r;
        }

        if (left is float lf && right is float rf)
        {
            return lf - rf;
        }

        throw new NotImplementedException($"Cannot add values of types {left?.GetType()} and {right?.GetType()}");
    }

    public override object? VisitMultiplicativeExpression(GrammarParser.MultiplicativeExpressionContext context)
    {
        var left = Visit(context.value(0));
        var right = Visit(context.value(1));

        var op = context.multOp().GetText();

        return op switch
        {
            "*" => Multiply(left, right),
            "/" => Divide(left, right),
            _ => throw new NotImplementedException()
        };
    }

    public override object? VisitMultiExpression(GrammarParser.MultiExpressionContext context)
    {
        var left = Visit(context.displayvalue(0));
        var right = Visit(context.displayvalue(1));

        var op = context.multOp().GetText();

        return op switch
        {
            "*" => Multiply(left, right),
            "/" => Divide(left, right),
            _ => throw new NotImplementedException()
        };
    }

    private object? Multiply(object? left, object? right)
    {   
        if (left is int l && right is int r)
        {
            return l * r;
        }

        if (left is float lf && right is float rf)
        {
            return lf * rf;
        }

        throw new NotImplementedException($"Cannot add values of types {left?.GetType()} and {right?.GetType()}");
    }
    
    private object? Divide(object? left, object? right)
    {   
        if (left is int l && right is int r)
        {
            return l / r;
        }

        if (left is float lf && right is float rf)
        {
            return lf / rf;
        }

        throw new NotImplementedException($"Cannot add values of types {left?.GetType()} and {right?.GetType()}");
    }

    public override object? VisitLogicalOpExpression(GrammarParser.LogicalOpExpressionContext context)
    {
        var left = Visit(context.value(0));
        var right = Visit(context.value(1));

        var op = context.logicalOp().GetText();

        return op switch
        {
            "AND" => And(left, right),
            "OR" => Or(left, right),
            _ => throw new NotImplementedException()
        };
    }

    public override object? VisitLogicExpression(GrammarParser.LogicExpressionContext context)
    {
        var left = Visit(context.displayvalue(0));
        var right = Visit(context.displayvalue(1));

        var op = context.logicalOp().GetText();

        return op switch
        {
            "AND" => And(left, right),
            "OR" => Or(left, right),
            _ => throw new NotImplementedException()
        };
    }

    private string And(object? left, object? right)
    {   
        if(left is string l && right is string r)
        {
            if (left is "TRUE" && right is "TRUE")
            {
                return "TRUE";
            }
            else
            {
                return "FALSE";
            }
        }
        
        throw new NotImplementedException($"Cannot compare values of types {left?.GetType()} and {right?.GetType()}");
    }
    
    private string Or(object? left, object? right)
    {   
        if(left is string l && right is string r)
        {
            if (left is "TRUE" || right is "TRUE")
            {
                return "TRUE";
            }
            else
            {
                return "FALSE";
            }
        }
        
        throw new NotImplementedException($"Cannot compare values of types {left?.GetType()} and {right?.GetType()}");
    }

    public override object? VisitComparisonExpression(GrammarParser.ComparisonExpressionContext context)
    {
        var left = Visit(context.value(0));
        var right = Visit(context.value(1));

        var op = context.compareOp().GetText();

        return op switch
        {
            "==" => IsEquals(left, right),
            "<>" => NotEquals(left, right),
            ">" => GreaterThan(left, right),
            "<" => LessThan(left, right),
            ">=" => GreaterThanOrEqual(left, right),
            "<=" => LessThanOrEqual(left, right),
            _ => throw new NotImplementedException()
        };
    }

    public override object? VisitCompExpression(GrammarParser.CompExpressionContext context)
    {
        var left = Visit(context.displayvalue(0));
        var right = Visit(context.displayvalue(1));

        var op = context.compareOp().GetText();

        return op switch
        {
            "==" => IsEquals(left, right),
            "<>" => NotEquals(left, right),
            ">" => GreaterThan(left, right),
            "<" => LessThan(left, right),
            ">=" => GreaterThanOrEqual(left, right),
            "<=" => LessThanOrEqual(left, right),
            _ => throw new NotImplementedException()
        };
    }

    private string IsEquals(object? left, object? right)
    {   
        if (left is int l && right is int r)
        {
            if (l == r)
                return "TRUE";

            return "FALSE";
        }
        
        if (left is float lf && right is float rf)
        {
            if (lf == rf)
                return "TRUE";

            return "FALSE";
            
        }
        
        throw new NotImplementedException($"Cannot compare values of types {left?.GetType()} and {right?.GetType()}");
    }
    
    private string LessThan(object? left, object? right)
    {   
        if (left is int l && right is int r)
        {
            if (l < r)
                return "TRUE";

            return "FALSE";
        }
        
        if (left is float lf && right is float rf)
        {
            if (lf < rf)
                return "TRUE";

            return "FALSE";
            
        }
        
        throw new NotImplementedException($"Cannot compare values of types {left?.GetType()} and {right?.GetType()}");
    }
    
    private string GreaterThan(object? left, object? right)
    {   
        if (left is int l && right is int r)
        {
            if (l > r)
                return "TRUE";

            return "FALSE";
        }
        
        if (left is float lf && right is float rf)
        {
            if (lf > rf)
                return "TRUE";

            return "FALSE";
            
        }
        
        throw new NotImplementedException($"Cannot compare values of types {left?.GetType()} and {right?.GetType()}");
    }
    
    private string NotEquals(object? left, object? right)
    {   
        if (left is int l && right is int r)
        {
            if (l != r)
                return "TRUE";

            return "FALSE";
        }
        
        if (left is float lf && right is float rf)
        {
            if (lf != rf)
                return "TRUE";

            return "FALSE";
            
        }
        
        throw new NotImplementedException($"Cannot compare values of types {left?.GetType()} and {right?.GetType()}");
    }
    
    private string LessThanOrEqual(object? left, object? right)
    {   
        if (left is int l && right is int r)
        {
            if (l <= r)
                return "TRUE";

            return "FALSE";
        }
        
        if (left is float lf && right is float rf)
        {
            if (lf <= rf)
                return "TRUE";

            return "FALSE";
            
        }
        
        throw new NotImplementedException($"Cannot compare values of types {left?.GetType()} and {right?.GetType()}");
    }
    
    private string GreaterThanOrEqual(object? left, object? right)
    {   
        if (left is int l && right is int r)
        {
            if (l >= r)
                return "TRUE";

            return "FALSE";
        }
        
        if (left is float lf && right is float rf)
        {
            if (lf >= rf)
                return "TRUE";

            return "FALSE";
            
        }
        
        throw new NotImplementedException($"Cannot compare values of types {left?.GetType()} and {right?.GetType()}");
    }

    public override object? VisitNotExpression(GrammarParser.NotExpressionContext context)
    {
        var val = Visit(context.value());
        
        if(val is string b && (b == "TRUE" || b == "FALSE"))
            return Not(b);
        
        throw new NotImplementedException($"Bool value expected instead of {val?.GetType()}");
    }

    public override object? VisitNoExpression(GrammarParser.NoExpressionContext context)
    {
        var val = Visit(context.displayvalue());
        
        if(val is string b && (b == "TRUE" || b == "FALSE"))
            return Not(b);
        
        throw new NotImplementedException($"Bool value expected instead of {val?.GetType()}");
    }

    public string Not(object? val)
    {
        if (val is string b)
        {
            if (b == "TRUE")
                return "FALSE";
            else
            {
                return "TRUE";
            }
        }
        throw new NotImplementedException($"Bool value expected instead of {val?.GetType()}");
    }

    public override object? VisitParenthesizedExpression(GrammarParser.ParenthesizedExpressionContext context)
    {
        return Visit(context.value());
    }

    public override object? VisitParenthesisExpression(GrammarParser.ParenthesisExpressionContext context)
    {
        return Visit(context.displayvalue());
    }

    public override object? VisitIfCond(GrammarParser.IfCondContext context)
    {
        var state = context.value().Select(Visit).ToArray();

        for (int i = 0; i < state.Length; i++)
        {
            if (state[i] is string b && b == "TRUE")
            {
                return Visit(context.ifBlock(i));
            }
        }

        if (context.GetText().Contains("ELSE"))
        {
            return Visit(context.ifBlock(state.Length));
        }
        

        return null;
    }

    public override object? VisitIfBlock(GrammarParser.IfBlockContext context)
    {
        return context.executes().Select(Visit).ToArray();
    }
}