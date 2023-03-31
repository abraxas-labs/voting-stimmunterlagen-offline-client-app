using System;
using FluentAssertions;
using Xunit;
using PdfGeneration.Prince.Job;

namespace PdfGeneration.Prince.UnitTests.Job.InputExtensions;

public class EnsureBaseUrl
{
    [Fact]
    public void Should_throw_on_missing_stream()
    {
        // arrange
        Input input = null!;

        // act
        var act = () => input.EnsureBaseUrl(null!);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }


    [Fact]
    public void Should_throw_on_missing_baseUrl()
    {
        // arrange
        var input = new Input();

        // act
        var act = () => input.EnsureBaseUrl(null!);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_not_change_existing_value()
    {
        // arrange
        var input = new Input { BaseUrl = "Test" };

        // act
        input.EnsureBaseUrl("Foo");

        // assert
        input.BaseUrl.Should().Be("Test");
    }

    [Fact]
    public void Should_not_ensure_value()
    {
        // arrange
        var input = new Input();

        // act
        input.EnsureBaseUrl("Foo");

        // assert
        input.BaseUrl.Should().Be("Foo");
    }
}
