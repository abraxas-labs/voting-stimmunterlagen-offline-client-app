using System;
using FluentAssertions;
using HtmlGeneration.RazorLight.Razor;
using Moq;
using RazorLight;
using Xunit;

namespace HtmlGeneration.RazorLight.UnitTests.RazorLightEngineBuilderExtensions;

public class UseStreamProject
{
    [Fact]
    public void Should_throw_on_null_this()
    {
        // act
        Action act = () => ((RazorLightEngineBuilder)null!).UseStreamProject(null!);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_throw_on_null_project()
    {
        // arrange
        var engineBuilderMock = new Mock<RazorLightEngineBuilder>(MockBehavior.Strict);
        var sut = engineBuilderMock.Object;

        // act
        Action act = () => sut.UseStreamProject(null!);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_add_project_to_engine_builder()
    {
        // arrange
        var projectMock = new Mock<StreamProject>(MockBehavior.Strict, null);
        var streamProject = projectMock.Object;

        var engineBuilderMock = new Mock<RazorLightEngineBuilder>();
        engineBuilderMock.Setup(eb => eb.UseProject(It.Is<StreamProject>(a => a == streamProject))).Returns(engineBuilderMock.Object);
        var sut = engineBuilderMock.Object;

        // act
        var engineBuilder = sut.UseStreamProject(streamProject);

        // assert
        engineBuilderMock.Verify();
        engineBuilder.Should().Be(sut);
    }
}
