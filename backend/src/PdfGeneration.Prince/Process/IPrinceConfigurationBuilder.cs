// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace PdfGeneration.Prince.Process;

using System;

public interface IPrinceConfigurationBuilder
{
    // Settings specific for this wrapper
    IPrinceConfigurationBuilder SetPrinceExecutableBasePath(string path);
    IPrinceConfigurationBuilder SetProcessEndTimeout(TimeSpan timeout);
    IPrinceConfigurationBuilder SetProcessLoggingInterval(TimeSpan interval);

    // Original Prince settings
    IPrinceConfigurationBuilder SetDebug(bool debug);
    IPrinceConfigurationBuilder SetJavascript(bool javascript);
    IPrinceConfigurationBuilder SetLogFileLocation(string path);
    IPrinceConfigurationBuilder SetNoNetwork(bool noNetwork);
    IPrinceConfigurationBuilder SetVerbose(bool verbose);
}
