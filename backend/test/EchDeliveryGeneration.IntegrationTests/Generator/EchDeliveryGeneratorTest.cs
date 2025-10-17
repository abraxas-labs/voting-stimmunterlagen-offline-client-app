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


public class EchDeliveryGeneratorTest : IDisposable
{
    private readonly ServiceProvider _sp;

    private const string ContestConfigV1File = "params_v1.json";
    private const string ContestConfigV2File = "params_v2.json";
    private const string PostConfigV6File = "post-config_v6_0.xml";
    private const string PostConfigV7File = "post-config_v7_0.xml";
    // private const string PostConfigV7File = "configuration-anonymized.xml";
    private const string PostPrintV1File = "post-print_v1_0.xml";
    private const string PostPrintV2File = "post-print_v2_0.xml";
    private const string Ech0045V4File = "ech-0045_v4_0.xml";
    private const string Ech0045V6File = "ech-0045_v4_2.xml";


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
        _sp = new ServiceCollection()
            .AddEchDeliveryGeneration()
            .AddLogging()
            .BuildServiceProvider();

        _generator = _sp.GetRequiredService<EchDeliveryGenerator>();
    }

    [Fact]
    public async Task TestGeneratePostPrintV1()
    {
        var inputFiles = new string[]
        {
            GetTestFilePath(ContestConfigV1File),
            GetTestFilePath(PostConfigV6File),
            GetTestFilePath(PostPrintV1File),
        };

        var generatorResult = await _generator.GenerateDelivery(inputFiles, null);
        generatorResult.PostSignatureValidationResult.Code.Should().Be(PostSignatureValidationResultCodes.Skipped);
        var serializedDelivery = JsonConvert.SerializeObject(generatorResult.Delivery, _jsonSerializerSettings);
        MatchJsonSnapshot(serializedDelivery, nameof(TestGeneratePostPrintV1));
    }

    [Fact]
    public async Task TestGeneratePostPrintV2()
    {
        var inputFiles = new string[]
        {
            GetTestFilePath(ContestConfigV2File),
            GetTestFilePath(PostConfigV7File),
            GetTestFilePath(PostPrintV2File),
        };

        var generatorResult = await _generator.GenerateDelivery(inputFiles, null);
        generatorResult.PostSignatureValidationResult.Code.Should().Be(PostSignatureValidationResultCodes.Skipped);
        var serializedDelivery = JsonConvert.SerializeObject(generatorResult.Delivery, _jsonSerializerSettings);
        MatchJsonSnapshot(serializedDelivery, nameof(TestGeneratePostPrintV2));
    }

    [Fact]
    public async Task TestGenerateEch0045v4_0()
    {
        var inputFiles = new string[]
        {
            GetTestFilePath(ContestConfigV1File),
            GetTestFilePath(PostConfigV6File),
            GetTestFilePath(PostPrintV1File),
            GetTestFilePath(Ech0045V4File),
        };

        var generatorResult = await _generator.GenerateDelivery(inputFiles, null);
        generatorResult.PostSignatureValidationResult.Code.Should().Be(PostSignatureValidationResultCodes.Skipped);
        var serializedDelivery = JsonConvert.SerializeObject(generatorResult.Delivery, _jsonSerializerSettings);
        MatchJsonSnapshot(serializedDelivery, nameof(TestGenerateEch0045v4_0));
    }

    [Fact]
    public async Task TestGenerateEch0045v6_0()
    {
        var inputFiles = new string[]
        {
            GetTestFilePath(ContestConfigV1File),
            GetTestFilePath(PostConfigV6File),
            GetTestFilePath(PostPrintV1File),
            GetTestFilePath(Ech0045V6File),
        };

        var generatorResult = await _generator.GenerateDelivery(inputFiles, null);
        generatorResult.PostSignatureValidationResult.Code.Should().Be(PostSignatureValidationResultCodes.Skipped);
        var serializedDelivery = JsonConvert.SerializeObject(generatorResult.Delivery, _jsonSerializerSettings);
        MatchJsonSnapshot(serializedDelivery, nameof(TestGenerateEch0045v6_0));
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

    private void MatchJsonSnapshot(string fileContent, string testName)
    {
#if UPDATE_SNAPSHOTS
        var updateSnapshot = true;
#else
        var updateSnapshot = true;
#endif

        fileContent.MatchRawSnapshot(
            Path.Join(
                FindProjectSourceDirectory(),
                "test",
                "EchDeliveryGeneration.IntegrationTests",
                "Generator",
                "_snapshots",
                $"EchDeliveryGeneratorTest_{testName}.json"),
            updateSnapshot);
    }

    public void Dispose()
    {
        _sp.Dispose();
    }
}

