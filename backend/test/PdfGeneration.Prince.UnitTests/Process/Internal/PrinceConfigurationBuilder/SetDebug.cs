// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentAssertions;
using Thinktecture.IO.Adapters;
using Xunit;
using SUT = PdfGeneration.Prince.Process.Internal.PrinceConfigurationBuilder;

namespace PdfGeneration.Prince.UnitTests.Process.Internal.PrinceConfigurationBuilder;

public class SetDebug
{
    [Fact]
    public void Should_set_true_value()
    {
        // arrange
        var sut = new SUT(new PathAdapter(), new DirectoryAdapter());

        // act
        sut.SetDebug(true);

        // assert
        var config = sut.BuildConfiguration();
        config.Debug.Should().BeTrue();
    }

    [Fact]
    public void Should_set_false_value()
    {
        // arrange
        var sut = new SUT(new PathAdapter(), new DirectoryAdapter());

        // act
        sut.SetDebug(false);

        // assert
        var config = sut.BuildConfiguration();
        config.Debug.Should().BeFalse();
    }
}
