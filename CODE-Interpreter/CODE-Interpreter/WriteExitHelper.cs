namespace CODE_Interpreter;

public class WriteExitHelper
{
    public void WriteLineAndExit(string message)
    {
        Console.WriteLine(message);
        Environment.Exit(0);
    }
}