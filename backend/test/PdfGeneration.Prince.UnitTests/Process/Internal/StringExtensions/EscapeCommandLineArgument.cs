using System;
using FluentAssertions;
using Xunit;
using PdfGeneration.Prince.Process.Internal;

namespace PdfGeneration.Prince.UnitTests.Process.Internal.StringExtensions;

public class EscapeCommandLineArgument
{
    [Fact]
    public void Should_throw_on_null_argument()
    {
        // arrange
        string argument = null!;

        // act
        var act = () =>
        {
            var unused = argument.EscapeCommandLineArgument();
        };

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_escape_argument()
    {
        // arrange
        var argument = "\\ \\\"  \" \\";

        // act
        var result = argument.EscapeCommandLineArgument();

        // assert
        result.Should().Be("\\ \\\\\\\"  \\\" \\\\");
    }
}
