// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace HtmlGeneration.RazorLight.Razor;

using Thinktecture.IO;

public interface IRazorLightProjectItem
{
    string Key { get; }

    IStream Stream { get; }
}
