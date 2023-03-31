namespace HtmlGeneration.RazorLight.Razor;

using Thinktecture.IO;

public interface IRazorLightProjectItem
{
    string Key { get; }

    IStream Stream { get; }
}
