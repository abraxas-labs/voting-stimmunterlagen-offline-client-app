using System;
using FluentAssertions;
using Moq;
using Thinktecture.IO;
using Xunit;
using SUT = PdfGeneration.Prince.Process.Chunk;

namespace PdfGeneration.Prince.UnitTests.Process.Chunk;

public class Constructor
{
    [Fact]
    public void Should_throw_on_null_tag()
    {
        // act
        var act = () =>
        {
            var unused = new SUT(null!, 0, null!);
        };

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_throw_on_emtpy_tag()
    {
        // act
        var act = () =>
        {
            var unused = new SUT(string.Empty, 0, null!);
        };

        // assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Should_throw_on_whitespace_tag()
    {
        // act
        var act = () =>
        {
            var unused = new SUT("   ", 0, null!);
        };

        // assert
        act.Should().Throw<ArgumentException>();
    }


    [Fact]
    public void Should_throw_on_too_short_tag()
    {
        // act
        var act = () =>
        {
            var unused = new SUT("Ta", 0, null!);
        };

        // assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Should_throw_on_too_long_tag()
    {
        // act
        var act = () =>
        {
            var unused = new SUT("Tags", 0, null!);
        };

        // assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Should_throw_on_negative_length()
    {
        // act
        var act = () =>
        {
            var unused = new SUT("Tag", -1, null!);
        };

        // assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Should_throw_on_null_data()
    {
        // act
        var act = () =>
        {
            var unused = new SUT("Tag", 0, null!);
        };

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_not_reset_not_seekable_stream()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.SetupGet(s => s.CanSeek).Returns(false);
        var stream = streamMock.Object;

        // act
        var unused = new SUT("Tag", 0, stream);

        // assert
        streamMock.Verify();
    }

    [Fact]
    public void Should_not_resetseekable_stream()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.SetupGet(s => s.CanSeek).Returns(true);
        streamMock.SetupSet(s => s.Position = 0);
        var stream = streamMock.Object;

        // act
        var unused = new SUT("Tag", 0, stream);

        // assert
        streamMock.Verify();
    }
}
