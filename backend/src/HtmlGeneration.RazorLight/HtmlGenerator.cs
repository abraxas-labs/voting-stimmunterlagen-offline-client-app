// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using HtmlGeneration.RazorLight.Razor;
using Microsoft.Extensions.Logging;
using RazorLight;
using Thinktecture.IO;

namespace HtmlGeneration.RazorLight;

public class HtmlGenerator : IHtmlGenerator
{
    private readonly ILogger<HtmlGenerator>? _logger;
    private readonly IRazorLightEngine _engine;
    private readonly IPrefillProject _project;

    public HtmlGenerator(ILogger<HtmlGenerator>? logger, IRazorLightEngine engine, IPrefillProject project)
    {
        _logger = logger;
        _engine = engine ?? throw new ArgumentNullException(nameof(engine));
        _project = project ?? throw new ArgumentNullException(nameof(project));
    }

    public Task RenderHtmlAsync<T>(string templateKey, IStream templateStream, T model, IStream outStream)
    {
        if (templateKey == null)
            throw new ArgumentNullException(nameof(templateKey));
        if (templateStream == null)
            throw new ArgumentNullException(nameof(templateStream));
        if (outStream == null)
            throw new ArgumentNullException(nameof(outStream));

        _logger?.LogTrace($"{{Function}}({{{nameof(templateKey)}}}, {{{nameof(model)}}})", nameof(RenderHtmlAsync), templateKey, model?.GetType().Name ?? "null");

        // if we get a template stream that's not already in the project system make sure we add it
        if (!_project.ContainsTemplate(templateKey))
        {
            _project.AddProjectItem(templateKey, templateStream);
        }

        return RenderHtmlAsync(templateKey, model, outStream);
    }

    public Task RenderHtmlAsync<T>(string templateKey, T model, IStream outStream)
    {
        if (templateKey == null)
            throw new ArgumentNullException(nameof(templateKey));
        if (outStream == null)
            throw new ArgumentNullException(nameof(outStream));

        _logger?.LogTrace($"{{Function}}({{{nameof(templateKey)}}}, {{{nameof(model)}}})", nameof(RenderHtmlAsync), templateKey, model?.GetType().Name ?? "null");

        if (!_project.ContainsTemplate(templateKey))
            throw new ArgumentException("The specified template is not available for rendering.", nameof(templateKey));

        return _engine.CompileAndRenderTemplateAsync(templateKey, model, outStream);
    }
}
