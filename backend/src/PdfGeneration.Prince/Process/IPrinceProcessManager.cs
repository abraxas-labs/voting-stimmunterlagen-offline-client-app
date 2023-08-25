namespace PdfGeneration.Prince.Process;

using System;
using System.Threading.Tasks;
using Thinktecture.IO;
using Job;

public interface IPrinceProcessManager : IAsyncDisposable
{
    void Initialize();
    Task Start();
    Task Stop();

    Task<bool> ProcessJob(JobDefinition jobDefinition, IStream output);
}
