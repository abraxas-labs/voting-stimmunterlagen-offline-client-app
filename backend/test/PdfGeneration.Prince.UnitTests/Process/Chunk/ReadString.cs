// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using System.Text;
using FluentAssertions;
using Thinktecture.IO.Adapters;
using Xunit;
using SUT = PdfGeneration.Prince.Process.Chunk;

namespace PdfGeneration.Prince.UnitTests.Process.Chunk;

public class ReadString
{
    [Fact]
    public void Should_return_stream_contents()
    {
        // arrange
        var ms = new MemoryStream(Encoding.UTF8.GetBytes("This is a test string."));
        var stream = new StreamAdapter(ms);
        var sut = new SUT("Tag", (int)stream.Length, stream);

        // act
        var result = sut.ReadString();

        // assert
        result.Should().Be("This is a test string.");
    }
}
