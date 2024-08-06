// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace HtmlGeneration;

using System.Threading.Tasks;
using Thinktecture.IO;

public interface IHtmlGenerator
{
    Task RenderHtmlAsync<T>(string templateKey, IStream templateStream, T model, IStream outStream);
    Task RenderHtmlAsync<T>(string templateKey, T model, IStream outStream);
}
