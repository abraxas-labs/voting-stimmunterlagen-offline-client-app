namespace PdfGeneration;

using System.Threading.Tasks;
using Thinktecture.IO;

public interface IPdfGenerator
{
    Task GeneratePdfAsync(IStream htmlStream, IStream outStream, string? resourceBasePath = null);
}
