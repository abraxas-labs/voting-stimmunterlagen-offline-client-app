// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Serilog;
using Serilog.Events;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;

namespace VotingCardGenerator;

public class Arguments
{
    public static readonly string DebugSwitch = "--debug";

    private readonly CommandOption _debugSwitch;
    private readonly CommandOption _logLevel;
    private readonly CommandOption _logFile;
    private readonly CommandOption _templateFile;
    private readonly CommandOption _streamData;
    private readonly CommandOption _dataFile;
    private readonly CommandOption _streamPdf;
    private readonly CommandOption _pdfFile;
    private readonly CommandOption _produceHtml;

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
    public string TemplateFile => _templateFile.Value()!;
    public IStream TemplateStream => new FileAdapter().OpenRead(TemplateFile);

    public bool StreamInput => _streamData.HasValue();
    public bool ProduceHtmlOnly => _produceHtml.HasValue();

    public IStream DataStream => (StreamInput)
        ? new StreamAdapter(Console.OpenStandardInput(0x10000))
        : new FileAdapter().OpenRead(_dataFile.Value());

    public bool StreamPdf => _streamPdf.HasValue();
    public IStream PdfStream => (StreamPdf)
        ? new StreamAdapter(Console.OpenStandardOutput(0x10000))
        : new FileAdapter().Open(_pdfFile.Value(), FileMode.Create, FileAccess.Write);

    public Arguments(CommandLineApplication cla)
    {
        if (cla == null)
            throw new ArgumentNullException(nameof(cla));

        cla.HelpOption("-?|-h|--help");

        _debugSwitch = cla.Option(DebugSwitch, "Optional. Debug mode. Application will wait with start until a debugger is attached.", CommandOptionType.NoValue);

        _logFile = cla.Option("-l|--logfile", "Optional. File to write the log to.", CommandOptionType.SingleValue);
        _logLevel = cla.Option("--loglevel", "Optional. Defines the log level. Possible values: verbose, debug, information (default), warning, error, fatal", CommandOptionType.SingleValue);

        _templateFile = cla.Option("-t|--template", "Required. The razor template file to generate the output from.", CommandOptionType.SingleValue);
        _streamData = cla.Option("--instream", "Read json data input from stdin. Use this or '--in filename' for specifying an input file.", CommandOptionType.NoValue);
        _dataFile = cla.Option("-i|--in", "The json data file to use for generating the output.", CommandOptionType.SingleValue);
        _streamPdf = cla.Option("--outstream", "Write pdf or html output to stdout. Use this or '--out filename' for specifying an output file.", CommandOptionType.NoValue);
        _pdfFile = cla.Option("-o|--out", "The file to write the generated pdf or html to.", CommandOptionType.SingleValue);
        _produceHtml = cla.Option("-h|--html", "Skips the PDF generation and writes the intermediate html.", CommandOptionType.NoValue);
    }

    public bool ValidateArguments()
    {
        var file = new FileAdapter();

        // Check Template file
        if (!_templateFile.HasValue() || string.IsNullOrWhiteSpace(_templateFile.Value()))
        {
            Log.Logger.Error("The template file is required, but not provided.");
            return false;
        }

        if (!file.Exists(_templateFile.Value()))
        {
            Log.Logger.Error("The specified template file {TemplateFileName} does not exist.", _templateFile.Value());
            return false;
        }

        // Check Data input
        if (_streamData.HasValue())
        {
            if (_dataFile.HasValue())
            {
                Log.Logger.Error("You can either specify --instream OR --in, but not both.");
                return false;
            }
        }
        else
        {
            if (!_dataFile.HasValue() || string.IsNullOrWhiteSpace(_dataFile.Value()))
            {
                Log.Logger.Error("The data input --instream or --in is required, but not provided.");
                return false;
            }

            if (!file.Exists(_dataFile.Value()))
            {
                Log.Logger.Error("The specified data file {DataFileName} does not exist.", _dataFile.Value());
                return false;
            }

            // there is no easy way to check for read permissions 
            try
            {
                using var stream = file.OpenRead(_dataFile.Value());
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "The specified data file {DataFileName} can not be read.", _dataFile.Value());
                return false;
            }
        }

        // Check pdf output
        if (_streamPdf.HasValue())
        {
            if (_pdfFile.HasValue())
            {
                Log.Logger.Error("You can either specify --outstream OR --out, but not both.");
                return false;
            }
        }
        else
        {
            if (!_pdfFile.HasValue() || string.IsNullOrWhiteSpace(_pdfFile.Value()))
            {
                Log.Logger.Error("The parameter --outstream or --out filename is required, but not provided.");
                return false;
            }

            // there is no easy way to check for write permissions 
            try
            {
                using var stream = file.Open(_pdfFile.Value(), FileMode.Create, FileAccess.Write);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "The specified output file {PdfFileName} can not be written to.", _pdfFile.Value());
                return false;
            }
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
        logger.Information("Config - Template: {TemplateFileName}", _templateFile.Value());
        logger.Information("Config - Stream input data: {StreamInputData}", _streamData.HasValue());
        logger.Information("Config - Input file name: {InputFileName}", _dataFile.Value() ?? "none (default)");
        logger.Information("Config - Stream output data: {StreamOutputData}", _streamPdf.HasValue());
        logger.Information("Config - Output file name: {OutputFileName}", _pdfFile.Value() ?? "none (default)");
        logger.Information("Config - Produce html: {ProduceHtml}", _produceHtml.HasValue() ? "yes" : "no (default)");
    }
}
