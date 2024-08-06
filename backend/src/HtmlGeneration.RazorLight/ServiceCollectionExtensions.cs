// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Microsoft.Extensions.DependencyInjection;

using System;
using Logging;
using RazorLight;
using HtmlGeneration;
using HtmlGeneration.RazorLight;
using HtmlGeneration.RazorLight.Razor;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the required classes to the dependency injection system.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> passed to this method.</returns>
    public static IServiceCollection AddRazorLightHtmlGeneration(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        // Register the StreamProject as itself and as IPrefillProject
        var provider = services.BuildServiceProvider();
        var streamProjectLogger = provider.GetService<ILogger<StreamProject>>();

        var streamProject = new StreamProject(streamProjectLogger);
        services.AddSingleton(streamProject);
        services.AddSingleton<IPrefillProject>(streamProject);

        // Register the other services that require the project system
        services.AddTransient<RazorLightEngineBuilder>();

        services.AddSingleton<IRazorLightEngine>(serviceProvider =>
        {
            var project = serviceProvider.GetRequiredService<StreamProject>();
            var engineBuilder = serviceProvider.GetRequiredService<RazorLightEngineBuilder>();

            return engineBuilder
                .UseStreamProject(project)
                .Build();
        });

        services.AddSingleton<IHtmlGenerator, HtmlGenerator>(serviceProvider =>
        {
            var generatorLogger = serviceProvider.GetService<ILogger<HtmlGenerator>>();
            var engine = serviceProvider.GetRequiredService<IRazorLightEngine>();
            var project = serviceProvider.GetRequiredService<IPrefillProject>();

            return new HtmlGenerator(generatorLogger, engine, project);
        });

        return services;
    }
}
