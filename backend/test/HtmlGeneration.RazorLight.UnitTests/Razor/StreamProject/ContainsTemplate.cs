// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using FluentAssertions;
using Moq;
using Thinktecture.IO;
using Xunit;

namespace HtmlGeneration.RazorLight.UnitTests.Razor.StreamProject;

public class ContainsTemplate
{
    [Fact]
    public void Should_throw_on_null_templateKey()
    {
        // arrange
        var sut = new RazorLight.Razor.StreamProject(null);

        // act
        Action act = () => sut.ContainsTemplate(null!);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_return_false_when_template_is_missing()
    {
        // arrange
        var sut = new RazorLight.Razor.StreamProject(null);

        // act
        var result = sut.ContainsTemplate(string.Empty);

        // assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Should_return_true_when_template_exists()
    {
        // arrange
        var innerStream = new MemoryStream();

        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.SetupGet(s => s.CanRead).Returns(true);
        streamMock.Setup(s => s.UnsafeConvert()).Returns(innerStream);

        var sut = new RazorLight.Razor.StreamProject(null);
        sut.AddProjectItem("test", streamMock.Object);

        // act
        var result = sut.ContainsTemplate("test");

        // assert
        result.Should().BeTrue();
    }
}
