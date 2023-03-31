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

public class ReadChunkAsync
{
    [Fact]
    public Task Should_throw_on_missing_stream()
    {
        // arrange
        IStream stream = null!;

        // act
        Func<Task> act = () => stream.ReadChunkAsync();

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
        Func<Task> act = () => stream.ReadChunkAsync();

        // assert
        return act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public Task Should_throw_on_too_short_data()
    {
        // arrange
        var stream = CreateStreamFromString("Tag");

        // act
        Func<Task> act = () => stream.ReadChunkAsync();

        // assert
        return act.Should().ThrowAsync<IOException>();
    }


    [Fact]
    public Task Should_throw_on_missing_spacer()
    {
        // arrange
        var stream = CreateStreamFromString("Tag_");

        // act
        Func<Task> act = () => stream.ReadChunkAsync();

        // assert
        return act.Should().ThrowAsync<IOException>();
    }


    [Fact]
    public Task Should_throw_on_missing_length()
    {
        // arrange
        var stream = CreateStreamFromString("Tag \n");

        // act
        Func<Task> act = () => stream.ReadChunkAsync();

        // assert
        return act.Should().ThrowAsync<IOException>();
    }

    [Fact]
    public Task Should_throw_on_too_long_length()
    {
        // arrange
        var stream = CreateStreamFromString("Tag 1234567890\nData\n");

        // act
        Func<Task> act = () => stream.ReadChunkAsync();

        // assert
        return act.Should().ThrowAsync<IOException>();
    }

    [Fact]
    public Task Should_throw_on_missing_newline_after_data()
    {
        // arrange
        var stream = CreateStreamFromString("Tag 4\nData");

        // act
        Func<Task> act = () => stream.ReadChunkAsync();

        // assert
        return act.Should().ThrowAsync<IOException>();
    }

    [Fact]
    public async Task Should_read_chunk()
    {
        // arrange
        var stream = CreateStreamFromString("Tag 4\nData\n");

        // act
        var chunk = await stream.ReadChunkAsync();

        // assert
        chunk.Tag.Should().Be("Tag");
        chunk.Data.ReadByte().Should().Be((byte)'D');
        chunk.Data.ReadByte().Should().Be((byte)'a');
        chunk.Data.ReadByte().Should().Be((byte)'t');
        chunk.Data.ReadByte().Should().Be((byte)'a');
        chunk.Data.ReadByte().Should().Be(-1);
    }

    private static IStream CreateStreamFromString(string data)
    {
        return new StreamAdapter(new MemoryStream(Encoding.UTF8.GetBytes(data)));
    }
}
