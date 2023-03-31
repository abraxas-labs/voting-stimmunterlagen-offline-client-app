namespace PdfGeneration.Prince.Process;

using System;
using System.Threading.Tasks;
using Thinktecture.IO;
using Job;

public interface IPrinceProcessManager : IDisposable
{
    void Initialize();
    void Start();
    void Stop();

    Task<bool> ProcessJob(JobDefinition jobDefinition, IStream output);
}
