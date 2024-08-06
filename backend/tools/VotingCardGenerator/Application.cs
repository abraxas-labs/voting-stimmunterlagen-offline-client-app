// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;

namespace VotingCardGenerator;

public class Application : IAsyncDisposable
{
    private readonly Arguments _arguments;

    private ServiceProvider _services = null!;

    public Application(Arguments arguments)
    {
        _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
    }

    public void Initialize()
    {
        var path = new PathAdapter();

        // Build DI container
        var serviceCollection = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder.AddSerilog())
            .AddTransient<IFile, FileAdapter>()
            .AddTransient<IPath, PathAdapter>()
            .AddTransient<IDirectory, DirectoryAdapter>()
            .AddTransient<Generator>()
            .AddRazorLightHtmlGeneration()
            .AddPrincePdfGeneration(configBuilder =>
            {
                configBuilder
                    .SetNoNetwork(true)
                    .SetDebug(true)
                    .SetJavascript(true)
                    .SetVerbose(_arguments.LogLevel == LogEventLevel.Verbose);

                if (!string.IsNullOrWhiteSpace(_arguments.LogFileName))
                {
                    configBuilder.SetLogFileLocation(path.Combine(path.GetDirectoryName(_arguments.LogFileName), $"princelog-{DateTime.UtcNow:yyyyMMdd-HHmm}.txt"));
                }
            });

        _services = serviceCollection.BuildServiceProvider();
    }

    public Task Run()
    {
        var generator = _services.GetRequiredService<Generator>();

        if (_arguments.ProduceHtmlOnly)
        {
            return generator.GenerateHtml(_arguments.TemplateStream, _arguments.DataStream, _arguments.PdfStream);
        }
        else
        {
            return generator.GeneratePdf(_arguments.TemplateStream, _arguments.DataStream, _arguments.PdfStream);
        }
    }

    public ValueTask DisposeAsync()
    {
        return _services.DisposeAsync();
    }
}
