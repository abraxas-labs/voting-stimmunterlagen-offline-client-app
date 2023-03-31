namespace PdfGeneration.Prince.Process;

using System;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;

public class Chunk : IDisposable
{
    public string Tag { get; }
    public int Length { get; }
    public IStream Data { get; }
    public bool KeepStreamOpen { get; set; }

    public Chunk(string tag, int length, IStream data)
    {
        Tag = tag ?? throw new ArgumentNullException(nameof(tag));
        if (string.IsNullOrWhiteSpace(tag) || tag.Length != 3)
            throw new ArgumentException("Tag must be 3 characters long.", nameof(tag));

        if (Length < 0)
            throw new ArgumentException("Cannot set a negative length.", nameof(length));

        Data = data ?? throw new ArgumentNullException(nameof(data));
        Length = length;

        // Make data in chunk readable per default, if possible
        if (Data.CanSeek)
        {
            Data.Position = 0;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public virtual void Dispose(bool disposing)
    {
        if (disposing && !KeepStreamOpen)
        {
            Data.Dispose();
        }
    }

    public string ReadString()
    {
        using var sr = new StreamReaderAdapter(Data);
        return sr.ReadToEnd();
    }
}
