using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Thinktecture.IO;
using Xunit;

namespace PdfGeneration.UnitTests.IO.StreamExtensions;

public class CopyOverBufferedAsync
{
    [Fact]
    public async Task Should_throw_on_null_input_stream()
    {
        // act
        Func<Task> act = async () => await ((IStream)null!).CopyOverBufferedAsync(null!, 10);

        // assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Should_throw_on_not_readble_input_stream()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.SetupGet(s => s.CanRead).Returns(false);
        var stream = streamMock.Object;

        // act
        Func<Task> act = async () => await stream.CopyOverBufferedAsync(null!, 10);

        //assert
        await act.Should().ThrowAsync<ArgumentException>();
        streamMock.Verify();
    }

    [Fact]
    public async Task Should_throw_on_null_output_stream()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.SetupGet(s => s.CanRead).Returns(true);
        var stream = streamMock.Object;

        // act
        Func<Task> act = async () => await stream.CopyOverBufferedAsync(null!, 10);

        //asset
        await act.Should().ThrowAsync<ArgumentNullException>();
        streamMock.Verify();
    }

    [Fact]
    public async Task Should_throw_on_not_writeable_output()
    {
        // arrange
        var inStreamMock = new Mock<IStream>(MockBehavior.Strict);
        inStreamMock.SetupGet(s => s.CanRead).Returns(true);
        var inStream = inStreamMock.Object;

        var outStreamMock = new Mock<IStream>(MockBehavior.Strict);
        outStreamMock.SetupGet(s => s.CanWrite).Returns(false);
        var outStream = outStreamMock.Object;

        // act
        Func<Task> act = async () => await inStream.CopyOverBufferedAsync(outStream, 10);

        // assert
        await act.Should().ThrowAsync<ArgumentException>();
        inStreamMock.Verify();
        outStreamMock.Verify();
    }

    [Fact]
    public async Task Should_throw_when_stream_did_not_read()
    {
        // arrange
        var inStreamMock = new Mock<IStream>(MockBehavior.Strict);
        inStreamMock.SetupGet(s => s.CanRead).Returns(true);
        inStreamMock.SetupGet(s => s.Length).Returns(10);
        inStreamMock.Setup(s => s.ReadAsync(It.IsAny<byte[]>(), 0, 10))
            .ReturnsAsync(-1);
        var inStream = inStreamMock.Object;

        var outStreamMock = new Mock<IStream>(MockBehavior.Strict);
        outStreamMock.SetupGet(s => s.CanWrite).Returns(true);
        var outStream = outStreamMock.Object;

        // act
        Func<Task> act = async () => await inStream.CopyOverBufferedAsync(outStream, 10);

        // assert
        await act.Should().ThrowAsync<IOException>();
        inStreamMock.Verify();
        outStreamMock.Verify();
    }

    [Fact]
    public async Task Should_throw_when_stream_did_over_read()
    {
        // arrange
        var inStreamMock = new Mock<IStream>(MockBehavior.Strict);
        inStreamMock.SetupGet(s => s.CanRead).Returns(true);
        inStreamMock.SetupGet(s => s.Length).Returns(10);
        inStreamMock.Setup(s => s.ReadAsync(It.IsAny<byte[]>(), 0, 10))
            .ReturnsAsync(11);
        var inStream = inStreamMock.Object;

        var outStreamMock = new Mock<IStream>(MockBehavior.Strict);
        outStreamMock.SetupGet(s => s.CanWrite).Returns(true);
        var outStream = outStreamMock.Object;

        // act
        Func<Task> act = async () => await inStream.CopyOverBufferedAsync(outStream, 10);

        // assert
        await act.Should().ThrowAsync<IOException>();
        inStreamMock.Verify();
        outStreamMock.Verify();
    }

    [Fact]
    public async Task Should_copy_in_one_go()
    {
        // arrange
        var bufferLength = 10;

        var inStreamMock = new Mock<IStream>(MockBehavior.Strict);
        inStreamMock.SetupGet(s => s.CanRead).Returns(true);
        inStreamMock.SetupGet(s => s.Length).Returns(10);
        inStreamMock.Setup(s => s.ReadAsync(It.Is<byte[]>(buffer => buffer.Length == bufferLength), 0, 10))
            .ReturnsAsync(10)
            .Callback((byte[] buffer, int _, int _) =>
            {
                for (var i = 0; i < buffer.Length; i++)
                    buffer[i] = (byte)i;
            });
        var inStream = inStreamMock.Object;

        var outStreamMock = new Mock<IStream>(MockBehavior.Strict);
        outStreamMock.SetupGet(s => s.CanWrite).Returns(true);
        outStreamMock.Setup(s => s.WriteAsync(It.Is<byte[]>(buffer => buffer.Length == 10 && buffer[0] == 0 && buffer[9] == 9), 0, 10))
            .Returns(Task.CompletedTask);
        var outStream = outStreamMock.Object;

        // act
        await inStream.CopyOverBufferedAsync(outStream, 10, bufferLength);

        // assert
        inStreamMock.Verify();
        outStreamMock.Verify();
    }

    [Fact]
    public async Task Should_copy_in_two_steps_for_long_data()
    {
        // arrange
        var bufferLength = 10;

        var inStreamMock = new Mock<IStream>(MockBehavior.Strict);
        inStreamMock.SetupGet(s => s.CanRead).Returns(true);
        inStreamMock.SetupGet(s => s.Length).Returns(11);
        inStreamMock.Setup(s => s.ReadAsync(It.Is<byte[]>(buffer => buffer.Length == bufferLength), 0, 10))
            .ReturnsAsync(10)
            .Callback((byte[] buffer, int _, int _) =>
            {
                for (var i = 0; i < buffer.Length; i++)
                    buffer[i] = (byte)i;
            });
        inStreamMock.Setup(s => s.ReadAsync(It.Is<byte[]>(buffer => buffer.Length == bufferLength), 0, 1))
            .ReturnsAsync(1)
            .Callback((byte[] buffer, int _, int _) =>
            {
                buffer[0] = (byte)'X';
            });
        var inStream = inStreamMock.Object;

        var outStreamMock = new Mock<IStream>(MockBehavior.Strict);
        outStreamMock.SetupGet(s => s.CanWrite).Returns(true);
        outStreamMock.Setup(s => s.WriteAsync(It.Is<byte[]>(buffer => buffer.Length == 10 && buffer[0] == 0 && buffer[9] == 9), 0, 10))
            .Returns(Task.CompletedTask);
        outStreamMock.Setup(s => s.WriteAsync(It.Is<byte[]>(buffer => buffer.Length == 10 && buffer[0] == (byte)'X'), 0, 1))
            .Returns(Task.CompletedTask);
        var outStream = outStreamMock.Object;

        // act
        await inStream.CopyOverBufferedAsync(outStream, 10, bufferLength);

        // assert
        inStreamMock.Verify();
        outStreamMock.Verify();
    }
}
