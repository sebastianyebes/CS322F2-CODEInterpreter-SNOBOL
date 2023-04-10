using Antlr4.Runtime;
using CODE_Interpreter;

var path = Path.Combine(Directory.GetCurrentDirectory(), "../../..");
Directory.SetCurrentDirectory(Path.GetFullPath(path));
var contents = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "testFolder/CODEtest.ss"));

if (contents.Substring(0, 10) != "BEGIN CODE" || contents.Substring(contents.Length - 8, 8) != "END CODE")
{
    throw new Exception("No BEGIN CODE or END CODE");
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
