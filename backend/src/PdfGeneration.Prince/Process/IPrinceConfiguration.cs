namespace PdfGeneration.Prince.Process;

using System;

public interface IPrinceConfiguration
{
    // Settings specific for this wrapper
    string PrinceExecutableBasePath { get; }
    TimeSpan ProcessEndTimeout { get; }
    TimeSpan ProcessLoggingInterval { get; }

    // Original Prince settings
    bool Debug { get; }
    bool JavaScript { get; set; }
    string? LogFileLocation { get; }
    bool NoNetwork { get; }
    bool Verbose { get; }
}
