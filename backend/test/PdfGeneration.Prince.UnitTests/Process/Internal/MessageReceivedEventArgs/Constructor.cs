// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentAssertions;
using Xunit;
using SUT = PdfGeneration.Prince.Process.Internal.MessageReceivedEventArgs;

namespace PdfGeneration.Prince.UnitTests.Process.Internal.MessageReceivedEventArgs;

public class Constructor
{
    [Fact]
    public void Should_not_throw_on_null_values()
    {
        // act
        var act = () =>
        {
            var unused = new SUT(null!, null!, null!);
        };

        // assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Should_initialize_type()
    {
        // act
        var result = new SUT("type", null!, null!);

        // assert
        result.Type.Should().Be("type");
    }

    [Fact]
    public void Should_initialize_location()
    {
        // act
        var result = new SUT(null!, "location", null!);

        // assert
        result.Location.Should().Be("location");
    }

    [Fact]
    public void Should_initialize_text()
    {
        // act
        var result = new SUT(null!, null!, "text");

        // assert
        result.Text.Should().Be("text");
    }
}
