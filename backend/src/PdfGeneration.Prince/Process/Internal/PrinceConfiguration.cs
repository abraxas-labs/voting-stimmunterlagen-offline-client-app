// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace PdfGeneration.Prince.Process.Internal;

using System;
using System.IO;

public class PrinceConfiguration : IPrinceConfiguration
{
    private static readonly string DefaultExecutablePath;

    static PrinceConfiguration()
    {
        DefaultExecutablePath = Path.Join(Path.GetDirectoryName(typeof(PrinceConfiguration).Assembly.Location), "prince");
        System.Diagnostics.Debug.WriteLine($"DEBUG: .cctor of {nameof(PrinceConfiguration)} determined prince base path as '{DefaultExecutablePath}'");
    }

    // Settings specific for this wrapper
    public string PrinceExecutableBasePath { get; set; } = DefaultExecutablePath;
    public TimeSpan ProcessEndTimeout { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan ProcessLoggingInterval { get; set; } = TimeSpan.FromSeconds(10);

    // Original Prince settings
    public bool Debug { get; set; }
    public bool JavaScript { get; set; }
    public string? LogFileLocation { get; set; }
    public bool NoNetwork { get; set; }
    public bool Verbose { get; set; }
}
