using Serilog;

namespace eCommerce.SharedLibrary.Logs;

public static class LogExceptions
{
    public static void LogException(Exception exception)
    {
        LogToFile(exception.Message);
        LogToConsole(exception.Message);
        LogToDebugger(exception.Message);
    }

    public static void LogToFile(string exceptionMessage) => Log.Information(exceptionMessage);
    public static void LogToConsole(string exceptionMessage) => Log.Warning(exceptionMessage);
    public static void LogToDebugger(string exceptionMessage) => Log.Debug(exceptionMessage);
} 