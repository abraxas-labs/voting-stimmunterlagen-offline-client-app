// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using HtmlGeneration.RazorLight.Razor;
using Moq;
using RazorLight;
using RazorLight.Razor;
using Thinktecture.IO.Adapters;
using Xunit;

namespace HtmlGeneration.RazorLight.UnitTests.HtmlGenerator;

public class RenderHtmlAsync
{
    [Fact]
    public Task Should_throw_on_missing_templateKey()
    {
        // arrange
        var sut = CreateGenerator();

        // act
        Func<Task> act = () => sut.RenderHtmlAsync<object>(null!, null!, null!);

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_missing_input_stream()
    {
        // arrange
        var sut = CreateGenerator();

        // act
        Func<Task> act = () => sut.RenderHtmlAsync<object>("TEST", null!, null!, null!);

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_missing_output_stream()
    {
        // arrange
        var sut = CreateGenerator();

        // act
        Func<Task> act = () => sut.RenderHtmlAsync<object>("TEST", null!, null!);

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_missing_template()
    {
        // arrange
        var sut = CreateGenerator();

        // act
        Func<Task> act = () => sut.RenderHtmlAsync<object>("test", null!, new StreamAdapter(new MemoryStream()));

        // assert
        return act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Should_call_renderTemplateAsync()
    {
        // arrange
        var projectMock = new Mock<RazorLightProject>();
        var prefillProjectMock = projectMock.As<IPrefillProject>();
        prefillProjectMock.Setup(p => p.ContainsTemplate("TEST")).Returns(true);

        var templatePage = new Mock<ITemplatePage>().Object;

        var engineMock = new Mock<IRazorLightEngine>(MockBehavior.Strict);
        engineMock.Setup(e => e.CompileTemplateAsync("TEST")).ReturnsAsync(templatePage);
        engineMock.Setup(e => e.RenderTemplateAsync<object>(templatePage, null!, It.IsAny<StreamWriter>(), null)).Returns(Task.CompletedTask);

        var sut = new RazorLight.HtmlGenerator(null, engineMock.Object, prefillProjectMock.Object);

        // act
        await sut.RenderHtmlAsync<object>("TEST", null!, new StreamAdapter(new MemoryStream()));

        // assert
        engineMock.Verify();
    }

    private static RazorLight.HtmlGenerator CreateGenerator()
    {
        var projectMock = new Mock<RazorLightProject>();
        var prefillProjectMock = projectMock.As<IPrefillProject>();
        prefillProjectMock.Setup(p => p.ContainsTemplate("TEST")).Returns(true);

        var engineMock = new Mock<IRazorLightEngine>();
        return new RazorLight.HtmlGenerator(null, engineMock.Object, prefillProjectMock.Object);
    }
}
