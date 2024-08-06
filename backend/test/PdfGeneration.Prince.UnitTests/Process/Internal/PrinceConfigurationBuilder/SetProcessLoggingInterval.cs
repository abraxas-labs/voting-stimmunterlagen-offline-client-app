// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using FluentAssertions;
using Thinktecture.IO.Adapters;
using Xunit;
using SUT = PdfGeneration.Prince.Process.Internal.PrinceConfigurationBuilder;

namespace PdfGeneration.Prince.UnitTests.Process.Internal.PrinceConfigurationBuilder;

public class SetProcessLoggingInterval
{
    [Fact]
    public void Should_throw_on_zero_timespan()
    {
        // arrange
        var sut = new SUT(new PathAdapter(), new DirectoryAdapter());

        // act
        Action act = () => sut.SetProcessLoggingInterval(TimeSpan.Zero);

        // assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Should_throw_on_negative_timespan()
    {
        // arrange
        var sut = new SUT(new PathAdapter(), new DirectoryAdapter());

        // act
        Action act = () => sut.SetProcessLoggingInterval(TimeSpan.FromSeconds(-1));

        // assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Should_set_value_on_positive_timespan()
    {
        // arrange
        var sut = new SUT(new PathAdapter(), new DirectoryAdapter());

        // act
        sut.SetProcessLoggingInterval(TimeSpan.FromSeconds(1));

        // assert
        var config = sut.BuildConfiguration();
        config.ProcessLoggingInterval.Should().Be(TimeSpan.FromSeconds(1));
    }
}
