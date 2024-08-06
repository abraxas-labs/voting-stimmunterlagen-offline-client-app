// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using FluentAssertions;
using Moq;
using Thinktecture.IO;
using Xunit;
using PdfGeneration.Prince.Process;
using PdfGeneration.Prince.Process.Internal;

namespace PdfGeneration.Prince.UnitTests.Process.Internal.PrinceConfigurationExtensions;

public class GetPrinceStartArguments
{
    [Fact]
    public void Should_throw_on_missing_config()
    {
        // arrange
        IPrinceConfiguration config = null!;

        // act
        Action act = () => config.GetPrinceStartArguments();

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_have_nothing_as_defaults()
    {
        // arrange
        var config = new PrinceConfiguration();

        // act
        var args = config.GetPrinceStartArguments();

        // assert
        args.Should().BeEmpty();
    }

    [Fact]
    public void Should_add_nothing()
    {
        // arrange
        var config = BuildConfig();

        // act
        var args = config.GetPrinceStartArguments();

        // assert
        args.Should().BeEmpty();
    }

    [Fact]
    public void Should_add_debug_if_specified()
    {
        // arrange
        var config = BuildConfig(b => b.SetDebug(true));

        // act
        var args = config.GetPrinceStartArguments();

        // assert
        args.Should().Contain("--debug ");
    }

    [Fact]
    public void Should_add_log_if_specified()
    {
        // arrange
        var config = BuildConfig(b => b.SetLogFileLocation("C:\\Logs\\log.txt"));

        // act
        var args = config.GetPrinceStartArguments();

        // assert
        args.Should().Contain("--log=\"C:\\Logs\\log.txt\" ");
    }

    [Fact]
    public void Should_add_verbose_if_specified()
    {
        // arrange
        var config = BuildConfig(b => b.SetVerbose(true));

        // act
        var args = config.GetPrinceStartArguments();

        // assert
        args.Should().Contain("--verbose ");
    }

    private static IPrinceConfiguration BuildConfig(Action<IPrinceConfigurationBuilder>? configBuilder = null)
    {
        // set up the mocks so that they return true for all arguments
        var pathMock = new Mock<IPath>();
        pathMock.Setup(p => p.GetDirectoryName(It.IsAny<string>())).Returns<string>(arg1 => arg1);
        var path = pathMock.Object;

        var directoryMock = new Mock<IDirectory>();
        directoryMock.Setup(d => d.Exists(It.IsAny<string>())).Returns(true);
        var directory = directoryMock.Object;

        var builder = new Prince.Process.Internal.PrinceConfigurationBuilder(path, directory);

        configBuilder?.Invoke(builder);

        return builder.BuildConfiguration();
    }
}
