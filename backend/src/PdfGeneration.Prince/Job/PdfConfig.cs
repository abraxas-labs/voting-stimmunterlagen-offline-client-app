// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace PdfGeneration.Prince.Job;

using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Newtonsoft.Json;

[ExcludeFromCodeCoverage] // Justification: Plain DTO for serialization
[JsonObject(MemberSerialization.OptIn)]
public class PdfConfig
{
    [JsonProperty("embed-fonts")]
    public bool EmbedFonts { get; set; } = true;

    [JsonProperty("subset-fonts")]
    public bool SubsetFonts { get; set; } = true;

    [JsonProperty("pdf-profile", NullValueHandling = NullValueHandling.Ignore)]
    public string? PdfProfile { get; set; }

    [JsonProperty("pdf-output-intent", NullValueHandling = NullValueHandling.Ignore)]
    public string? PdfOutputIntent { get; set; }

    [JsonProperty("convert-colors", NullValueHandling = NullValueHandling.Ignore)]
    public bool? ConvertColors { get; set; }

    [JsonProperty("artifical-fonts")]
    public bool ArtificialFonts { get; set; }

    [JsonProperty("fallback-cmyk-profile", NullValueHandling = NullValueHandling.Ignore)]
    public string? FallbackCmykProfile { get; set; }

    [JsonProperty("force-identity-encoding")]
    public bool ForceIdentityEncoding { get; set; }

    [JsonProperty("compress")]
    public bool Compress { get; set; } = true;

    [JsonProperty("encrypt", NullValueHandling = NullValueHandling.Ignore)]
    public EncryptSettings? EncryptSettings { get; set; }

    [JsonProperty("attach")]
    public IList<FileAttachment> FileAttachments { get; set; } = new List<FileAttachment>();
    public bool ShouldSerializeFileAttachments => FileAttachments.Count > 0;
}
