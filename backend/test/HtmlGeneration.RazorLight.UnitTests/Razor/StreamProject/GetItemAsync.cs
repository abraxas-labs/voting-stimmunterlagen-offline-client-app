using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Thinktecture.IO;
using Xunit;

namespace HtmlGeneration.RazorLight.UnitTests.Razor.StreamProject;

public class GetItemAsync
{
    [Fact]
    public void Should_throw_on_null_templateKey()
    {
        // arrange
        var sut = new RazorLight.Razor.StreamProject(null);

        Action act = () => sut.GetItemAsync(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task Should_return_nothing_when_template_is_missing()
    {
        // arrange
        var sut = new RazorLight.Razor.StreamProject(null);

        var result = await sut.GetItemAsync(string.Empty);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Should_return_template_by_key()
    {
        // arrange
        var innerStream = new MemoryStream();

        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.SetupGet(s => s.CanRead).Returns(true);
        streamMock.Setup(s => s.UnsafeConvert()).Returns(innerStream);

        var sut = new RazorLight.Razor.StreamProject(null);
        sut.AddProjectItem("test", streamMock.Object);

        // act
        var result = await sut.GetItemAsync("test");

        // assert
        result.Should().NotBeNull();
        result!.Key.Should().Be("test");
        result.Exists.Should().BeTrue();
        result.Read().Should().BeSameAs(innerStream);
    }
}
