using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Serilog;
using Serilog.Events;

namespace PdfMerger;

public class Arguments
{
    public static readonly string DebugSwitch = "--debug";

    private readonly CommandOption _debugSwitch;
    private readonly CommandOption _logLevel;
    private readonly CommandOption _logFile;
    private readonly CommandOption _input;
    private readonly CommandOption _output;
    public LogEventLevel LogLevel
    {
        get
        {
            if (Enum.TryParse<LogEventLevel>(_logLevel.Value(), true, out var logLevel))
                return logLevel;

            return LogEventLevel.Information;
        }
    }

    public string? LogFileName => _logFile.Value();

    public string Input => _input.Value()!;
    public string? Output => _output.Value();


    public Arguments(CommandLineApplication cla)
    {
        if (cla == null)
            throw new ArgumentNullException(nameof(cla));

        cla.HelpOption("-?|-h|--help");

        _debugSwitch = cla.Option(DebugSwitch, "Optional. Debug mode. Application will wait with start until a debugger is attached.", CommandOptionType.NoValue);

        _logFile = cla.Option("--logfile", "File to write the log to.", CommandOptionType.SingleValue);
        _logLevel = cla.Option("--loglevel", "Defines the log level. Possible values: verbose, debug, information (default), warning, error, fatal", CommandOptionType.SingleValue);

        _input = cla.Option("-i|--in", "Directory that contains Pdf Files that should be merged.", CommandOptionType.SingleValue);
        _output = cla.Option("-o|--out", "Filepath where the merged Pdf should be written to.", CommandOptionType.SingleValue);
    }

    public bool ValidateArguments()
    {
        if (!_input.HasValue())
        {
            Console.WriteLine("At least one '--in' directory must be specified.");
            return false;
        }

        if (_input.HasValue() && !_output.HasValue())
        {
            Console.WriteLine("If an 'in' directory is specified, an 'out' file must also be provided.");
            return false;
        }

        if (!Directory.Exists(_input.Value()))
        {
            Console.WriteLine($"The directory given at '--in' {_input.Value()} is not existing");
            return false;
        }


        return true;
    }

    public void LogConfiguration(ILogger? logger)
    {
        if (logger == null)
            return;

        logger.Information("Config - Debug mode: {DebugMode}", _debugSwitch.Value() ?? "no (default)");
        logger.Information("Config - Log file: {LogFileName}", _logFile.Value() ?? "none (default)");
        logger.Information("Config - Log level: {LogLevel}", _logLevel.Value() ?? "information (default)");
        logger.Information("Config - Input(s): {Input}", _input.HasValue() ? string.Join(", ", _input.Values) : "n.a.");
        logger.Information("Config - Output(s): {Output}", _output.HasValue() ? string.Join(", ", _output.Values) : "n.a.");
    }
}
