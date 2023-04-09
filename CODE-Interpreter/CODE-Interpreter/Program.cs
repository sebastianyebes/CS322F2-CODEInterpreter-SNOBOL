using Antlr4.Runtime;
using CODE_Interpreter;

var path = Path.Combine(Directory.GetCurrentDirectory(), "../../..");
Directory.SetCurrentDirectory(Path.GetFullPath(path));
var contents = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "testFolder/CODEtest.ss"));

Console.WriteLine(contents);

var inputStream = new AntlrInputStream(contents);
var grammarLexer = new GrammarLexer(inputStream);
var commonTokenStream = new CommonTokenStream(grammarLexer);
var grammarParser = new GrammarParser(commonTokenStream);

var grammarContext = grammarParser.program();
var visitor = new Visitor();

visitor.Visit(grammarContext);

foreach(KeyValuePair<string, object?> kvp in visitor.Variables)
{
 Console.Write("Variable name: " + kvp.Key + ", Variable value: " + kvp.Value + "\n");
}