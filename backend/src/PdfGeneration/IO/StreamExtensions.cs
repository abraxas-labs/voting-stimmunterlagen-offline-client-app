// ReSharper disable once CheckNamespace; Justification: Extension methods
namespace Thinktecture.IO;

using System;
using System.IO;
using System.Threading.Tasks;

public static class StreamExtensions
{
    public static async Task CopyOverBufferedAsync(this IStream input, IStream output, long length, int bufferSize = 0x1000)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));
        if (!input.CanRead)
            throw new ArgumentException("Input stream is not readable", nameof(input));
        if (output == null)
            throw new ArgumentNullException(nameof(output));
        if (!output.CanWrite)
            throw new ArgumentException("Output stream is not writeable", nameof(output));

        // default buffer size 0x1000 = 4KB
        if (length < bufferSize)
        {
            bufferSize = (int)length;
        }

        var buffer = new byte[bufferSize];

        while (length > 0)
        {
            // do not read over rest of expected length
            var readLength = length >= bufferSize
                ? bufferSize
                : (int)length;

            var count = await input.ReadAsync(buffer, 0, readLength).ConfigureAwait(false);

            if (count < 0)
            {
                throw new IOException("Copy process failed to read data from input stream");
            }

            if (count > bufferSize)
            {
                throw new IOException("Copy process encountered unexpected read overrun");
            }

            await output.WriteAsync(buffer, 0, count).ConfigureAwait(false);

            length -= count;
        }
    }
}
