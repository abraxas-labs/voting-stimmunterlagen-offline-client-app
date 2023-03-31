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

public class CompileAndRenderTemplateAsync
{
    [Fact]
    public Task Should_throw_on_null_object()
    {
        // act
        var act = async () => await ((IRazorLightEngine)null!).CompileAndRenderTemplateAsync<object>(null!, null!, null!);

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_null_template()
    {
        // arrange
        var sut = new Mock<IRazorLightEngine>(MockBehavior.Strict).Object;

        // act
        var act = async () => await sut.CompileAndRenderTemplateAsync<object>(null!, null!, null!);

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_null_stream()
    {
        // arrange
        var sut = new Mock<IRazorLightEngine>(MockBehavior.Strict).Object;

        // act
        var act = async () => await sut.CompileAndRenderTemplateAsync<object>(string.Empty, null!, null!);

        // assert
        return act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public Task Should_throw_on_not_writeable_stream()
    {
        // arrange
        var sut = new Mock<IRazorLightEngine>(MockBehavior.Strict).Object;

        var streamMock = new Mock<IStream>(MockBehavior.Strict);
        streamMock.SetupGet(s => s.CanWrite).Returns(false);
        var stream = streamMock.Object;

        // act
        Func<Task> act = async () => await sut.CompileAndRenderTemplateAsync<object>(string.Empty, null!, stream);

        // assert
        return act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Should_call_through_to_engine()
    {
        // arrange
        var template = new Mock<ITemplatePage>(MockBehavior.Strict).Object;

        // inner stream will be used by StreamWriter, so no strict here, only tell we are writable
        var innerStreamMock = new Mock<Stream>();
        innerStreamMock.SetupGet(s => s.CanWrite).Returns(true);

        var stream = new StreamAdapter(new MemoryStream());

        var engineMock = new Mock<IRazorLightEngine>(MockBehavior.Strict);
        engineMock.Setup(e => e.CompileTemplateAsync(string.Empty)).Returns(Task.FromResult(template));
        engineMock.Setup(e => e.RenderTemplateAsync<object>(template, null!, It.IsAny<TextWriter>(), null)).Returns(Task.CompletedTask);
        var sut = engineMock.Object;

        // act
        await sut.CompileAndRenderTemplateAsync<object>(string.Empty, null!, stream);

        // assert
        engineMock.Verify();
        innerStreamMock.Verify();
    }
}
