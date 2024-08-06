// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;

namespace PdfGeneration.Prince.Process;

public interface IPrinceProcessWrapper
{
    IPrinceStreamCommunicator Prince { get; }
    bool Running { get; }

    void Initialize();
    Task Start();
    Task Stop();
}
