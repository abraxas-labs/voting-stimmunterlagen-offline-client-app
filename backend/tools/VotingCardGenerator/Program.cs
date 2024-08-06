// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Serilog;
using Voting.Stimmunterlagen.OfflineClient.Logging;

namespace VotingCardGenerator;

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
            FullName = "VotingCardTool PdfCreator",
            Description = "Uses a template file and voting data to create a printable document with voting cards",
            ExtendedHelpText = $"{Environment.NewLine}"
        };

        var arguments = new Arguments(cla);

        cla.OnExecuteAsync(async ct =>
        {
            LoggerInitializer.Initialize(arguments.LogLevel, !arguments.StreamPdf, arguments.LogFileName);
            Log.Logger.Information("+++++ Application {AssemblyName} {AssemblyVersion} is starting up +++++");
            arguments.LogConfiguration(Log.Logger);

            if (!arguments.ValidateArguments())
            {
                cla.ShowHelp();
                return 1;
            }

            await using var app = new Application(arguments);

            app.Initialize();
            await app.Run();

            Log.Logger.Information("+++++ Application {AssemblyName} {AssemblyVersion} finished +++++");
            return 0;
        });

        return cla;
    }
}
