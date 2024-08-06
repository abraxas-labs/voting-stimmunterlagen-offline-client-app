// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace PdfGeneration.Prince.Process.Internal;

using System;
using System.Text;

internal static class PrinceConfigurationExtensions
{
    public static string GetPrinceStartArguments(this IPrinceConfiguration configuration, bool controlMode = false)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var sb = new StringBuilder();

        if (configuration.Debug)
            sb.Append("--debug ");

        if (!string.IsNullOrEmpty(configuration.LogFileLocation))
            sb.Append($"--log=\"{configuration.LogFileLocation.EscapeCommandLineArgument()}\" ");

        if (configuration.NoNetwork)
            sb.Append("--no-network ");
        if (configuration.Verbose)
            sb.Append("--verbose ");

        if (configuration.JavaScript)
            sb.Append("--javascript ");

        if (controlMode)
            sb.Append("--control ");

        return sb.ToString();
    }
}
