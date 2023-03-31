using Serilog.Events;
using Serilog;
using System;
using Serilog.Formatting.Json;
using Serilog.Formatting;

namespace Voting.Stimmunterlagen.OfflineClient.Logging;

public static class LoggerInitializer
{
    private static readonly ITextFormatter DefaultWriteFormatter = new JsonFormatter();

    public static void Initialize(LogEventLevel minimumLevel, bool writeToConsole, string? logFileName)
    {
        var config = new LoggerConfiguration()
            .MinimumLevel.Is(minimumLevel)
            .Enrich.FromLogContext()
            .Enrich.WithAssemblyName()
            .Enrich.WithAssemblyVersion()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithThreadId()
            .Enrich.WithUserName()
            .Enrich.WithMemoryUsage();

        if (writeToConsole)
        {
            config = config.WriteTo.Console(DefaultWriteFormatter);
        }

        string? logFileError = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(logFileName))
            {
                config = config.WriteTo.File(path: logFileName, formatter: DefaultWriteFormatter);
            }
        }
        catch (Exception ex)
        {
            logFileError = ex.Message;
        }

        Log.Logger = config.CreateLogger();

        if (!string.IsNullOrWhiteSpace(logFileError))
        {
            Log.Logger.Error("There was an error during logger file configuration: {Error}", logFileError);
        }
    }
}
