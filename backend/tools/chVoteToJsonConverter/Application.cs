using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using chVoteToJsonConverter.Converters;
using chVoteToJsonConverter.ErrorHandling;
using chVoteToJsonConverter.Schemas;
using eCH_0045_4_0;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

namespace chVoteToJsonConverter;

public class Application
{
    private readonly ILogger<Application> _logger;
    private readonly Arguments _arguments;


    public Application(ILogger<Application> logger, Arguments arguments)
    {
        _logger = logger;
        _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
    }

    public void Run()
    {
        var data = LoadChVoteData();

        if (data != null)
        {
            WriteJsonData(data);
        }
    }

    private object? LoadChVoteData()
    {
        _logger.LogInformation("Reading chVote XML data from file {InFileName}", _arguments.InFile);

        try
        {
            var settings = CreateXmlReaderSettings();

            var voterDelivery = default(object);

            if (_arguments.HasMultipleInFiles)
            {
                var config = new List<configuration>();
                var votingCards = new List<votingCardList>();
                var eCH0045voterDelivery = new List<VoterDelivery>();
                var jsonConfig = default(Configuration);

                foreach (var inFile in _arguments.MultipleInFiles)
                {
                    if (inFile.EndsWith(".xml"))
                    {
                        using var stream = File.OpenRead(inFile);
                        using var xr = XmlReader.Create(stream, settings);
                        xr.MoveToContent();
                        var xmlTypeRef = xr.GetAttribute("xmlns");
                        if (!string.IsNullOrEmpty(xmlTypeRef) &&
                            xmlTypeRef.Contains("http://www.evoting.ch/xmlns/config/"))
                        {
                            config.Add(EVotingXmlSerializer.DeserializeXml<configuration>(xr));
                        }

                        xmlTypeRef = xr.GetAttribute("xmlns");
                        if (!string.IsNullOrEmpty(xmlTypeRef) &&
                            xmlTypeRef.Contains("http://www.evoting.ch/xmlns/print"))
                        {
                            votingCards.Add(EVotingXmlSerializer.DeserializeXml<votingCardList>(xr));
                        }

                        xmlTypeRef = xr.GetAttribute("xmlns:eCH-0045");
                        if (!string.IsNullOrEmpty(xmlTypeRef) &&
                            xmlTypeRef.Contains("http://www.ech.ch/xmlns/eCH-0045/4"))
                        {
                            eCH0045voterDelivery.Add(EVotingXmlSerializer.DeserializeXml<VoterDelivery>(xr));
                        }
                    }
                    else if (inFile.EndsWith(".json"))
                    {
                        using var stream = File.OpenText(inFile);

                        JsonSerializer serializer = new();
                        jsonConfig = (Configuration?)serializer.Deserialize(stream, typeof(Configuration));
                    }
                }

                if (jsonConfig == null)
                    throw new ValidationException($"{nameof(jsonConfig)} is not provided");

                if (votingCards.Count > 0)
                    voterDelivery = DataTransformer.Transform(config, votingCards, jsonConfig);
                else if (eCH0045voterDelivery.Count > 0)
                    voterDelivery = DataTransformer.Transform(eCH0045voterDelivery, jsonConfig);
            }
            else
            {
                using var stream = _arguments.XmlData;
                using var xr = XmlReader.Create(stream, settings);

                xr.MoveToContent();
                var xmlTypeRef = xr.GetAttribute("xmlns:pfve");
                if (!string.IsNullOrEmpty(xmlTypeRef) && xmlTypeRef.Contains("printerFileVotationElection"))
                {
                    voterDelivery = EVotingXmlSerializer.DeserializeXml<chVote.votingCardsPrinterFileType1>(xr);
                }

                xmlTypeRef = xr.GetAttribute("xmlns:pf");
                if (!string.IsNullOrEmpty(xmlTypeRef) && xmlTypeRef.Contains("printerFile"))
                {
                    voterDelivery = EVotingXmlSerializer.DeserializeXml<chVote.votingCardsPrinterFileType>(xr);
                }
            }

            return voterDelivery;
        }
        catch (Exception ex)
        {
            if (ex is TransformationException transformationException)
            {
                return transformationException.GetError();
            }

            _logger?.LogError(ex, "An error occured while loading voter data.");
            return null;
        }
    }

    private XmlReaderSettings CreateXmlReaderSettings()
    {
        var settings = new XmlReaderSettings()
        {
            ValidationType = ValidationType.Schema,
            Schemas = ChVoteSchemaLoader.LoadChVoteSchemas(),
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
                _logger.LogWarning("XML validation warning: {XmlValidationWarning}", e.Message);
            }
            else
            {
                _logger.LogError("XML validation error: {XmlValidationError}", e.Message);
            }
        };

        return settings;
    }

    private void WriteJsonData(object data)
    {
        using var writer = new StreamWriter(_arguments.JsonData);
        using var jsonWriter = new JsonTextWriter(writer);

        var ser = new JsonSerializer()
        {
            DateFormatString = "dd.MM.yyyy HH:mm:ss"
        };
        ser.Serialize(jsonWriter, data);
        jsonWriter.Flush();
    }
}
