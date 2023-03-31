namespace PdfGeneration.Prince.Process;

public interface IPrinceMessageLogger
{
    bool ProcessLogMessagesFromChunk(Chunk logChunk);
}
