// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace PdfGeneration.Prince.Process;

public interface IPrinceMessageLogger
{
    bool ProcessLogMessagesFromChunk(Chunk logChunk);
}
