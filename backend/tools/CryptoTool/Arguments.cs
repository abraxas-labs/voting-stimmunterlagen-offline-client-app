// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Serilog;
using Serilog.Events;

namespace CryptoTool;

public class Arguments
{
#if DEBUG
    public bool SwissCertsOnly => false;
#else
		public bool SwissCertsOnly => true;
#endif

    public static readonly string DebugSwitch = "--debug";

    private readonly CommandOption _debugSwitch;
    private readonly CommandOption _logLevel;
    private readonly CommandOption _logFile;
    private readonly CommandOption _senderJsonList;
    private readonly CommandOption _receiverJsonList;
    private readonly CommandOption _inFile;
    private readonly CommandOption _outFile;
    private readonly CommandOption _senderCertificateFile;
    private readonly CommandOption _senderCertificateFilePassword;
    private readonly CommandOption _senderCertificateSubject;
    private readonly CommandOption _receiverCertificateFile;

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
    public bool SenderJsonList => _senderJsonList.HasValue();

    public bool ReceiverJsonList => _receiverJsonList.HasValue();

    public List<string> InFiles => _inFile.Values.WhereNotNull().ToList();
    public List<string> OutFiles => _outFile.Values.WhereNotNull().ToList();

    public string? SenderCertificateFile => _senderCertificateFile.Value();
    public string SenderCertificateFilePassword => _senderCertificateFilePassword.Value()!;
    public string? SenderCertificateSubject => _senderCertificateSubject.Value();

    public List<string> ReceiverCertificateFiles => _receiverCertificateFile.Values.WhereNotNull().ToList();

    public bool Encrypt { get; private set; }

    public Arguments(CommandLineApplication cla)
    {
        if (cla == null)
            throw new ArgumentNullException(nameof(cla));

        cla.HelpOption("-?|-h|--help");

        _debugSwitch = cla.Option(DebugSwitch, "Optional. Debug mode. Application will wait with start until a debugger is attached.", CommandOptionType.NoValue);

        _logFile = cla.Option("--logfile", "File to write the log to.", CommandOptionType.SingleValue);
        _logLevel = cla.Option("--loglevel", "Defines the log level. Possible values: verbose, debug, information (default), warning, error, fatal", CommandOptionType.SingleValue);

        _senderJsonList = cla.Option("-sjl|--senderJsonList", "Lists all available sender certificate subjects as json.", CommandOptionType.NoValue);
        _receiverJsonList = cla.Option("-rjl|--receiverJsonList", "Lists all available receiver certificate subjects as json.", CommandOptionType.NoValue);

        _inFile = cla.Option("-i|--in", "File(s) that should be encrypted.", CommandOptionType.MultipleValue);
        _outFile = cla.Option("-o|--out", "File(s) where encrypted output should be written to. Specify exact the same amount of out files as for in files.", CommandOptionType.MultipleValue);

        _receiverCertificateFile = cla.Option("-rf|--receiverCertificateFile", "Load the receivers certificate(s) from this file(s).", CommandOptionType.MultipleValue);

        _senderCertificateSubject = cla.Option("-s|--senderSubject", "Load the senders certificate from the store by this subject name.", CommandOptionType.SingleValue);
        _senderCertificateFile = cla.Option("-sf|--senderCertificateFile", "Load the senders certificate from this file.", CommandOptionType.SingleValue);
        _senderCertificateFilePassword = cla.Option("-sfp|--senderCertificateFilePassword", "Unlock the senders certificate file private key with this password for signing when encrypting.", CommandOptionType.SingleValue);
    }

    public bool ValidateArguments()
    {
        var isListingCertificates = _senderJsonList.HasValue() || _receiverJsonList.HasValue();

        // don't validate further if it has no in file but certificates are listed, because no encryption takes place
        if (!_inFile.HasValue())
        {
            if (isListingCertificates)
            {
                return ValidateArgumentsWithListingCertificatesWithoutInFiles();
            }
            else
            {
                Console.WriteLine($"No files specified for '--in. This is only available when certificates are listed");
                return false;
            }
        }

        Encrypt = true;

        // the in files need to exist
        foreach (var fileName in _inFile.Values)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"The file specified for '--in {fileName}' does not exist.");
                return false;
            }
        }

        // outfile required, if infile is provided
        if (_inFile.HasValue() && !_outFile.HasValue())
        {
            Console.WriteLine("If an 'in' file is specified, an 'out' file must also be provided.");
            return false;
        }

        // we need the same amount of in and out files
        if (_inFile.Values.Count != _outFile.Values.Count)
        {
            Console.WriteLine("The same amount of files for 'in' and 'out' must be specified.");
            return false;
        }

        return ValidateSenderCertificate() && ValidateReceiverCertificates();
    }

    private bool ValidateArgumentsWithListingCertificatesWithoutInFiles()
    {
        if (_senderJsonList.HasValue() && !ValidateSenderCertificate())
        {
            return false;
        }

        if (_receiverJsonList.HasValue() && !ValidateReceiverCertificates())
        {
            return false;
        }

        return true;
    }

    private bool ValidateSenderCertificate()
    {
        // sender cert is required when using sender json list
        if (!_senderCertificateFile.HasValue() && !_senderCertificateSubject.HasValue())
        {
            Console.WriteLine("A sender certificate is required. Please provide a sender subject name or sender certificate file.");
            return false;
        }

        // sender cert password is required and the file needs to exist when it is provided per file
        if (_senderCertificateFile.HasValue())
        {
            if (!_senderCertificateFilePassword.HasValue())
            {
                Console.WriteLine("A sender certificate password is required. Please provide the sender certificate password.");
                return false;
            }

            if (!File.Exists(_senderCertificateFile.Value()))
            {
                Console.WriteLine($"The specified receiver certificate file '{_receiverCertificateFile.Value()}' does not exist.");
                return false;
            }
        }

        return true;
    }

    private bool ValidateReceiverCertificates()
    {
        if (!_receiverCertificateFile.HasValue())
        {
            Console.WriteLine("At least one receiver certificate file must be provided when using receiver json list");
            return false;
        }

        // receiver certs need to exist
        foreach (var receiverCertificateFile in _receiverCertificateFile.Values)
        {
            if (!File.Exists(receiverCertificateFile))
            {
                Console.WriteLine($"The specified receiver certificate file '{_receiverCertificateFile.Value()}' does not exist.");
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
        logger.Information("Config - SenderJsonList: {JsonList}", _senderJsonList.HasValue() ? "yes" : "no");
        logger.Information("Config - ReceiverJsonList: {JsonList}", _receiverJsonList.HasValue() ? "yes" : "no");
        logger.Information("Config - InFiles(s): {InFiles}", _inFile.HasValue() ? string.Join(", ", _inFile.Values) : "n.a.");
        logger.Information("Config - OutFiles(s): {OutFiles}", _outFile.HasValue() ? string.Join(", ", _outFile.Values) : "n.a.");
        logger.Information("Config - ReceiverCertificateFile(s): {ReceiverCertificateFiles}", _receiverCertificateFile.HasValue() ? string.Join(", ", _receiverCertificateFile.Values) : "n.a.");
        logger.Information("Config - SenderCertificateSubject: {SenderCertificateSubject}", _senderCertificateSubject.HasValue() ? _senderCertificateSubject.Value() : "n.a.");
        logger.Information("Config - SenderCertificateFile: {SenderCertificateFile}", _senderCertificateFile.HasValue() ? _senderCertificateFile.Value() : "n.a.");
        logger.Information("Config - SenderCertificateFilePassword: {SenderCertificateFilePassword}", _senderCertificateFilePassword.HasValue() ? "********" : "n.a.");
    }
}
