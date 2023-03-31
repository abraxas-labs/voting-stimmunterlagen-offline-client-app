namespace PdfGeneration.Prince.Job;

using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

[ExcludeFromCodeCoverage] // Justification: Plain DTO for serialization
[JsonObject(MemberSerialization.OptIn)]
public class FileAttachment
{
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
    public string? FileName { get; set; }

    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
    public string? Description { get; set; }
}
