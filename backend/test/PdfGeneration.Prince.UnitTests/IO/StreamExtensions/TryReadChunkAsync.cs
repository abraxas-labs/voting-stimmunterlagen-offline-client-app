using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;
using Xunit;

namespace PdfGeneration.Prince.UnitTests.IO.StreamExtensions;

public class TryReadChunkAsync
{
    [Fact]
    public Task Should_throw_on_missing_stream()
    {
        // arrange
        IStream stream = null!;

        // act
        Func<Task> act = () => stream.TryReadChunkAsync("Tag");

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_not_readable_input_stream()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.Setup(s => s.CanRead).Returns(false);
        var stream = streamMock.Object;

        // act
        Func<Task> act = () => stream.TryReadChunkAsync("Tag");

        // assert
        return act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public Task Should_throw_on_too_short_data()
    {
        // arrange
        var stream = CreateStreamFromString("Tag");

        // act
        Func<Task> act = () => stream.TryReadChunkAsync("Tag");

        // assert
        return act.Should().ThrowAsync<IOException>();
    }


    [Fact]
    public Task Should_throw_on_missing_spacer()
    {
        // arrange
        var stream = CreateStreamFromString("Tag_");

        // act
        Func<Task> act = () => stream.TryReadChunkAsync("Tag");

        // assert
        return act.Should().ThrowAsync<IOException>();
    }


    [Fact]
    public Task Should_throw_on_missing_length()
    {
        // arrange
        var stream = CreateStreamFromString("Tag \n");

        // act
        Func<Task> act = () => stream.TryReadChunkAsync("Tag");

        // assert
        return act.Should().ThrowAsync<IOException>();
    }

    [Fact]
    public Task Should_throw_on_too_long_length()
    {
        // arrange
        var stream = CreateStreamFromString("Tag 1234567890\nData\n");

        // act
        Func<Task> act = () => stream.TryReadChunkAsync("Tag");

        // assert
        return act.Should().ThrowAsync<IOException>();
    }

    [Fact]
    public Task Should_throw_on_missing_newline_after_data()
    {
        // arrange
        var stream = CreateStreamFromString("Tag 4\nData");

        // act
        Func<Task> act = () => stream.TryReadChunkAsync("Tag");

        // assert
        return act.Should().ThrowAsync<IOException>();
    }

    [Fact]
    public async Task Should_read_chunk_to_internal_stream()
    {
        // arrange
        var stream = CreateStreamFromString("Tag 4\nData\n");

        // act
        var chunk = await stream.TryReadChunkAsync("Tag");

        // assert
        chunk.Tag.Should().Be("Tag");
        chunk.Data.Position = 0;
        chunk.Data.ReadByte().Should().Be((byte)'D');
        chunk.Data.ReadByte().Should().Be((byte)'a');
        chunk.Data.ReadByte().Should().Be((byte)'t');
        chunk.Data.ReadByte().Should().Be((byte)'a');
        chunk.Data.ReadByte().Should().Be(-1);
    }

    [Fact]
    public async Task Should_read_wrong_chunk_to_internal_stream()
    {
        // arrange
        var stream = CreateStreamFromString("Tag 4\nData\n");

        var outStream = new StreamAdapter(new MemoryStream());

        // act
        var chunk = await stream.TryReadChunkAsync("TAG", outStream);

        // assert
        chunk.Tag.Should().Be("Tag");

        chunk.Data.Position = 0;
        chunk.Data.ReadByte().Should().Be((byte)'D');
        chunk.Data.ReadByte().Should().Be((byte)'a');
        chunk.Data.ReadByte().Should().Be((byte)'t');
        chunk.Data.ReadByte().Should().Be((byte)'a');
        chunk.Data.ReadByte().Should().Be(-1);

        chunk.Data.Should().NotBe(outStream);
        outStream.Length.Should().Be(0);
    }

    [Fact]
    public async Task Should_read_correct_chunk_to_passed_in_stream()
    {
        // arrange
        var stream = CreateStreamFromString("Tag 4\nData\n");

        var outStream = new StreamAdapter(new MemoryStream());

        // act
        var chunk = await stream.TryReadChunkAsync("Tag", outStream);

        // assert
        chunk.Tag.Should().Be("Tag");

        chunk.Data.Should().Be(outStream);

        outStream.Position = 0;
        outStream.ReadByte().Should().Be((byte)'D');
        outStream.ReadByte().Should().Be((byte)'a');
        outStream.ReadByte().Should().Be((byte)'t');
        outStream.ReadByte().Should().Be((byte)'a');
        outStream.ReadByte().Should().Be(-1);
    }

    private static IStream CreateStreamFromString(string data)
    {
        return new StreamAdapter(new MemoryStream(Encoding.UTF8.GetBytes(data)));
    }
}
