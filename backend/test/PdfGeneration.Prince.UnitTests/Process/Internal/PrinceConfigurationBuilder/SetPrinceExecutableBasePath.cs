using System;
using FluentAssertions;
using Moq;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;
using Xunit;
using SUT = PdfGeneration.Prince.Process.Internal.PrinceConfigurationBuilder;

namespace PdfGeneration.Prince.UnitTests.Process.Internal.PrinceConfigurationBuilder;

public class SetPrinceExecutableBasePath
{
    [Fact]
    public void Should_throw_on_null_path()
    {
        // arrange
        var sut = new SUT(new PathAdapter(), new DirectoryAdapter());

        // act
        Action act = () => sut.SetPrinceExecutableBasePath(null!);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_throw_on_empty_path()
    {
        // arrange
        var sut = new SUT(new PathAdapter(), new DirectoryAdapter());

        // act
        Action act = () => sut.SetPrinceExecutableBasePath(string.Empty);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_throw_on_whitespace_path()
    {
        // arrange
        var sut = new SUT(new PathAdapter(), new DirectoryAdapter());

        // act
        Action act = () => sut.SetPrinceExecutableBasePath(" ");

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_throw_on_invalid_path()
    {
        // arrange
        var directoryMock = new Mock<IDirectory>(MockBehavior.Strict);
        directoryMock.Setup(d => d.Exists("invalid")).Returns(false);
        var directory = directoryMock.Object;

        var sut = new SUT(new PathAdapter(), directory);

        // act
        Action act = () => sut.SetPrinceExecutableBasePath("invalid");

        // assert
        act.Should().Throw<ArgumentException>();
        directoryMock.Verify();
    }

    [Fact]
    public void Should_set_value_on_valid_path()
    {
        // arrange
        var directoryMock = new Mock<IDirectory>(MockBehavior.Strict);
        directoryMock.Setup(d => d.Exists("valid")).Returns(true);
        var directory = directoryMock.Object;

        var sut = new SUT(new PathAdapter(), directory);

        // act
        sut.SetPrinceExecutableBasePath("valid");

        // assert
        directoryMock.Verify();
        var config = sut.BuildConfiguration();
        config.PrinceExecutableBasePath.Should().Be("valid");
    }
}
