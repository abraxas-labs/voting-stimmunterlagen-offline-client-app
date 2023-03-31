using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace HtmlGeneration.RazorLight.UnitTests.Razor.StreamProject;

public class GetImportsAsync
{
    [Fact]
    public void Should_throw_on_null_templateKey()
    {
        // arrange
        var sut = new RazorLight.Razor.StreamProject(null);

        // act
        Action act = () => sut.GetImportsAsync(null!);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task Should_return_empty_collection()
    {
        // arrange
        var sut = new RazorLight.Razor.StreamProject(null);

        // act
        var result = await sut.GetImportsAsync(string.Empty);

        // assert
        result.Should().BeEmpty();
    }
}
