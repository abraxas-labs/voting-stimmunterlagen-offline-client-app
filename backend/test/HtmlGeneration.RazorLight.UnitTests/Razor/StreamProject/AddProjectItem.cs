// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Thinktecture.IO;
using Xunit;

namespace HtmlGeneration.RazorLight.UnitTests.Razor.StreamProject;

public class AddProjectItem
{
    [Fact]
    public void Should_throw_on_null_templateKey()
    {
        // arrange
        var sut = new RazorLight.Razor.StreamProject(null);

        // act
        var act = () => sut.AddProjectItem(null!, null!);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_throw_on_null_stream()
    {
        // arrange
        var sut = new RazorLight.Razor.StreamProject(null);

        // act
        var act = () => sut.AddProjectItem("test", null!);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task Should_add_template()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.SetupGet(s => s.CanRead).Returns(true);

        var sut = new RazorLight.Razor.StreamProject(null!);
        sut.AddProjectItem("test", streamMock.Object);

        // act
        var result = await sut.GetItemAsync("test");

        // assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Should_throw_on_duplicate_templatey_key()
    {
        // arrange
        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.SetupGet(s => s.CanRead).Returns(true);

        var sut = new RazorLight.Razor.StreamProject(null);
        sut.AddProjectItem("test", streamMock.Object);

        // act
        var act = () => sut.AddProjectItem("test", streamMock.Object);

        // assert
        act.Should().Throw<InvalidOperationException>();
    }
}
