using System;

public static class Logger
{
    private static readonly object _lock = new object();

    public static void Info(string message)
    {
        Log("INFO", ConsoleColor.Black, message);
    }

    public static void Success(string message)
    {
        Log("SUCCESS", ConsoleColor.Green, message);
    }

    public static void Warning(string message)
    {
        Log("WARN", ConsoleColor.DarkYellow, message);
    }

    public static void Error(string message)
    {
        Log("ERROR", ConsoleColor.Red, message);
    }

    public static void Debug(string message)
    {
        Log("DEBUG", ConsoleColor.Cyan, message);
    }

    private static void Log(string level, ConsoleColor color, string message)
    {
        lock (_lock)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"[{DateTime.Now:HH:mm:ss}] ");

            Console.ForegroundColor = color;
            Console.Write($"[{level,-7}] ");

            Console.WriteLine(message);

            Console.ResetColor();
        }
    }
}
