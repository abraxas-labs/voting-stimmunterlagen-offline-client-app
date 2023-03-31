namespace HtmlGeneration.RazorLight.Razor;

using Thinktecture.IO;

public interface IPrefillProject
{
    void AddProjectItem(string templateKey, IStream templateStream);
    bool ContainsTemplate(string templateKey);
}
