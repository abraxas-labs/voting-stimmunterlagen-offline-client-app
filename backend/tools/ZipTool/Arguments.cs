using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Serilog;
using Serilog.Events;

namespace ZipTool;

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

    public List<string> Input => _input.Values.WhereNotNull().ToList();
    public List<string> Output => _output.Values.WhereNotNull().ToList();

    public bool? Zip { get; private set; } = null;

    public Arguments(CommandLineApplication cla)
    {
        if (cla == null)
            throw new ArgumentNullException(nameof(cla));

        cla.HelpOption("-?|-h|--help");

        _debugSwitch = cla.Option(DebugSwitch, "Optional. Debug mode. Application will wait with start until a debugger is attached.", CommandOptionType.NoValue);

        _logFile = cla.Option("--logfile", "File to write the log to.", CommandOptionType.SingleValue);
        _logLevel = cla.Option("--loglevel", "Defines the log level. Possible values: verbose, debug, information (default), warning, error, fatal", CommandOptionType.SingleValue);

        _input = cla.Option("-i|--in", "File(s) or Directories that should be zipped or unzipped.", CommandOptionType.MultipleValue);
        _output = cla.Option("-o|--out", "File(s) or Direcories where zipped or unzipped output should be written to. Specify exact the same amount of out files as for in files.", CommandOptionType.MultipleValue);
    }

    public bool ValidateArguments()
    {
        if (!_input.HasValue())
        {
            Console.WriteLine("At least one '--in' file or directory must be specified.");
            return false;
        }

        if (_input.HasValue() && !_output.HasValue())
        {
            Console.WriteLine("If an 'in' file is specified, an 'out' file must also be provided.");
            return false;
        }

        if (_input.Values.Count != _output.Values.Count)
        {
            Console.WriteLine("The same amount of files or directories for 'in' and 'out' must be specified.");
            return false;
        }

        var validationFailed = false;
        for (var i = 0; i < _input.Values.Count; i++)
        {
            var input = _input.Values[i];
            var output = _output.Values[i];
            var inputIsDirectory = !string.IsNullOrWhiteSpace(input) && (File.GetAttributes(input) & FileAttributes.Directory) == FileAttributes.Directory;

            if (inputIsDirectory)
            {
                if (!Directory.Exists(input))
                {
                    Console.WriteLine($"The directory specified for '--in {input}' does not exist.");
                    validationFailed = true;
                }
                if (File.Exists(output))
                {
                    Console.WriteLine($"The file specified for '--out {output}' already exists");
                    validationFailed = true;
                }
                if (string.IsNullOrWhiteSpace(output) || !Path.GetExtension(output).Equals(".zip", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine($"Since '--in {input}' is a Directory, --out {output} must be a zip file");
                    validationFailed = true;
                }
                if (Path.GetDirectoryName(output) == Path.GetDirectoryName(input))
                {
                    Console.WriteLine($"The path specified for '--in {input}' can not be the same as, --out {output}");
                    validationFailed = true;
                }
            }
            else if (!File.Exists(input))
            {
                Console.WriteLine($"The file specified for '--in {input}' does not exist.");
                validationFailed = true;
            }
            else if (!Path.GetExtension(input).Equals(".zip", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine($"The path specified for '--in {input}' must be a zip (to unzip it) file or a directory (to zip it).");
                validationFailed = true;
            }
        }

        if (validationFailed)
        {
            return false;
        }

        return true;
    }

    public void LogConfiguration(ILogger logger)
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
