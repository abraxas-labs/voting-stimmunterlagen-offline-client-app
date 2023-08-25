using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Serilog;
using Serilog.Events;

namespace EchDeliveryJsonConverter;

public class Arguments
{
    private readonly ILogger _logger;

    public static readonly string DebugSwitch = "--debug";

    private readonly CommandOption _debugSwitch;
    private readonly CommandOption _logLevel;
    private readonly CommandOption _logFile;
    private readonly CommandOption _inFile;
    private readonly CommandOption _outFile;
    private readonly CommandOption _streamIn;
    private readonly CommandOption _streamOut;

    public Stream XmlData => (_streamIn.HasValue())
        ? Console.OpenStandardInput(0x10000)
        : File.OpenRead(InFile!);

    public bool StreamOutput => _streamOut.HasValue();
    public Stream JsonData => (StreamOutput)
        ? Console.OpenStandardOutput(0x10000)
        : File.Open(OutFile!, FileMode.Create, FileAccess.Write);

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

    public string? InFile => _inFile.Value();
    public string? OutFile => _outFile.Value();

    public List<string> MultipleInFiles => _inFile.Values.WhereNotNull().ToList();

    public bool HasMultipleInFiles => _inFile.HasValue() && _inFile.Values.Count > 1;

    public Arguments(ILogger logger, CommandLineApplication cla)
    {
        _logger = logger;
        if (cla == null)
            throw new ArgumentNullException(nameof(cla));

        cla.HelpOption("-?|-h|--help");

        _debugSwitch = cla.Option(DebugSwitch, "Optional. Debug mode. Application will wait with start until a debugger is attached.", CommandOptionType.NoValue);

        _logFile = cla.Option("--logfile", "File to write the log to.", CommandOptionType.SingleValue);
        _logLevel = cla.Option("--loglevel", "Defines the log level. Possible values: verbose, debug, information (default), warning, error, fatal", CommandOptionType.SingleValue);
        _streamIn = cla.Option("--instream", "Read XML data input from stdin. Use this or '--in filename' for specifying an input file.", CommandOptionType.NoValue);
        _inFile = cla.Option("-i|--in", "XML files that should be converted to json.", CommandOptionType.MultipleValue);
        _streamOut = cla.Option("--outstream", "Write json output to stdout. Use this or '--out filename' for specifying an output file.", CommandOptionType.NoValue);
        _outFile = cla.Option("-o|--out", "File where converted json output should be written to.", CommandOptionType.SingleValue);

    }

    public bool ValidateArguments()
    {
        // instream or infile required
        if (!_streamIn.HasValue())
        {
            if (!_inFile.HasValue())
            {
                _logger.Error("No file specified for required argument '--in {{InFileName}}'.");
                return false;
            }

            foreach (var fileValue in _inFile.Values)
            {
                if (!File.Exists(fileValue))
                {
                    _logger.Error("The file specified for '--in {InFileName}' does not exist.", _inFile.Value());
                    return false;
                }
            }
        }

        // outstream or outfile required
        if (!_streamOut.HasValue() && !_outFile.HasValue())
        {
            _logger.Error("No file specified for required argument '--out {{OutFileName}}'.");
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
        logger.Information("Config - InFile: {InFile}", _inFile.HasValue() ? _inFile.Value() : "n.a.");
        logger.Information("Config - OutFile: {OutFile}", _outFile.HasValue() ? _outFile.Value() : "n.a.");
    }
}
