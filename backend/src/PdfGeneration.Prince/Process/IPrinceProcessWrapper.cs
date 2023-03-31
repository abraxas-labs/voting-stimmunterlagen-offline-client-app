namespace PdfGeneration.Prince.Process;

public interface IPrinceProcessWrapper
{
    IPrinceStreamCommunicator Prince { get; }
    bool Running { get; }

    void Initialize();
    void Start();
    void Stop();
}
