using FluentAssertions;
using Thinktecture.IO.Adapters;
using Xunit;
using SUT = PdfGeneration.Prince.Process.Internal.PrinceConfigurationBuilder;

namespace PdfGeneration.Prince.UnitTests.Process.Internal.PrinceConfigurationBuilder;

public class SetJavaScript
{
    [Fact]
    public void Should_set_true_value()
    {
        // arrange
        var sut = new SUT(new PathAdapter(), new DirectoryAdapter());

        // act
        sut.SetJavascript(true);

        // assert
        var config = sut.BuildConfiguration();
        config.JavaScript.Should().BeTrue();
    }

    [Fact]
    public void Should_set_false_value()
    {
        // arrange
        var sut = new SUT(new PathAdapter(), new DirectoryAdapter());

        // act
        sut.SetJavascript(false);

        // assert
        var config = sut.BuildConfiguration();
        config.JavaScript.Should().BeFalse();
    }
}
