using FluentAssertions;
using Thinktecture.IO.Adapters;
using Xunit;
using PdfGeneration.Prince.Process.Internal;
using SUT = PdfGeneration.Prince.Process.Internal.PrinceConfigurationBuilder;

namespace PdfGeneration.Prince.UnitTests.Process.Internal.PrinceConfigurationBuilder;

public class BuildConfiguration
{
    [Fact]
    public void Should_return_config()
    {
        // arrange
        var sut = new SUT(new PathAdapter(), new DirectoryAdapter());

        // act
        var config = sut.BuildConfiguration();

        // assert
        config.Should().NotBeNull();
        config.Should().BeOfType<PrinceConfiguration>();
    }
}
