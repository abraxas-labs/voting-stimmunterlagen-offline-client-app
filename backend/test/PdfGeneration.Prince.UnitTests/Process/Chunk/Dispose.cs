// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Moq;
using Thinktecture.IO;
using Xunit;
using SUT = PdfGeneration.Prince.Process.Chunk;

namespace PdfGeneration.Prince.UnitTests.Process.Chunk;

public class Dispose
{
    [Fact]
    public void Should_dispose_data_by_default()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.SetupGet(s => s.CanSeek).Returns(false);
        streamMock.Setup(s => s.Dispose()).Verifiable();
        var stream = streamMock.Object;

        var sut = new SUT("Tag", 0, stream);

        // act
        sut.Dispose();

        // assert
        streamMock.Verify();
    }

    [Fact]
    public void Should_not_dispose_when_asked_to_keep_stream_open()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.SetupGet(s => s.CanSeek).Returns(false);
        var stream = streamMock.Object;

        var sut = new SUT("Tag", 0, stream)
        {
            KeepStreamOpen = true
        };

        sut.Dispose();

        // assert
        streamMock.Verify();
    }
}
