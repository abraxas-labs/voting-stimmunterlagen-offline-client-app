using System;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Voting.Stimmunterlagen.OfflineClient.Logging;

namespace EchDeliveryJsonConverter;

public class Program
{
    static int Main(string[] args)
    {
        HandleDebugSwitch(ref args);

        try
        {
            var cla = BuildCommandLineApplication();
            return cla.Execute(args);
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "Fatal error while executing application.");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
                Console.ReadKey();
#endif
        }
    }

    private static void HandleDebugSwitch(ref string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            if (string.Equals(Arguments.DebugSwitch, args[i], StringComparison.InvariantCultureIgnoreCase))
            {
                // remove found element from argument list
                args = args.Where((_, index) => index != i).ToArray();

                var aborted = false;
                Console.CancelKeyPress += (_, _) => { aborted = true; };

                while (!aborted && !System.Diagnostics.Debugger.IsAttached)
                {
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
        }
    }

    private static CommandLineApplication BuildCommandLineApplication()
    {
        var cla = new CommandLineApplication()
        {
            FullName = "VotingCardTool chVote to JSON converter",
            Description = "Converts an chVote XML file to JSON"
        };

        var arguments = new Arguments(Log.Logger, cla);

        cla.OnExecute(() =>
        {
            LoggerInitializer.Initialize(arguments.LogLevel, !arguments.StreamOutput, arguments.LogFileName);
            Log.Logger.Information("+++++ Application {AssemblyName} {AssemblyVersion} is starting up +++++");
            arguments.LogConfiguration(Log.Logger);

            if (!arguments.ValidateArguments())
            {
                cla.ShowHelp();
                return 1;
            }

            var services = new ServiceCollection();
            services.AddLogging(lb => lb.AddSerilog());
            services.AddSingleton<Application>();
            services.AddSingleton(arguments);
            services.AddEchDeliveryGeneration();

            var serviceProvider = services.BuildServiceProvider();
            var app = serviceProvider.GetRequiredService<Application>();

            app.Run().Wait();

            Log.Logger.Information("+++++ Application {AssemblyName} {AssemblyVersion} finished +++++");
            return 0;
        });

        return cla;
    }
}
