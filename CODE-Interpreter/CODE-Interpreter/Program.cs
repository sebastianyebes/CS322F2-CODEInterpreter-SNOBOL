using System.Text;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using CODE_Interpreter;

var path = Path.Combine(Directory.GetCurrentDirectory(), "../../..");
Directory.SetCurrentDirectory(Path.GetFullPath(path));
var contents = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "testFolder/CODEtest.ss"));

bool hasBegin = false;
bool hasEnd = false;
string[] program = contents.Split("\n");

foreach (string line in program)
{
    if (line.Length <= 0 || line[0].Equals('#'))
    {
        continue;
    }
            
    if (!line[0].Equals('#') && hasBegin && hasEnd)
    {
        throw new Exception("Error: Variable is out of scope.");
    }
            
    if ( line.Length >=10 && line.Substring(0, 10).Equals("BEGIN CODE"))
    {
                
        if (hasBegin)
        {
            throw new Exception("Error: Multiple BEGIN CODE exists.");
        }
        hasBegin = true;
                
    }else if (line.Length >=8 && hasBegin && line.Substring(0, 8).Equals("END CODE"))
    {

        if (hasEnd)
        {
            throw new Exception("Error: Multiple END CODE exists.");
        }

        hasEnd = true;
    }

}

if (hasBegin == false)
{
    throw new Exception("Error: No BEGIN CODE exists.");
}else if (hasBegin && hasEnd == false )
{
    throw new Exception("Error: No END CODE exists.");
}
/*
var program = contents.Trim();
var sample = program.Split("\r\n",StringSplitOptions.RemoveEmptyEntries);
*/

/*
foreach (String s in sample)
{
    if (s.StartsWith("BEGIN CODE"))
        break;
    if (!s.StartsWith("#"))
        throw new Exception("Error Code Format");
}
*/
/*
bool hasBegin = false;
bool hasEnd = false;

if (sample.Contains("BEGIN CODE") && sample.Contains("END CODE"))
{
    for (int i = 0; i < sample.Length; i++)
    {
        if (sample[i].StartsWith("BEGIN CODE"))
            hasBegin = true;

        if (!sample[i].StartsWith("#") && !hasBegin && sample[i]!="")
            throw new Exception("Error Code Format");

    
        if (hasEnd && hasBegin && !sample[i].StartsWith("#") && !sample[i].StartsWith("END CODE"))
        {
            Console.WriteLine(sample[i]);
            throw new Exception("Error Code Format2");
        }
        else if (sample[i].StartsWith("END CODE"))
        {
            hasEnd = true;
        }
    }
}
else
{
    throw new Exception("Error Code Format3");
}
*/

var inputStream = new AntlrInputStream(contents);
var grammarLexer = new GrammarLexer(inputStream);
var commonTokenStream = new CommonTokenStream(grammarLexer);
var grammarParser = new GrammarParser(commonTokenStream);

var grammarContext = grammarParser.program();
var visitor = new Visitor();

visitor.Visit(grammarContext);

// foreach(KeyValuePair<string, object?> kvp in visitor.CharVar)
// {
//     Console.Write("CharVariable name: " + kvp.Key + ", Variable value: " + kvp.Value + "\n");
// }
// foreach(KeyValuePair<string, object?> kvp in visitor.IntVar)
// {
//     Console.Write("IntVariable name: " + kvp.Key + ", Variable value: " + kvp.Value + "\n");
// }
// foreach(KeyValuePair<string, object?> kvp in visitor.FloatVar)
// {
//     Console.Write("FlatVariable name: " + kvp.Key + ", Variable value: " + kvp.Value + "\n");
// }
// foreach(KeyValuePair<string, object?> kvp in visitor.BoolVar)
// {
//     Console.Write("BoolVariable name: " + kvp.Key + ", Variable value: " + kvp.Value + "\n");
// }
