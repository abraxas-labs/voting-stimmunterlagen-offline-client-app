namespace PdfGeneration.Prince.Process.Internal;

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Thinktecture.IO;
using Job;

public class PrinceProcessManager : IPrinceProcessManager
{
    private readonly ILogger<PrinceProcessManager>? _logger;
    private readonly IPrinceProcessWrapper _processWrapper;
    private readonly IPrinceMessageLogger _messageLogger;

    public PrinceProcessManager(ILogger<PrinceProcessManager>? logger, IPrinceProcessWrapper processWrapper, IPrinceMessageLogger messageLogger)
    {
        _logger = logger;
        _processWrapper = processWrapper ?? throw new ArgumentNullException(nameof(processWrapper));
        _messageLogger = messageLogger ?? throw new ArgumentNullException(nameof(messageLogger));
    }

    public void Initialize()
    {
        _processWrapper.Initialize();
    }

    public void Start()
    {
        _processWrapper.Start();
    }

    public void Stop()
    {
        _processWrapper.Stop();
    }

    public async Task<bool> ProcessJob(JobDefinition jobDefinition, IStream output)
    {
        if (jobDefinition == null)
            throw new ArgumentNullException(nameof(jobDefinition));
        if (!_processWrapper.Running)
            throw new InvalidOperationException("External Prince process is not available. Make sure to start the process first.");

        if (string.IsNullOrEmpty(jobDefinition.Input.BaseUrl))
            _logger?.LogWarning($"{nameof(ProcessJob)}: Provided {nameof(jobDefinition)} has not been provided with a {nameof(Input.BaseUrl)}. Relative document references may not be resolved properly.");

        // pipe Job and job resources into prince
        await _processWrapper.Prince.WriteJobAsync(jobDefinition).ConfigureAwait(false);
        await _processWrapper.Prince.WriteResourcesAsync(jobDefinition.JobResources).ConfigureAwait(false);

        // expect the PDF stream first
        var chunk = await _processWrapper.Prince.TryReadPdfChunkToStreamAsync(output).ConfigureAwait(false);
        if (chunk.Tag == "pdf")
        {
            // we already did write the pdf to the output, read expected log chunk now
            chunk = await _processWrapper.Prince.ReadChunkAsync().ConfigureAwait(false);
        }

        if (chunk.Tag == "log")
        {
            return _messageLogger.ProcessLogMessagesFromChunk(chunk);
        }

        if (chunk.Tag == "err")
        {
            _logger?.LogError("Processing job failed: {ErrorMessage}", chunk.ReadString());
            return false;
        }

        throw new IOException($"Got unexpected unknown chunk from external Prince process: {chunk.Tag}");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Stop();
        }
    }
}
