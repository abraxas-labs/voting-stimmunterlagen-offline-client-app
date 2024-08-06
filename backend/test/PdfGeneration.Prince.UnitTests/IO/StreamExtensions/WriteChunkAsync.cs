// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

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

public class WriteChunkAsync
{
    [Fact]
    public Task Should_throw_on_null_input_stream()
    {
        // arrange
        IStream stream = null!;

        // act
        Func<Task> act = () => stream.WriteChunkAsync(null!);

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_not_writeable_input_stream()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.Setup(s => s.CanWrite).Returns(false);
        var stream = streamMock.Object;

        // act
        Func<Task> act = () => stream.WriteChunkAsync(null!);

        // assert
        return act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public Task Should_throw_on_null_tag()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.Setup(s => s.CanWrite).Returns(true);
        var stream = streamMock.Object;

        // act
        Func<Task> act = () => stream.WriteChunkAsync(null!);

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_empty_tag()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.Setup(s => s.CanWrite).Returns(true);
        var stream = streamMock.Object;

        // act
        Func<Task> act = () => stream.WriteChunkAsync(string.Empty);

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_whitespace_tag()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.Setup(s => s.CanWrite).Returns(true);
        var stream = streamMock.Object;

        // act
        Func<Task> act = () => stream.WriteChunkAsync("  ");

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_too_short_tag()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.Setup(s => s.CanWrite).Returns(true);
        var stream = streamMock.Object;

        // act
        Func<Task> act = () => stream.WriteChunkAsync("aa");

        // assert
        return act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public Task Should_throw_on_too_long_tag()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.Setup(s => s.CanWrite).Returns(true);
        var stream = streamMock.Object;

        // act
        Func<Task> act = () => stream.WriteChunkAsync("aaaa");

        // assert
        return act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public Task Should_throw_on_null_data_bytes()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.Setup(s => s.CanWrite).Returns(true);
        var stream = streamMock.Object;

        // act
        Func<Task> act = () => stream.WriteChunkAsync("Tag", (byte[])null!);

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_null_data_stream()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.Setup(s => s.CanWrite).Returns(true);
        var stream = streamMock.Object;

        // act
        Func<Task> act = () => stream.WriteChunkAsync("Tag", (IStream)null!);

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_not_readable_data_stream()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.Setup(s => s.CanWrite).Returns(true);
        var stream = streamMock.Object;

        var dataStreamMock = new Mock<IStream>(MockBehavior.Strict);
        dataStreamMock.Setup(s => s.CanRead).Returns(false);
        var dataStream = dataStreamMock.Object;

        // act
        Func<Task> act = () => stream.WriteChunkAsync("Tag", dataStream);

        // assert
        return act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Should_write_tag_only_chunk_to_stream()
    {
        // arrange
        var stream = new StreamAdapter(new MemoryStream());
        var dataStream = new StreamAdapter(Stream.Null);

        // act
        await stream.WriteChunkAsync("Tag", dataStream);

        // assert
        stream.Position = 0;
        var buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);

        buffer.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("Tag 0\n\n"));
    }

    [Fact]
    public async Task Should_write_tag_and_string_chunk_to_stream()
    {
        // arrange
        var stream = new StreamAdapter(new MemoryStream());

        // act
        await stream.WriteChunkAsync("Tag", "Test");

        // assert
        stream.Position = 0;
        var buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);

        buffer.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("Tag 4\nTest\n"));
    }

    [Fact]
    public async Task Should_write_tag_and_data_chunk_to_stream()
    {
        // arrange
        var stream = new StreamAdapter(new MemoryStream());
        var data = Encoding.ASCII.GetBytes("This is a test chunk.");

        // act
        await stream.WriteChunkAsync("Tag", data);

        // assert
        stream.Position = 0;
        var buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);

        buffer.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("Tag 21\nThis is a test chunk.\n"));
    }
}
