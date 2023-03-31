namespace PdfGeneration.Prince.Process;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thinktecture.IO;
using Job;

public interface IPrinceStreamCommunicator : IDisposable
{
    void Initialize(IStreamWriter princeStandardInput, IStreamReader princeStandardOutput);
    Task SendEndAsync();
    Task<string> ReadVersionChunkAsync();
    Task WriteJobAsync(JobDefinition jobDefinition);
    Task WriteResourcesAsync(IEnumerable<IStream> jobResources);
    Task<Chunk> TryReadPdfChunkToStreamAsync(IStream output);
    Task<Chunk> ReadChunkAsync();
}
