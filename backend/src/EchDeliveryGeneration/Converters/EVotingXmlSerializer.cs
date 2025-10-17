// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using EVoting.Schemas;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EchDeliveryJsonConverter.Converters;

public class EVotingXmlSerializer
{
    private readonly ILogger<EVotingXmlSerializer> _logger;

    public EVotingXmlSerializer(ILogger<EVotingXmlSerializer> logger)
    {
        _logger = logger;
    }

    internal T DeserializeXml<T>(string path)
    {
        var settings = CreateXmlReaderSettings();

        using var stream = File.OpenRead(path);
        using var xmlReader = XmlReader.Create(stream, settings);

        try
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T?)serializer.Deserialize(xmlReader)
                   ?? throw new ValidationException("Deserialization returned null");
        }
        catch (InvalidOperationException ex) when (ex.InnerException != null)
        {
            // The XmlSerializer wraps all exceptions into an InvalidOperationException.
            // Unwrap it to surface the "correct" exception type.
            throw ex.InnerException;
        }
    }

    private XmlReaderSettings CreateXmlReaderSettings()
    {
        var settings = new XmlReaderSettings()
        {
            ValidationType = ValidationType.Schema,
            Schemas = EVotingSchemaLoader.LoadEVotingSchemas(),
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersFromEntities = 1024
        };

        settings.ValidationFlags = settings.ValidationFlags
                                   | XmlSchemaValidationFlags.ProcessInlineSchema
                                   | XmlSchemaValidationFlags.ProcessSchemaLocation
                                   | XmlSchemaValidationFlags.ReportValidationWarnings;

        settings.ValidationEventHandler += (_, e) =>
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                _logger?.LogWarning("XML validation warning: {XmlValidationWarning}", e.Message);
            }
            else
            {
                _logger?.LogError("XML validation error: {XmlValidationError}", e.Message);
            }
        };

        return settings;
    }
}
