using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;
using Xunit;
using PdfGeneration.Prince.Process.Internal;
using System.Runtime.InteropServices;

namespace PdfGeneration.Prince.IntegrationTests;

public class StreamPdfGeneration
{
    [Fact]
    public async Task Test_complete_in_memory_pdf_generation()
    {
        // this test requires prince xml
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        // arrange
        var directory = new DirectoryAdapter();
        var path = new PathAdapter();
        var file = new FileAdapter();
        var config = new PrinceConfiguration();
        var communicator = new PrinceStreamCommunicator(null);
        var processWrapper = new PrinceProcessWrapper(null, path, file, directory, config, communicator);
        var messageLogger = new PrinceMessageLogger(null);
        var sut = new PrincePdfGenerator(null, new PrinceProcessManager(null, processWrapper, messageLogger));

        // input
        var htmlStream = GetHtmlStream();

        // out stream
        if (!Directory.Exists("./TestResult"))
            Directory.CreateDirectory("./TestResult");

        var resultFile = "./TestResult/generated.pdf";

        // act
        using (var fs = File.OpenWrite(resultFile))
        using (var outStream = new StreamAdapter(fs))
        {
            await sut.GeneratePdfAsync(htmlStream, outStream);
        }

        // assert
        using (var fs = File.OpenRead(resultFile))
        using (var outStream = new StreamAdapter(fs))
        {
            outStream.Position = 0;
            var buffer = new byte[4];
            await outStream.ReadAsync(buffer, 0, 4);

            // quick and dirty check if its a pdf by file preamble
            buffer.Should().BeEquivalentTo(new byte[] { 0x25, 0x50, 0x44, 0x46 });
        }
    }

    private static IStream GetHtmlStream()
    {
        var html = @"<!DOCTYPE html>
<html lang=""en"">
<head>
</head>
<body>
    <table>
        <thead>
            <tr><td>Name</td><td>Age</td></tr>
        </thead>
        <tbody>
                <tr><td>Alice</td><td>34</td></tr>
                <tr><td>Bob</td><td>37</td></tr>
                <tr><td>Mr. Smith</td><td>28</td></tr>
        </tbody>
    </table>
</body>
</html>";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(html))
        {
            Position = 0
        };

        return new StreamAdapter(stream);
    }
}
