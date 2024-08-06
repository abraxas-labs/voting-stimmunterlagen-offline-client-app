// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace PdfGeneration.Prince.Job;

using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

[ExcludeFromCodeCoverage] // Justification: Plain DTO for serialization
[JsonObject(MemberSerialization.OptIn)]
public class EncryptSettings
{
    [JsonProperty("key-bits")]
    public int KeyBits { get; set; }

    [JsonProperty("user-password", NullValueHandling = NullValueHandling.Ignore)]
    public string? UserPassword { get; set; }

    [JsonProperty("owner-password", NullValueHandling = NullValueHandling.Ignore)]
    public string? OwnerPassword { get; set; }

    [JsonProperty("disallow-print")]
    public bool DisallowPrint { get; set; }

    [JsonProperty("disallow-modify")]
    public bool DisallowModify { get; set; }

    [JsonProperty("disallow-copy")]
    public bool DisallowCopy { get; set; }

    [JsonProperty("disallow-annotate")]
    public bool DisallowAnnotate { get; set; }
}
