namespace PdfGeneration.Prince.Job;

using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using System.Collections.Generic;
using Thinktecture.IO;

[ExcludeFromCodeCoverage] // Justification: Plain DTO for serialization
[JsonObject(MemberSerialization.OptIn)]
public class JobDefinition
{
    [JsonProperty("input")]
    public Input Input { get; set; } = new Input();

    [JsonIgnore]
    public List<IStream> JobResources { get; set; } = new List<IStream>();

    [JsonProperty("job-resource-count")]
    public int JobResourceCount => JobResources.Count;

    [JsonProperty("pdf")]
    public PdfConfig PdfConfig { get; set; } = new PdfConfig();

    [JsonProperty("metadata")]
    public PdfMetadata Metadata { get; set; } = new PdfMetadata();

    public void AddSource(IStream dataStream)
    {
        AddElement(dataStream, Input.Sources);
    }

    public void AddStyleSheet(IStream dataStream)
    {
        AddElement(dataStream, Input.StyleSheets);
    }

    public void AddJavaScript(IStream dataStream)
    {
        AddElement(dataStream, Input.JavaScripts);
    }

    public void AddFileAttachment(IStream dataStream, string filename = "", string description = "")
    {
        JobResources.Add(dataStream);
        PdfConfig.FileAttachments.Add(new FileAttachment()
        {
            Url = GetResourceIdentifier(),
            FileName = filename,
            Description = description
        });
    }

    private void AddElement(IStream dataStream, IList<string> container)
    {
        JobResources.Add(dataStream);
        container.Add(GetResourceIdentifier());
    }

    private string GetResourceIdentifier()
    {
        return $"job-resource:{JobResources.Count - 1}";
    }
}
