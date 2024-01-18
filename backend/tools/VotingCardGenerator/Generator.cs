using System;
using System.IO;
using System.Threading.Tasks;
using HtmlGeneration;
using Newtonsoft.Json;
using PdfGeneration;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;

namespace VotingCardGenerator;

public class Generator
{
    private readonly IHtmlGenerator _htmlBuilder;
    private readonly IPdfGenerator _pdfBuilder;

    public Generator(IHtmlGenerator htmlBuilder, IPdfGenerator pdfBuilder)
    {
        _htmlBuilder = htmlBuilder ?? throw new ArgumentNullException(nameof(htmlBuilder));
        _pdfBuilder = pdfBuilder ?? throw new ArgumentNullException(nameof(pdfBuilder));
    }

    public async Task GenerateHtml(IStream templateStream, IStream modelStream, IStream htmlStream)
    {
        if (templateStream == null) throw new ArgumentNullException(nameof(templateStream));
        if (modelStream == null) throw new ArgumentNullException(nameof(modelStream));
        if (htmlStream == null) throw new ArgumentNullException(nameof(htmlStream));

        var model = DeserializeModel(modelStream);

        await _htmlBuilder.RenderHtmlAsync(Guid.NewGuid().ToString(), templateStream, model, htmlStream);
        await htmlStream.FlushAsync();
    }

    public async Task GeneratePdf(IStream templateStream, IStream modelStream, IStream pdfStream)
    {
        if (templateStream == null) throw new ArgumentNullException(nameof(templateStream));
        if (modelStream == null) throw new ArgumentNullException(nameof(modelStream));
        if (pdfStream == null) throw new ArgumentNullException(nameof(pdfStream));

        using var htmlStream = new StreamAdapter(new MemoryStream());
        await GenerateHtml(templateStream, modelStream, htmlStream);

        htmlStream.Position = 0;
        await _pdfBuilder.GeneratePdfAsync(htmlStream, pdfStream);
        await pdfStream.FlushAsync();
    }

    private static dynamic? DeserializeModel(IStream modelStream)
    {
        using var streamReader = new StreamReader(modelStream.UnsafeConvert());
        using var jsonTextReader = new JsonTextReader(streamReader);
        var jsonSerializer = JsonSerializer.Create();
        return jsonSerializer.Deserialize(jsonTextReader);
    }
}
