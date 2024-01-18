﻿using EVoting.Schemas;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml;
using Microsoft.Extensions.Logging;
using EchDeliveryGeneration.Ech0045;
using System.IO;
using EchDeliveryJsonConverter.Converters;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Ech0228_1_0;
using EchDeliveryGeneration.Post;
using Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

namespace EchDeliveryGeneration;

public class EchDeliveryGenerator
{
    private readonly ILogger<EchDeliveryGenerator> _logger;
    private readonly Ech0045Reader _ech0045Reader;
    private readonly PostDataTransformer _postDataTransformer;

    private const string PostConfigXmlNamespace = "http://www.evoting.ch/xmlns/config";
    private const string PostPrintXmlNamespace = "http://www.evoting.ch/xmlns/print";
    private const string Ech0045XmlNamespace = "http://www.ech.ch/xmlns/eCH-0045/4";

    public EchDeliveryGenerator(ILogger<EchDeliveryGenerator> logger, Ech0045Reader ech0045Reader, PostDataTransformer postDataTransformer)
    {
        _logger = logger;
        _ech0045Reader = ech0045Reader;
        _postDataTransformer = postDataTransformer;
    }

    public async Task<Delivery> GenerateDelivery(
        IReadOnlyCollection<string> files)
    {
        var delivery = new Delivery();

        var config = new List<EVoting.Config.Configuration>();
        var votingCardLists = new List<EVoting.Print.VotingCardList>();
        var echVoterByPersonId = new Dictionary<string, Ech0045VoterExtension>();
        var jsonConfig = new Configuration();
        var settings = CreateXmlReaderSettings();

        foreach (var file in files)
        {
            if (file.EndsWith(".xml"))
            {
                using var stream = File.OpenRead(file);
                using var xr = XmlReader.Create(stream, settings);
                xr.MoveToContent();
                var xmlTypeRef = xr.GetAttribute("xmlns");
                if (!string.IsNullOrEmpty(xmlTypeRef) &&
                    xmlTypeRef.Contains(PostConfigXmlNamespace))
                {
                    config.Add(EVotingXmlSerializer.DeserializeXml<EVoting.Config.Configuration>(xr));
                }

                xmlTypeRef = xr.GetAttribute("xmlns");
                if (!string.IsNullOrEmpty(xmlTypeRef) &&
                    xmlTypeRef.Contains(PostPrintXmlNamespace))
                {
                    votingCardLists.Add(EVotingXmlSerializer.DeserializeXml<EVoting.Print.VotingCardList>(xr));
                }

                xmlTypeRef = xr.GetAttribute("xmlns:eCH-0045");
                if (!string.IsNullOrEmpty(xmlTypeRef) &&
                    xmlTypeRef.Contains(Ech0045XmlNamespace))
                {
                    stream.Dispose();

                    // to deserialize the xml, the file stream needs to start from the beginning.
                    using var resettedStream = File.OpenRead(file);

                    echVoterByPersonId = await _ech0045Reader.ReadVoterExtensions(resettedStream);
                }
            }
            else if (file.EndsWith(".json"))
            {
                using var stream = File.OpenText(file);

                JsonSerializer serializer = new();
                jsonConfig = (Configuration?)serializer.Deserialize(stream, typeof(Configuration));
            }
        }

        if (jsonConfig == null)
            throw new ValidationException($"{nameof(jsonConfig)} is not provided");

        if (votingCardLists.Count > 0)
            delivery = _postDataTransformer.Transform(config, votingCardLists, jsonConfig, echVoterByPersonId);

        return delivery;
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
