// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace PdfGeneration.Prince.Job;

using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

[ExcludeFromCodeCoverage] // Justification: Plain DTO for serialization
[JsonObject(MemberSerialization.OptIn)]
public class PdfMetadata
{
    [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
    public string? PdfTitle { get; set; }

    [JsonProperty("subject", NullValueHandling = NullValueHandling.Ignore)]
    public string? PdfSubject { get; set; }

    [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
    public string? PdfAuthor { get; set; }

    [JsonProperty("keywords", NullValueHandling = NullValueHandling.Ignore)]
    public string? PdfKeywords { get; set; }

    [JsonProperty("creator", NullValueHandling = NullValueHandling.Ignore)]
    public string? PdfCreator { get; set; }
}
