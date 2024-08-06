// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using FluentAssertions;
using Moq;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;
using Xunit;
using SUT = PdfGeneration.Prince.Process.Internal.PrinceConfigurationBuilder;

namespace PdfGeneration.Prince.UnitTests.Process.Internal.PrinceConfigurationBuilder;

public class SetLogFileLocation
{
    [Fact]
    public void Should_throw_on_null_path()
    {
        // arrange
        var sut = new SUT(new PathAdapter(), new DirectoryAdapter());

        // act
        Action act = () => sut.SetLogFileLocation(null!);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_throw_on_empty_path()
    {
        // arrange
        var sut = new SUT(new PathAdapter(), new DirectoryAdapter());

        // act
        Action act = () => sut.SetLogFileLocation(string.Empty);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_throw_on_whitespace_path()
    {
        // arrange
        var sut = new SUT(new PathAdapter(), new DirectoryAdapter());

        // act
        Action act = () => sut.SetLogFileLocation(" ");

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_throw_on_invalid_path()
    {
        // arrange
        var pathMock = new Mock<IPath>(MockBehavior.Strict);
        pathMock.Setup(p => p.GetDirectoryName("invalid/invalid")).Returns("invalid");
        var path = pathMock.Object;

        var directoryMock = new Mock<IDirectory>(MockBehavior.Strict);
        directoryMock.Setup(d => d.Exists("invalid")).Returns(false);
        var directory = directoryMock.Object;

        var sut = new SUT(path, directory);

        // act
        Action act = () => sut.SetLogFileLocation("invalid/invalid");

        // assert
        act.Should().Throw<ArgumentException>();
        directoryMock.Verify();
    }

    [Fact]
    public void Should_set_value_on_valid_path()
    {
        // arrange
        var pathMock = new Mock<IPath>(MockBehavior.Strict);
        pathMock.Setup(p => p.GetDirectoryName("valid/valid")).Returns("valid");
        var path = pathMock.Object;

        var directoryMock = new Mock<IDirectory>(MockBehavior.Strict);
        directoryMock.Setup(d => d.Exists("valid")).Returns(true);
        var directory = directoryMock.Object;

        var sut = new SUT(path, directory);

        // act
        sut.SetLogFileLocation("valid/valid");

        // assert
        directoryMock.Verify();
        var config = sut.BuildConfiguration();
        config.LogFileLocation.Should().Be("valid/valid");
    }
}
