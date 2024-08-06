// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using RazorLight.Razor;

namespace HtmlGeneration.RazorLight.Razor;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Thinktecture.IO;

public class StreamProject : RazorLightProject, IPrefillProject
{
    private readonly ILogger<StreamProject>? _logger;
    private readonly IDictionary<string, StreamRazorLightProjectItem> _projectItems;

    public StreamProject(ILogger<StreamProject>? logger)
    {
        _logger = logger;
        _projectItems = new ConcurrentDictionary<string, StreamRazorLightProjectItem>();
    }

    public override Task<IEnumerable<RazorLightProjectItem>> GetImportsAsync(string templateKey)
    {
        if (templateKey == null)
            throw new ArgumentNullException(nameof(templateKey));

        _logger?.LogTrace($"{{Function}}({{{nameof(templateKey)}}})", nameof(GetImportsAsync), templateKey);

        return Task.FromResult(Enumerable.Empty<RazorLightProjectItem>());
    }

    public override Task<RazorLightProjectItem?> GetItemAsync(string templateKey)
    {
        if (templateKey == null)
            throw new ArgumentNullException(nameof(templateKey));

        _logger?.LogTrace($"{{Function}}({{{nameof(templateKey)}}})", nameof(GetItemAsync), templateKey);

        return Task.FromResult<RazorLightProjectItem?>(
            _projectItems.TryGetValue(templateKey, out var item)
                ? item
                : null
        );
    }

    public void AddProjectItem(string templateKey, IStream templateStream)
    {
        if (templateKey == null)
            throw new ArgumentNullException(nameof(templateKey));
        if (templateStream == null)
            throw new ArgumentNullException(nameof(templateStream));

        _logger?.LogTrace($"{{Function}}({{{nameof(templateKey)}}})", nameof(AddProjectItem), templateKey);

        if (_projectItems.ContainsKey(templateKey))
            throw new InvalidOperationException("Cannot add a new project item with the same key as an already existing item.");

        _projectItems.Add(templateKey, new StreamRazorLightProjectItem(templateKey, templateStream));
    }

    public bool ContainsTemplate(string templateKey)
    {
        if (templateKey == null)
            throw new ArgumentNullException(nameof(templateKey));

        _logger?.LogTrace($"{{Function}}({{{nameof(templateKey)}}})", nameof(ContainsTemplate), templateKey);

        return _projectItems.ContainsKey(templateKey);
    }
}
