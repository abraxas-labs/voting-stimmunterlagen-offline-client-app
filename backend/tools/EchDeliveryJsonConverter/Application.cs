﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using EchDeliveryGeneration;
using EchDeliveryGeneration.ErrorHandling;
using System.Collections.Generic;
using eCH_0228;
using System.Linq;

namespace EchDeliveryJsonConverter;

public class Application
{
    private const int VotingCardBatchSize = 100;

    private readonly ILogger<Application> _logger;
    private readonly Arguments _arguments;
    private readonly EchDeliveryGenerator _echDeliveryGenerator;
    private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { DateFormatString = "dd.MM.yyyy HH:mm:ss" };


    public Application(ILogger<Application> logger, Arguments arguments, EchDeliveryGenerator echDeliveryGenerator)
    {
        _logger = logger;
        _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        _echDeliveryGenerator = echDeliveryGenerator;
    }

    public async Task Run()
    {
        var data = await LoadEchDelivery();

        if (data != null)
        {
            WriteOutput(data);
        }
    }

    private async Task<object?> LoadEchDelivery()
    {
        try
        {
            var voterDelivery = await _echDeliveryGenerator.GenerateDelivery(_arguments.MultipleInFiles);
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

    private void WriteOutput(object data)
    {
        var chunks = new List<object>();

        switch (data)
        {
            case Delivery deliveryData:
                // Send voting cards and delivery in different chunks, for performance reasons.
                var votingCards = deliveryData.VotingCardDelivery.VotingCardData;

                // One element is required for the xml validation. This should be ignored by the consuming service, because it will be present in the voting card batches.
                deliveryData.VotingCardDelivery.VotingCardData = new() { votingCards.First() };

                // 1.   Chunk: eCH-0228 delivery without voting cards
                // 2.   Chunk: Count of voting cards
                // 3-n. Chunk: Voting cards batches
                chunks.Add(deliveryData);
                chunks.Add(votingCards.Count);

                foreach (var votingCardsBatch in votingCards.Chunk(VotingCardBatchSize))
                {
                    chunks.Add(votingCardsBatch);
                }
                break;
            case TransformationError error:
                chunks.Add(error);
                break;
            default:
                throw new InvalidOperationException("Invalid output type");
        }

        using var writer = new StreamWriter(_arguments.JsonData);

        for (var i = 0; i < chunks.Count; i++)
        {
            writer.Write(JsonConvert.SerializeObject(chunks[i], _jsonSerializerSettings));
            writer.Write($"\n--- CHUNK {i + 1} ---\n");
            writer.Flush();
        }
    }
}
