using System;
using System.IO;
using FluentAssertions;
using HtmlGeneration.RazorLight.Razor;
using Moq;
using Thinktecture.IO;
using Xunit;

namespace HtmlGeneration.RazorLight.UnitTests.Razor.StreamRazorProjectItem;

public class Constructor
{
    [Fact]
    public void Should_throw_when_called_with_null_key()
    {
        // act
        var act = () =>
        {
            var unused = new StreamRazorLightProjectItem(null!, null!);
        };

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_throw_when_called_with_null_stream()
    {
        // act
        var act = () =>
        {
            var unused = new StreamRazorLightProjectItem(string.Empty, null!);
        };

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_throw_when_called_with_not_readable_stream()
    {
        // arrange
        var streamMock = new Mock<IStream>();
        streamMock.SetupGet(s => s.CanRead).Returns(false);
        var stream = streamMock.Object;

        // act
        var act = () =>
        {
            var unused = new StreamRazorLightProjectItem(string.Empty, stream);
        };

        // assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Should_create_object()
    {
        // arrange
        var innerStream = new Mock<Stream>().Object;

        var streamMock = new Mock<IStream>();
        streamMock.SetupGet(s => s.CanRead).Returns(true);
        streamMock.Setup(s => s.UnsafeConvert()).Returns(innerStream);
        var stream = streamMock.Object;

        // act
        var projectItem = new StreamRazorLightProjectItem("Test", stream);

        // assert
        projectItem.Key.Should().Be("Test");
        projectItem.Exists.Should().BeTrue();
        projectItem.Read().Should().BeSameAs(innerStream);
    }
}
