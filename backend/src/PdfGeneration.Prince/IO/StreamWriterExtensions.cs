// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace; Justification: Extension methods
namespace Thinktecture.IO;

public static class StreamWriterExtensions
{
    public static Task WriteChunkAsync(this IStreamWriter streamWriter, string tag, string data = "")
    {
        if (streamWriter == null)
            throw new ArgumentNullException(nameof(streamWriter));

        return streamWriter.BaseStream.WriteChunkAsync(tag, data);
    }

    public static Task WriteChunkAsync(this IStreamWriter streamWriter, string tag, IStream data)
    {
        if (streamWriter == null)
            throw new ArgumentNullException(nameof(streamWriter));

        return streamWriter.BaseStream.WriteChunkAsync(tag, data);
    }
}
