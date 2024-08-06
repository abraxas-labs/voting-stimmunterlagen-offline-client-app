// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace PdfGeneration.Prince.Process.Internal;

using System;
using System.IO;
using Microsoft.Extensions.Logging;

public class PrinceMessageLogger : IPrinceMessageLogger
{
    private const char MessageDelimiter = '|';

    private readonly ILogger<PrinceMessageLogger>? _logger;

    public PrinceMessageLogger(ILogger<PrinceMessageLogger>? logger)
    {
        _logger = logger;
    }

    public bool ProcessLogMessagesFromChunk(Chunk logChunk)
    {
        if (logChunk == null)
            throw new ArgumentNullException(nameof(logChunk));
        if (logChunk.Tag != "log")
            throw new ArgumentException("Provided chunk ist not a log chunk", nameof(logChunk));

        var result = string.Empty;

        using (var sr = new StreamReader(logChunk.Data.UnsafeConvert()))
        {
            var line = sr.ReadLine();
            while (line != null)
            {
                if (line.Length >= 4)
                {
                    var tag = line.Substring(0, 4);
                    var body = line.Substring(4);

                    if (tag.Equals("msg|"))
                    {
                        HandleMessage(body);
                    }

                    else if (tag.Equals("dat|"))
                    {
                        var dataParts = body.Split(new[] { MessageDelimiter }, 2);
                        _logger?.LogInformation("Received data log from Prince. {FirstPart} {SecondPart}", dataParts[0], dataParts[1]);
                    }
                    else if (tag.Equals("fin|"))
                    {
                        result = body;
                    }

                    // ignore unknown log messages
                }

                line = sr.ReadLine();
            }
        }

        return result == "success";
    }

    private void HandleMessage(string messageBody)
    {
        // ignore too short messages
        if (messageBody.Length < 4)
            return;

        var messageType = messageBody.Substring(0, 3);
        var messagePayload = messageBody.Substring(4);
        var locationOffset = messagePayload.IndexOf(MessageDelimiter);

        // ignore incorrectly formatted messages
        if (locationOffset == -1)
            return;

        var messageLocation = messagePayload.Substring(0, locationOffset);
        var messageText = messagePayload.Substring(locationOffset + 1);
        var logLevel = messageType switch
        {
            "vrb" => LogLevel.Trace,
            "dbg" => LogLevel.Trace,
            "wrn" => LogLevel.Warning,
            "err" => LogLevel.Error,
            _ => LogLevel.Debug // also includes "inf"
        };
        _logger?.Log(
            logLevel,
            0,
            "Message received from Prince. Type: {MessageType}, Location: {MessageLocation}, Text: {MessageText}",
            messageType,
            messageLocation,
            messageText
        );
    }
}
