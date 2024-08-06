// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace PdfGeneration.Prince;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Thinktecture.IO;
using Job;
using Process;

public class PrincePdfGenerator : IPdfGenerator
{
    private readonly ILogger<PrincePdfGenerator>? _logger;
    private readonly IPrinceProcessManager _processManager;

    private bool _isInitialized;

    public PrincePdfGenerator(ILogger<PrincePdfGenerator>? logger, IPrinceProcessManager processManager)
    {
        _logger = logger;
        _processManager = processManager ?? throw new ArgumentNullException(nameof(processManager));
    }

    public async Task GeneratePdfAsync(IStream htmlStream, IStream outStream, string? resourceBasePath = null)
    {
        await Initialize();

        var job = new JobDefinition();
        job.AddSource(htmlStream);

        _logger?.LogDebug($"{nameof(PrincePdfGenerator)}.{nameof(GeneratePdfAsync)}() Generating pdf from html stream with resources relative to base url {{HtmlBaseUrl}}", job.Input.BaseUrl);

        await _processManager.ProcessJob(job, outStream);
        await outStream.FlushAsync();
    }

    private async Task Initialize()
    {
        if (!_isInitialized)
        {
            _processManager.Initialize();
            await _processManager.Start();

            _isInitialized = true;
        }
    }
}
