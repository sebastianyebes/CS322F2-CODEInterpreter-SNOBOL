using Antlr4.Runtime;
using CODE_Interpreter;

var fileName = "..\\..\\..\\testFolder\\CODEtest.ss";

var read = File.ReadAllText(fileName);

var inputStream = new AntlrInputStream(read);
var grammarLexer = new GrammarLexer(inputStream);
CommonTokenStream commonTokenStream = new CommonTokenStream(grammarLexer);
var grammarParser = new GrammarParser(commonTokenStream);
//grammarParser.AddErrorListener();
var grammarContext = grammarParser.program();
var visitor = new Visitor();        

visitor.Visit(grammarContext);