using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;
using Voting.Lib.Testing.Utils;
using Xunit;

namespace EchDeliveryGeneration.IntegrationTests.Generator;


public class EchDeliveryGeneratorTest
{
    private const string ContestConfigFile = "params.json";
    private const string PostConfigFile = "post-config.xml";
    private const string PostPrintFile = "post-print.xml";
    private const string Ech0045File = "ech-0045.xml";

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

        var delivery = await _generator.GenerateDelivery(inputFiles);
        delivery.MatchSnapshot();
    }

    private string GetTestFilePath(string fileName)
    {
        var assemblyFolder = Path.GetDirectoryName(GetType().Assembly.Location);
        return Path.Join(assemblyFolder, $"Generator/TestFiles/{fileName}");
    }
}
