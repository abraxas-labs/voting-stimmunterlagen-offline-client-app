// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Thinktecture.IO;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Adapters;
using PdfGeneration.Prince.Process;

internal static class StreamExtensions
{
    public static Task WriteChunkAsync(this IStream stream, string tag, string data = "")
    {
        return stream.WriteChunkAsync(tag, Encoding.UTF8.GetBytes(data));
    }

    public static Task WriteChunkAsync(this IStream stream, string tag, byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        using var ms = new MemoryStream(data);
        using var sa = new StreamAdapter(ms);
        return stream.WriteChunkAsync(tag, sa);
    }

    public static Task WriteChunkAsync(this IStream stream, string tag, IStream data)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        if (!stream.CanWrite)
            throw new ArgumentException("Stream is not writeable.", nameof(stream));
        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentNullException(nameof(tag));
        if (tag.Length != 3)
            throw new ArgumentException("Tag must be 3 characters long.", nameof(tag));
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        if (!data.CanRead)
            throw new ArgumentException("Data stream is not readable.", nameof(data));

        var preamble = Encoding.UTF8.GetBytes($"{tag} {data.Length}\n");
        stream.Write(preamble, 0, preamble.Length);

        // the std in of the prince process is a file stream, async copy to leads to an error.
        // https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/filestream-position-updates-after-readasync-writeasync-completion
        data.CopyTo(stream);

        var newLine = Encoding.UTF8.GetBytes("\n");
        stream.Write(newLine, 0, newLine.Length);
        return Task.CompletedTask;
    }

    public static async Task<Chunk> ReadChunkAsync(this IStream stream, IStream? output = null)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        if (!stream.CanRead)
            throw new ArgumentException("Stream is not readable.", nameof(stream));

        MemoryStream? ms = null;
        if (output == null)
        {
            output = new StreamAdapter(ms = new MemoryStream());
        }

        var tag = await stream.ReadTagAsync().ConfigureAwait(false);
        var length = await stream.ReadChunkLengthAsync().ConfigureAwait(false);

        await stream.CopyOverBufferedAsync(output, length);
        await stream.ReadNewlineAsync().ConfigureAwait(false);

        // make sure our own stream can be read right from the start
        if (ms != null)
        {
            ms.Position = 0;
        }

        return new Chunk(tag, length, output);
    }

    public static async Task<Chunk> TryReadChunkAsync(this IStream stream, string expectedTag, IStream? output = null)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        if (!stream.CanRead)
            throw new ArgumentException("Stream is not readable.", nameof(stream));
        if (string.IsNullOrWhiteSpace(expectedTag))
            throw new ArgumentNullException(nameof(expectedTag));

        var tag = await stream.ReadTagAsync().ConfigureAwait(false);

        // If we got an unexpected tag (or don't have an output stream), write to a temporary internal stream instead,
        // else, directly write the expected tag to the provided output stream
        if (tag != expectedTag || output == null)
        {
            output = new StreamAdapter(new MemoryStream());
        }

        var length = await stream.ReadChunkLengthAsync().ConfigureAwait(false);
        await stream.CopyOverBufferedAsync(output, length);
        await stream.ReadNewlineAsync().ConfigureAwait(false);

        return new Chunk(tag, length, output);
    }

    private static async Task<string> ReadTagAsync(this IStream stream)
    {
        var tagBuffer = new byte[4];
        if (await stream.ReadAsync(tagBuffer, 0, 4).ConfigureAwait(false) != 4)
        {
            throw new IOException("Failed to read the tag and spacer expected to start the chunk.");
        }

        if (tagBuffer[3] != ' ')
        {
            throw new IOException("Missing space after chunk tag.");
        }

        return Encoding.ASCII.GetString(tagBuffer, 0, 3);
    }

    private static async Task<int> ReadChunkLengthAsync(this IStream stream)
    {
        var length = 0;
        var maxNumLength = 9;
        var numLength = 0;
        var buffer = new byte[1];

        for (; numLength < maxNumLength + 1; numLength++)
        {
            await stream.ReadAsync(buffer, 0, 1).ConfigureAwait(false);

            if (buffer[0] == '\n')
                break;

            if (buffer[0] < '0' || buffer[0] > '9')
            {
                throw new IOException("Unexpected character in chunk length");
            }

            length *= 10;
            length += buffer[0] - '0';
        }

        if (numLength < 1 || numLength > maxNumLength)
        {
            throw new IOException("Invalid chunk length.");
        }

        return length;
    }

    private static async Task ReadNewlineAsync(this IStream stream)
    {
        var buffer = new byte[1];
        await stream.ReadAsync(buffer, 0, 1).ConfigureAwait(false);

        if (buffer[0] != '\n')
        {
            throw new IOException("Missing newline after chunk data.");
        }
    }
}
