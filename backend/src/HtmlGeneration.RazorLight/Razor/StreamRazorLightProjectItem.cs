using RazorLight.Razor;

namespace HtmlGeneration.RazorLight.Razor;

using System;
using System.IO;
using Thinktecture.IO;

public class StreamRazorLightProjectItem : RazorLightProjectItem
{
    private readonly IStream _stream;

    public override string Key { get; }

    public override bool Exists => true;

    public StreamRazorLightProjectItem(string key, IStream stream)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));

        if (!stream.CanRead)
            throw new ArgumentException("Stream must be readable", nameof(stream));
    }

    public override Stream Read()
    {
        return _stream.UnsafeConvert();
    }
}
