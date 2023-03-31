using System;
using FluentAssertions;
using Thinktecture.IO.Adapters;
using Xunit;
using SUT = PdfGeneration.Prince.Process.Internal.PrinceConfigurationBuilder;

namespace PdfGeneration.Prince.UnitTests.Process.Internal.PrinceConfigurationBuilder;

public class Constructor
{
    [Fact]
    public void Should_throw_on_missing_path()
    {
        // act
        var act = () =>
        {
            var ununsed = new SUT(null!, null!);
        };

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_throw_on_missing_directory()
    {
        // act
        var act = () =>
        {
            var ununsed = new SUT(new PathAdapter(), null!);
        };

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_not_throw_on_passed_tools()
    {
        // act
        var act = () =>
        {
            var unused = new SUT(new PathAdapter(), new DirectoryAdapter());
        };

        // assert
        act.Should().NotThrow();
    }
}
