// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Microsoft.Extensions.DependencyInjection;

using System;
using System.Diagnostics.CodeAnalysis;
using PdfGeneration;
using PdfGeneration.Prince;
using PdfGeneration.Prince.Process;
using PdfGeneration.Prince.Process.Internal;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the required classes to the dependency injection system.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="princeConfigurationBuilder">The <see cref="IPrinceConfigurationBuilder"/> to be used with the generator.</param>
    /// <returns>The <see cref="IServiceCollection"/> passed to this method.</returns>
    [ExcludeFromCodeCoverage] // Justification: This is an optional helper method, not crucial behaviour
    public static IServiceCollection AddPrincePdfGeneration(this IServiceCollection services, Action<IPrinceConfigurationBuilder>? princeConfigurationBuilder = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        // have consumer build the configuration
        // use DI, as ConfigBuilder requires IDirectory to check for paths
        services.AddTransient<PrinceConfigurationBuilder>();

        var provider = services.BuildServiceProvider();
        var configBuilder = provider.GetRequiredService<PrinceConfigurationBuilder>();
        princeConfigurationBuilder?.Invoke(configBuilder);
        var config = configBuilder.BuildConfiguration();

        services.AddSingleton(config);
        services.AddTransient<IPrinceProcessManager, PrinceProcessManager>();
        services.AddTransient<IPdfGenerator, PrincePdfGenerator>();
        services.AddTransient<IPrinceProcessWrapper, PrinceProcessWrapper>();
        services.AddTransient<IPrinceStreamCommunicator, PrinceStreamCommunicator>();
        services.AddTransient<IPrinceMessageLogger, PrinceMessageLogger>();

        return services;
    }
}
