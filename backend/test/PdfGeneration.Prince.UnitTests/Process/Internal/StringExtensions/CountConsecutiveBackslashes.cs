using System;
using FluentAssertions;
using Xunit;
using PdfGeneration.Prince.Process.Internal;

namespace PdfGeneration.Prince.UnitTests.Process.Internal.StringExtensions;

public class CountConsecutiveBackslashes
{
    [Fact]
    public void Should_throw_on_null_argument()
    {
        // arrange
        string argument = null!;

        // act
        var act = () =>
        {
            var unused = argument.CountConsecutiveBackslashes();
        };

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_count_nothing_on_empty_string()
    {
        // arrange
        var argument = string.Empty;

        // act
        var result = argument.CountConsecutiveBackslashes();

        // assert
        result.Should().Be(0);
    }

    [Fact]
    public void Should_count_nothing_on_whitespace_string()
    {
        // arrange
        var argument = "     ";

        // act
        var result = argument.CountConsecutiveBackslashes();

        // assert
        result.Should().Be(0);
    }

    [Fact]
    public void Should_count_one_backslash_at_end()
    {
        // arrange
        var argument = "     \\";

        // act
        var result = argument.CountConsecutiveBackslashes();

        // assert
        result.Should().Be(1);
    }

    [Fact]
    public void Should_count_multiple_backslashes_at_end()
    {
        // arrange
        var argument = "     \\\\\\\\";

        // act
        var result = argument.CountConsecutiveBackslashes();

        // assert
        result.Should().Be(4);
    }

    [Fact]
    public void Should_not_count_over_break_in_backslashes()
    {
        // arrange
        var argument = "    \\ \\\\\\\\";

        // act
        var result = argument.CountConsecutiveBackslashes();

        // assert
        result.Should().Be(4);
    }

    [Fact]
    public void Should_count_correctly_in_small_range()
    {
        // arrange
        var argument = "    \\ \\\\\\\\     ";

        // act
        var result = argument.CountConsecutiveBackslashes(9, 8);

        // assert
        result.Should().Be(2);
    }

    [Fact]
    public void Should_count_correctly_in_open_range()
    {
        // arrange
        var argument = "    \\ \\\\\\\\     ";

        // act
        var result = argument.CountConsecutiveBackslashes(9);

        // assert
        result.Should().Be(4);
    }

    [Fact]
    public void Should_count_correctly_start_range()
    {
        // arrange
        var argument = "    \\ \\\\\\\\     ";

        // act
        var result = argument.CountConsecutiveBackslashes(10);

        // assert
        result.Should().Be(0);
    }
}
