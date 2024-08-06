// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Voting.Lib.Testing.Utils;
using Xunit;
using System.Reflection;
using System;
using EchDeliveryGeneration.Validation;
using FluentAssertions;

namespace EchDeliveryGeneration.IntegrationTests.Generator;


public class EchDeliveryGeneratorTest
{
    private const string ContestConfigFile = "params.json";
    private const string PostConfigFile = "post-config.xml";
    private const string PostPrintFile = "post-print.xml";
    private const string Ech0045File = "ech-0045.xml";

    // keep in sync with EchDeliveryGenerator.Application
    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        DateFormatString = "dd.MM.yyyy HH:mm:ss",
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Formatting = Formatting.Indented,
    };

    private readonly EchDeliveryGenerator _generator;

    public EchDeliveryGeneratorTest()
    {
        using var serviceProvider = new ServiceCollection()
            .AddEchDeliveryGeneration()
            .AddLogging()
            .BuildServiceProvider();

        _generator = serviceProvider.GetRequiredService<EchDeliveryGenerator>();
    }

    [Fact]
    public async Task TestGenerate()
    {
        var inputFiles = new string[]
        {
            GetTestFilePath(ContestConfigFile),
            GetTestFilePath(PostConfigFile),
            GetTestFilePath(PostPrintFile),
            GetTestFilePath(Ech0045File),
        };

        var generatorResult = await _generator.GenerateDelivery(inputFiles, null);
        generatorResult.PostSignatureValidationResult.Code.Should().Be(PostSignatureValidationResultCodes.Skipped);
        var serializedDelivery = JsonConvert.SerializeObject(generatorResult.Delivery, _jsonSerializerSettings);

#if UPDATE_SNAPSHOTS
        var updateSnapshot = true;
#else
        var updateSnapshot = false;
#endif
        serializedDelivery.MatchRawSnapshot(
            Path.Join(
                FindProjectSourceDirectory(),
                "test",
                "EchDeliveryGeneration.IntegrationTests",
                "Generator",
                "_snapshots",
                "EchDeliveryGeneratorTest_TestGenerate.json"),
            updateSnapshot);
    }

    private string GetTestFilePath(string fileName)
    {
        var assemblyFolder = Path.GetDirectoryName(GetType().Assembly.Location);
        return Path.Join(assemblyFolder, $"Generator/TestFiles/{fileName}");
    }

    private static string FindProjectSourceDirectory()
    {
        var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                  ?? throw new InvalidOperationException();

        do
        {
            if (Directory.GetFiles(dir, "*.sln", SearchOption.TopDirectoryOnly).Length > 0)
            {
                return dir;
            }

            dir = Path.GetDirectoryName(dir);
        }
        while (dir != null);

        throw new InvalidOperationException();
    }
}
