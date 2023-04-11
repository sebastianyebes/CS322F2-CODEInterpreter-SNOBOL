using System.Text;
using Antlr4.Runtime;
using CODE_Interpreter;

var path = Path.Combine(Directory.GetCurrentDirectory(), "../../..");
Directory.SetCurrentDirectory(Path.GetFullPath(path));
var contents = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "testFolder/CODEtest.ss"));

var program = contents.Trim();
if (!program.StartsWith("BEGIN CODE") || !program.EndsWith("END CODE"))
{
    throw new Exception("Must start with 'BEGIN CODE' and end with 'END CODE'");
}

var inputStream = new AntlrInputStream(contents);
var grammarLexer = new GrammarLexer(inputStream);
var commonTokenStream = new CommonTokenStream(grammarLexer);
var grammarParser = new GrammarParser(commonTokenStream);

var grammarContext = grammarParser.program();
var visitor = new Visitor();

visitor.Visit(grammarContext);

foreach(KeyValuePair<string, object?> kvp in visitor.CharVar)
{
    Console.Write("CharVariable name: " + kvp.Key + ", Variable value: " + kvp.Value + "\n");
}
foreach(KeyValuePair<string, object?> kvp in visitor.IntVar)
{
    Console.Write("IntVariable name: " + kvp.Key + ", Variable value: " + kvp.Value + "\n");
}
foreach(KeyValuePair<string, object?> kvp in visitor.FloatVar)
{
    Console.Write("FlatVariable name: " + kvp.Key + ", Variable value: " + kvp.Value + "\n");
}
foreach(KeyValuePair<string, object?> kvp in visitor.BoolVar)
{
    Console.Write("BoolVariable name: " + kvp.Key + ", Variable value: " + kvp.Value + "\n");
}
