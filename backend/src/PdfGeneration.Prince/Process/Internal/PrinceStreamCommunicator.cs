// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace PdfGeneration.Prince.Process.Internal;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Thinktecture.IO;
using Job;

public class PrinceStreamCommunicator : IPrinceStreamCommunicator
{
    private readonly ILogger<PrinceStreamCommunicator>? _logger;

    private IStreamWriter _princeStandardInput = null!;
    private IStreamReader _princeStandardOutput = null!;

    public PrinceStreamCommunicator(ILogger<PrinceStreamCommunicator>? logger)
    {
        _logger = logger;
    }

    public void Initialize(IStreamWriter princeStandardInput, IStreamReader princeStandardOutput)
    {
        if (_princeStandardInput != null || _princeStandardOutput != null)
            throw new InvalidOperationException($"{nameof(PrinceStreamCommunicator)} has already been initialized.");

        _princeStandardInput = princeStandardInput;
        _princeStandardOutput = princeStandardOutput;
    }

    public Task SendEndAsync()
    {
        _logger?.LogTrace("Sending END chunk to prince standard input.");
        return _princeStandardInput.WriteChunkAsync("end");
    }

    public async Task<string> ReadVersionChunkAsync()
    {
        _logger?.LogTrace("Trying to receive VER chunk from prince standard output.");
        var chunk = await _princeStandardOutput.ReadChunkAsync();
        if (chunk.Tag == "ver")
        {
            return chunk.ReadString();
        }

        throw new InvalidOperationException($"Excpected to read a version chunk (with tag 'ver'), but received chunk with tag '{chunk.Tag}'");
    }

    public Task WriteJobAsync(JobDefinition jobDefinition)
    {
        if (jobDefinition == null)
            throw new ArgumentNullException(nameof(jobDefinition));

        var jobJson = JsonConvert.SerializeObject(jobDefinition);

        _logger?.LogDebug("Writing job to external Prince process: {PrinceJobJson}", jobJson);

        return _princeStandardInput.WriteChunkAsync("job", jobJson);
    }

    public async Task WriteResourcesAsync(IEnumerable<IStream> jobResources)
    {
        foreach (var jobResource in jobResources)
        {
            await _princeStandardInput.WriteChunkAsync("dat", jobResource).ConfigureAwait(false);
        }

        // make sure all is written
        await _princeStandardInput.FlushAsync();
    }

    public Task<Chunk> TryReadPdfChunkToStreamAsync(IStream output)
    {
        return _princeStandardOutput.TryReadChunkAsync("pdf", output);
    }

    public Task<Chunk> ReadChunkAsync()
    {
        return _princeStandardOutput.ReadChunkAsync();
    }

    #region Dispose pattern

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _princeStandardInput?.Dispose();
            _princeStandardOutput?.Dispose();
        }
    }

    #endregion Dispose pattern
}
