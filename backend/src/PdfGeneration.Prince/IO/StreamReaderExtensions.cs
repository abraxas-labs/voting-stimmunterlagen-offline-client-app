// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Thinktecture.IO;

using System;
using System.Threading.Tasks;
using PdfGeneration.Prince.Process;

internal static class StreamReaderExtensions
{
    internal static Task<Chunk> TryReadChunkAsync(this IStreamReader streamReader, string tag, IStream? output = null)
    {
        if (streamReader == null)
            throw new ArgumentNullException(nameof(streamReader));

        return streamReader.BaseStream.TryReadChunkAsync(tag, output);
    }

    internal static Task<Chunk> ReadChunkAsync(this IStreamReader streamReader, IStream? output = null)
    {
        if (streamReader == null)
            throw new ArgumentNullException(nameof(streamReader));

        return streamReader.BaseStream.ReadChunkAsync(output);
    }
}
