using System;
using System.IO;
using System.Windows.Forms;

public static class Logger
{
    private static readonly object _lock = new object();
    private static string logDirectory = Path.Combine(Application.StartupPath, "Logs");

    private static string GetDailyLogFile()
    {
        if (!Directory.Exists(logDirectory))
            Directory.CreateDirectory(logDirectory);

        return Path.Combine(logDirectory, $"Log_{DateTime.Now:yyyy-MM-dd}.txt");
    }

    public static void Write(string message)
    {
        lock (_lock)
        {
            string logFile = GetDailyLogFile();
            File.AppendAllText(logFile,$"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
        }
    }
}
