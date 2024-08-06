// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using FluentAssertions;
using Xunit;
using PdfGeneration.Prince.Process.Internal;

namespace PdfGeneration.Prince.UnitTests.Process.Internal.StringExtensions;

public class EscapeEndingBackslashes
{
    [Fact]
    public void Should_throw_on_null_argument()
    {
        // arrange
        string argument = null!;

        // act
        var act = () =>
        {
            var unused = argument.EscapeEndingBackslashes();
        };

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_not_alter_string_without_ending_backslash()
    {
        // arrange
        var argument = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+¡²³¤€¼½¾‘’¥×äÄöÖüÜ[]{}«»;'\\:\"|¶´¬,./<>?ç¿éµ\\ ";

        // act
        var result = argument.EscapeEndingBackslashes();

        // assert
        result.Should().Be(argument);
    }

    [Fact]
    public void Should_escape_trailing_backslash()
    {
        // arrange
        var argument = ".\\";

        // act
        var result = argument.EscapeEndingBackslashes();

        // assert
        result.Should().Be(".\\\\");
    }

    [Fact]
    public void Should_escape_multiple_trailing_backslashes()
    {
        // arrange
        var argument = ".\\\\\\\\\\";

        // act
        var result = argument.EscapeEndingBackslashes();

        // assert
        result.Should().Be(".\\\\\\\\\\\\\\\\\\\\");
    }

    [Fact]
    public void Should_stop_on_first_non_backslash_at_end()
    {
        // arrange
        var argument = ".\\\\\\\\ \\";

        // act
        var result = argument.EscapeEndingBackslashes();

        // assert
        result.Should().Be(".\\\\\\\\ \\\\");
    }

}
