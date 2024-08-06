// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using RazorLight;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;
using Xunit;

namespace HtmlGeneration.RazorLight.UnitTests.RazorLightEngineExtensions;

public class RenderTemplateAsync
{
    [Fact]
    public Task Should_throw_on_null_object()
    {
        // act
        Func<Task> act = () => ((IRazorLightEngine)null!).RenderTemplateAsync<object>(null!, null!, (IStream)null!);

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_null_template()
    {
        // arrange
        var sut = new Mock<IRazorLightEngine>(MockBehavior.Strict).Object;

        // act
        Func<Task> act = () => sut.RenderTemplateAsync<object>(null!, null!, (IStream)null!);

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_null_stream()
    {
        // arrange
        var sut = new Mock<IRazorLightEngine>(MockBehavior.Strict).Object;
        var template = new Mock<ITemplatePage>(MockBehavior.Strict).Object;

        // act
        Func<Task> act = () => sut.RenderTemplateAsync<object>(template, null!, (IStream)null!);

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_not_writeable_stream()
    {
        // arrange
        var sut = new Mock<IRazorLightEngine>(MockBehavior.Strict).Object;
        var template = new Mock<ITemplatePage>(MockBehavior.Strict).Object;

        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.SetupGet(s => s.CanWrite).Returns(false);
        var stream = streamMock.Object;

        // act
        Func<Task> act = () => sut.RenderTemplateAsync<object>(template, null!, stream);

        // assert
        return act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Should_call_through_to_engine()
    {
        // arrange
        var template = new Mock<ITemplatePage>(MockBehavior.Strict).Object;

        var stream = new StreamAdapter(new MemoryStream());

        var engineMock = new Mock<IRazorLightEngine>(MockBehavior.Strict);
        engineMock.Setup(e => e.RenderTemplateAsync<object>(template, null!, It.IsAny<TextWriter>(), null)).Returns(Task.CompletedTask);
        var sut = engineMock.Object;

        // act
        await sut.RenderTemplateAsync<object>(template, null!, stream);

        // assert
        engineMock.Verify();
    }
}
