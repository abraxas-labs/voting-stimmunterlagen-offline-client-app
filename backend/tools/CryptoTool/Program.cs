// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Voting.Stimmunterlagen.OfflineClient.Logging;

namespace CryptoTool;

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
        var n = Environment.NewLine;
        var cla = new CommandLineApplication()
        {
            FullName = "VotingCardTool CryptoTool",
            Description = "En- or decrypt files using certificates.",
            ExtendedHelpText = $"{n}If only list is requested, returns the list of the certificates found in the users store."
                               + $"{n}If both sender and receiver certificates are specified, the input file(s) will be encrypted."
                               + $"{n}If only the receiver certificate is specified, the input file(s) will be decrypted."
                               + $"{n}Certificates can be specified by subject name from the store, or as a file. Provide the file's password if the private key needs to be accessed."
        };

        var arguments = new Arguments(cla);

        cla.OnExecute(() =>
        {
            LoggerInitializer.Initialize(arguments.LogLevel, !arguments.SenderJsonList && !arguments.ReceiverJsonList, arguments.LogFileName);
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
            services.AddSingleton<ICertificateProvider, CertificateProvider>();
            services.AddSingleton(arguments);
            services.AddCryptography();

            var serviceProvider = services.BuildServiceProvider();
            var app = serviceProvider.GetRequiredService<Application>();

            app.Initialize();
            app.Run();

            Log.Logger.Information("+++++ Application {AssemblyName} {AssemblyVersion} finished +++++");
            return 0;
        });

        return cla;
    }
}
