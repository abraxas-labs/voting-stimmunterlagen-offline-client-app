using System;
using FluentAssertions;
using Xunit;
using PdfGeneration.Prince.Process.Internal;

namespace PdfGeneration.Prince.UnitTests.Process.Internal.StringExtensions;

public class EscapeDoubleQuotes
{
    [Fact]
    public void Should_throw_on_null_argument()
    {
        // arrange
        string argument = null!;

        // act
        var act = () =>
        {
            var unused = argument.EscapeDoubleQuotes();
        };

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_do_nothing_on_empty_string()
    {
        // arrange
        var argument = string.Empty;

        // act
        var result = argument.EscapeDoubleQuotes();

        // assert
        result.Should().Be(argument);
    }

    [Fact]
    public void Should_do_nothing_on_whitespace_string()
    {
        // arrange
        var argument = "     ";

        // act
        var result = argument.EscapeDoubleQuotes();

        // assert
        result.Should().Be(argument);
    }

    [Fact]
    public void Should_escape_doublequote_at_end_with_backslash()
    {
        // arrange
        var argument = "something \"";

        // act
        var result = argument.EscapeDoubleQuotes();

        // assert
        result.Should().Be("something \\\"");
    }

    [Fact]
    public void Should_escape_doublequote_at_start_with_backslash()
    {
        // arrange
        var argument = "\" something";

        // act
        var result = argument.EscapeDoubleQuotes();

        // assert
        result.Should().Be("\\\" something");
    }

    [Fact]
    public void Should_escape_multiple_doublequotes_with_backslash()
    {
        // arrange
        var argument = "\" some\"thing\"\"";

        // act
        var result = argument.EscapeDoubleQuotes();

        // assert
        result.Should().Be("\\\" some\\\"thing\\\"\\\"");
    }

    [Fact]
    public void Should_escape_backslashes_before_doublequote()
    {
        // arrange
        var argument = "something \\\" something";

        // act
        var result = argument.EscapeDoubleQuotes();

        // assert
        result.Should().Be("something \\\\\\\" something");
    }

    [Fact]
    public void Should_not_touch_spaced_spaced_backslash_before_quote()
    {
        // arrange
        var argument = "something \\ \" something";

        // act
        var result = argument.EscapeDoubleQuotes();

        // assert
        result.Should().Be("something \\ \\\" something");
    }

    [Fact]
    public void Should_stop_at_pause_in_backslashes()
    {
        // arrange
        var argument = "something \\/\\\\\\\" something";

        // act
        var result = argument.EscapeDoubleQuotes();

        // assert
        result.Should().Be("something \\/\\\\\\\\\\\\\\\" something");
    }
}
