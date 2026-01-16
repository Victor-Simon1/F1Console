using System.Text;

public static class RacingLogger
{
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Exception = 4,
    }

    // Master switch: disable to mute all logs at runtime.
    public static bool Enabled = true;

    // Minimum level to output.
    public static LogLevel MinimumLevel = LogLevel.Debug;

    // Global prefix for all logs
    public static string GlobalPrefix = "[RACING]";

    // Enable/disable timestamp in logs
    public static bool IncludeTimestamp = false;

    #region Core Logging Methods

    /// <summary>
    /// Log a message with a given level. This is the main logging method with full formatting support.
    /// </summary>
    /// <param name="level">Severity of the log.</param>
    /// <param name="message">Text to print. Provide a preformatted message.</param>
    public static void Log(LogLevel level, string message)
    {
        if (!Enabled) return;                 // Fast exit if logging is globally disabled
        if (level < MinimumLevel) return;     // Filter out messages below the configured level
        
        string formattedMessage = FormatMessage(level, message);

        ConsoleColor originalColor = Console.ForegroundColor;
        
        try
        {
            switch (level)
            {
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                case LogLevel.Exception:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.WriteLine(formattedMessage);
        }
        finally
        {
            Console.ForegroundColor = originalColor;
        }
    }

    #endregion

    #region Convenience Overloads

    /// <summary>
    /// Log a debug message (for detailed technical information).
    /// </summary>
    public static void Debug(string message)
    {
        Log(LogLevel.Debug, message);
    }

    /// <summary>
    /// Log an info message (for general information).
    /// </summary>
    public static void Info(string message)
    {
        Log(LogLevel.Info, message);
    }

    /// <summary>
    /// Log a warning message (for non-critical issues).
    /// </summary>
    public static void Warning(string message)
    {
        Log(LogLevel.Warning, message);
    }

    /// <summary>
    /// Log an error message (for errors that don't stop execution).
    /// </summary>
    public static void Error(string message)
    {
        Log(LogLevel.Error, message);
    }

    /// <summary>
    /// Log an exception message.
    /// </summary>
    public static void Exception(string message)
    {
        Log(LogLevel.Exception, message);
    }

    /// <summary>
    /// Log an exception with its details (message and stack trace).
    /// </summary>
    public static void Exception(Exception ex, string? additionalContext = null)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(additionalContext))
        {
            sb.Append($"{additionalContext}: ");
        }
        sb.Append($"Exception: {ex.GetType().Name}: {ex.Message}");
        if (!string.IsNullOrEmpty(ex.StackTrace))
        {
            sb.Append($"\nStack Trace: {ex.StackTrace}");
        }
        Log(LogLevel.Exception, sb.ToString());
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Format a log message with prefix, timestamp, and level information.
    /// </summary>
    private static string FormatMessage(LogLevel level, string message)
    {
        var sb = new StringBuilder();

        // Add global prefix
        if (!string.IsNullOrEmpty(GlobalPrefix))
        {
            sb.Append(GlobalPrefix);
            sb.Append(" ");
        }

        // Add timestamp if enabled
        if (IncludeTimestamp)
        {
            sb.Append($"[{DateTime.Now:HH:mm:ss.fff}] ");
        }

        // Add level indicator
        sb.Append($"[{level.ToString().ToUpper()}] ");

        // Add the actual message
        sb.Append(message);

        return sb.ToString();
    }

    #endregion
}
