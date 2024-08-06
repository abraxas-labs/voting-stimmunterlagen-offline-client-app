// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentAssertions;
using Xunit;

namespace HtmlGeneration.RazorLight.UnitTests.Razor.StreamProject;

public class Constructor
{
    [Fact]
    public void Should_not_throw_on_empty_logger()
    {
        // act
        var act = () =>
        {
            var unused = new RazorLight.Razor.StreamProject(null);
        };

        // assert
        act.Should().NotThrow();
    }
}
