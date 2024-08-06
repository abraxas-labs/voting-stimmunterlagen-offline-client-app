// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace PdfGeneration.Prince.Job;

using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[ExcludeFromCodeCoverage] // Justification: Plain DTO for serialization
[JsonObject(MemberSerialization.OptIn)]
public class Input
{
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(StringEnumConverter), true)]
    public InputType? InputType { get; set; }

    [JsonProperty("base", NullValueHandling = NullValueHandling.Ignore)]
    public string? BaseUrl { get; set; }

    [JsonProperty("media", NullValueHandling = NullValueHandling.Ignore)]
    public string? Media { get; set; }

    [JsonProperty("javascript")]
    public bool JavaScript { get; set; }

    [JsonProperty("xinclude")]
    public bool XInclude { get; set; }

    [JsonProperty("xml-external-entities")]
    public bool XmlExternalEntities { get; set; }

    [JsonProperty("default-style")]
    public bool UseDefaultStyle { get; set; } = true;

    [JsonProperty("author-style")]
    public bool UseAuthorStyle { get; set; } = true;

    [JsonProperty("src")]
    public IList<string> Sources { get; set; } = new List<string>();
    public bool ShouldSerializeSources => Sources.Count > 0;

    [JsonProperty("styles")]
    public IList<string> StyleSheets { get; set; } = new List<string>();
    public bool ShouldSerializeStyleSheets => StyleSheets.Count > 0;

    [JsonProperty("scripts")]
    public IList<string> JavaScripts { get; set; } = new List<string>();
    public bool ShouldSerializeJavaScripts => JavaScripts.Count > 0;
}
