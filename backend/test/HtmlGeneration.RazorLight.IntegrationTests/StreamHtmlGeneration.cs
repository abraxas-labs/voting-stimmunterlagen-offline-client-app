using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;
using Xunit;

namespace HtmlGeneration.RazorLight.IntegrationTests;

public class StreamHtmlGeneration
{
    [Fact]
    public async Task Test_complete_in_memory_template_rendering()
    {
        // arrange
        // use DI
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddRazorLightHtmlGeneration();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // retrieve Html Generator from DI
        var generator = serviceProvider.GetRequiredService<IHtmlGenerator>();

        // model
        var model = BuildModel();

        // out stream
        var ms = new MemoryStream();
        var outStream = new StreamAdapter(ms);

        // build with 
        await generator.RenderHtmlAsync("TestProjectItem", BuildTestTemplateStream(), model, outStream);

        // act
        var renderedHtml = Encoding.UTF8.GetString(ms.ToArray());

        // assert
        renderedHtml.Should().Contain("<li>First</li>");
        renderedHtml.Should().Contain("<li>Second</li>");
    }


    private static IStream BuildTestTemplateStream()
    {
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(BuildTestTemplate()));
        return new StreamAdapter(ms);
    }

    private static string BuildTestTemplate()
    {
        return @"@model IEnumerable<dynamic>
<!DOCTYPE html>
<html lang=""en"">
<head></head>
<body>
<ul>
@foreach(var element in Model)
{
	<li>@element.Name</li>
}
</ul>
</body>
</html>
";
    }

    public class ModelItem
    {
        public string Name { get; set; } = string.Empty;
    }

    private static List<ModelItem> BuildModel()
    {
        return new List<ModelItem>()
        {
            new ModelItem() { Name = "First" },
            new ModelItem() { Name = "Second" }
        };
    }
}
