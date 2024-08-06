// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace PdfGeneration;

using System.Threading.Tasks;
using Thinktecture.IO;

public interface IPdfGenerator
{
    Task GeneratePdfAsync(IStream htmlStream, IStream outStream, string? resourceBasePath = null);
}
