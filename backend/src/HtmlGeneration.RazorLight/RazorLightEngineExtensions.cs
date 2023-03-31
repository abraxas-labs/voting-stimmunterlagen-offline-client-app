// ReSharper disable once CheckNamespace; Justification: Extension method
namespace RazorLight;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IO;

public static class RazorLightEngineExtensions
{
    internal const int DefaultBufferSize = 2014;

    public static async Task CompileAndRenderTemplateAsync<T>(this IRazorLightEngine engine, string templateKey, T model, IStream outStream, int bufferSize = 0, Encoding? encoding = null)
    {
        if (engine == null)
            throw new ArgumentNullException(nameof(engine));
        if (templateKey == null)
            throw new ArgumentNullException(nameof(templateKey));
        if (outStream == null)
            throw new ArgumentNullException(nameof(outStream));
        if (!outStream.CanWrite)
            throw new InvalidOperationException("The output stream must be writable, but CanWrite is false.");

        var template = await engine.CompileTemplateAsync(templateKey);
        await engine.RenderTemplateAsync(template, model, outStream, bufferSize, encoding);
    }

    public static async Task RenderTemplateAsync<T>(this IRazorLightEngine engine, ITemplatePage template, T model, IStream outStream, int bufferSize = 0, Encoding? encoding = null)
    {
        if (engine == null)
            throw new ArgumentNullException(nameof(engine));
        if (template == null)
            throw new ArgumentNullException(nameof(template));
        if (outStream == null)
            throw new ArgumentNullException(nameof(outStream));
        if (!outStream.CanWrite)
            throw new InvalidOperationException("The output stream must be writable, but CanWrite is false.");

        if (bufferSize <= 0)
        {
            bufferSize = DefaultBufferSize;
        }

        encoding ??= Encoding.UTF8;

        // it is important to leave the inner stream opened after writing, disposing the StreamWriter at the
        // end of the using block will ensure that the buffer is flushed to the underlying stream
        using var writer = new StreamWriter(outStream.UnsafeConvert(), encoding, bufferSize, true);
        await engine.RenderTemplateAsync(template, model, writer);
    }
}
