// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace PdfGeneration.Prince.Process.Internal;

using System;
using Thinktecture.IO;

internal class PrinceConfigurationBuilder : IPrinceConfigurationBuilder
{
    private readonly PrinceConfiguration _configuration = new();

    private readonly IPath _path;
    private readonly IDirectory _directory;

    public PrinceConfigurationBuilder(IPath path, IDirectory directory)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));
        _directory = directory ?? throw new ArgumentNullException(nameof(directory));
    }

    public IPrinceConfiguration BuildConfiguration()
    {
        return _configuration;
    }

    // Settings specific to this wrapper
    public IPrinceConfigurationBuilder SetPrinceExecutableBasePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));
        if (!_directory.Exists(path))
            throw new ArgumentException("Provided path does not exist.", nameof(path));

        _configuration.PrinceExecutableBasePath = path;
        return this;
    }

    public IPrinceConfigurationBuilder SetProcessEndTimeout(TimeSpan timeout)
    {
        if (timeout <= TimeSpan.Zero)
            throw new ArgumentException("Timeout must be a positive time span larger than zero.", nameof(timeout));

        _configuration.ProcessEndTimeout = timeout;
        return this;
    }

    public IPrinceConfigurationBuilder SetProcessLoggingInterval(TimeSpan interval)
    {
        if (interval <= TimeSpan.Zero)
            throw new ArgumentException("Interval must be a positive time span larger than zero.", nameof(interval));

        _configuration.ProcessLoggingInterval = interval;
        return this;
    }

    // Original Prince settings
    public IPrinceConfigurationBuilder SetDebug(bool debug)
    {
        _configuration.Debug = debug;
        return this;
    }

    public IPrinceConfigurationBuilder SetJavascript(bool javascript)
    {
        _configuration.JavaScript = javascript;
        return this;
    }

    public IPrinceConfigurationBuilder SetLogFileLocation(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));
        var directory = _path.GetDirectoryName(path);
        if (directory == null)
            throw new ArgumentException("Provided log file location does not point to a valid directory.", nameof(path));
        if (!_directory.Exists(directory))
            throw new ArgumentException("Provided path to log directory does not exist.", nameof(path));

        _configuration.LogFileLocation = path;
        return this;
    }

    public IPrinceConfigurationBuilder SetNoNetwork(bool noNetwork)
    {
        _configuration.NoNetwork = noNetwork;
        return this;
    }

    public IPrinceConfigurationBuilder SetVerbose(bool verbose)
    {
        _configuration.Verbose = verbose;
        return this;
    }
}
