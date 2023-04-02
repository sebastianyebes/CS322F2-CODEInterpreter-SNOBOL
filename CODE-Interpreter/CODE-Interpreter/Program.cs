using Antlr4.Runtime;
using CODE_Interpreter;

var baseDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
var fileName = $@"{baseDirectory}\testFolder\CODEtest.ss";

var read = File.ReadAllText(fileName);

var inputStream = new AntlrInputStream(read);
var grammarLexer = new GrammarLexer(inputStream);
CommonTokenStream commonTokenStream = new CommonTokenStream(grammarLexer);
var grammarParser = new GrammarParser(commonTokenStream);
//grammarParser.AddErrorListener();
var grammarContext = grammarParser.program();
var visitor = new Visitor();

visitor.Visit(grammarContext);