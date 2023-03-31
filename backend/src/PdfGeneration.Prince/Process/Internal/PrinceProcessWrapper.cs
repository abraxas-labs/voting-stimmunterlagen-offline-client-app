namespace PdfGeneration.Prince.Process.Internal;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;

public class PrinceProcessWrapper : IPrinceProcessWrapper, IDisposable
{
    private readonly ILogger<PrinceProcessWrapper>? _logger;
    private readonly IPath _path;
    private readonly IFile _file;
    private readonly IDirectory _directory;
    private readonly IPrinceConfiguration _configuration;
    private readonly IPrinceStreamCommunicator _communicator;

    private string _princePath = string.Empty;
    private Process? _princeProcess;

    public bool Running => ((_princeProcess != null) && !_princeProcess.HasExited);

    public IPrinceStreamCommunicator Prince => _communicator;

    public PrinceProcessWrapper(ILogger<PrinceProcessWrapper>? logger, IPath path, IFile file, IDirectory directory, IPrinceConfiguration configuration, IPrinceStreamCommunicator communicator)
    {
        _logger = logger;
        _path = path ?? throw new ArgumentNullException(nameof(path));
        _file = file ?? throw new ArgumentNullException(nameof(file));
        _directory = directory ?? throw new ArgumentNullException(nameof(directory));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _communicator = communicator ?? throw new ArgumentNullException(nameof(communicator));
    }

    public void Initialize()
    {
        _princePath = DeterminePrinceExecutablePath();
        _logger?.LogInformation("Initialized with the path to prince: {PrincePath}", _princePath);
    }

    public void Start()
    {
        if (string.IsNullOrWhiteSpace(_princePath))
            throw new InvalidOperationException($"{nameof(PrinceProcessManager)} is not initialized. Make sure to call {nameof(Initialize)} first.");

        CreateProcess();
        StartProcess();
    }

    public void Stop()
    {
        if (_princeProcess == null)
            return;

        _logger?.LogInformation("Stopping external Prince process {ProcessId}", _princeProcess.Id);

        // this does not end the external prince process, but it should be sent
        _communicator.SendEndAsync();

        try
        {
            // terminate the external prince process
            _princeProcess.Kill();
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, "Error stopping the external Prince process {ProcessId}. Abandoning process.", _princeProcess.Id);
        }
        finally
        {
            _princeProcess.Dispose();
            _princeProcess = null;
        }
    }

    private string DeterminePrinceExecutablePath()
    {
        string? result;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var architecturePart = (RuntimeInformation.OSArchitecture == Architecture.X64)
                ? "win64"
                : "win32";

            result = _path.Combine(_configuration.PrinceExecutableBasePath, architecturePart, "bin", "prince.exe");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            result = _path.Combine(_configuration.PrinceExecutableBasePath, "macos/lib/prince/bin", "prince");
        }
        else
        {
            throw new InvalidOperationException("Currently only Windows and macOS are supported.");
        }

        if (!_file.Exists(result))
        {
            throw new InvalidOperationException("Prince executable was not found at specified location: " + _princePath);
        }

        return result;
    }

    private void CreateProcess()
    {
        _princeProcess = new Process()
        {
            StartInfo = BuildStartInfo()
        };
    }

    private ProcessStartInfo BuildStartInfo()
    {
        var arguments = _configuration.GetPrinceStartArguments(true);
        _logger?.LogInformation("Starting external Prince process with arguments: {PrinceArguments}", arguments);

        return new ProcessStartInfo()
        {
            FileName = _princePath,
            WorkingDirectory = _directory.GetCurrentDirectory(),
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
    }

    private void StartProcess()
    {
        if (_princeProcess == null)
            throw new InvalidOperationException("Create the prince process first");

        try
        {
            if (_princeProcess.Start())
            {
                // _princeProcess.StandardError is currently not used
                _communicator.Initialize(new StreamWriterAdapter(_princeProcess.StandardInput), new StreamReaderAdapter(_princeProcess.StandardOutput));

                _logger?.LogInformation("External Prince process {ProcessId} was started ", _princeProcess.Id);

                // this needs to be awaited, because the single chunks need to be read sequentially.
                var version = _communicator.ReadVersionChunkAsync().GetAwaiter().GetResult();
                _logger?.LogInformation("External Prince process has version {PrinceVersion}", version);

                if (_logger != null)
                {
                    Task.Run(LogProcessInfo);
                }
            }
            else
            {
                throw new Exception("Prince process could not be started.");
            }
        }
        catch (Exception ex)
        {
            _princeProcess = null;
            _logger?.LogError(ex, "Error while starting prince process.");
            throw;
        }
    }

    private void LogProcessInfo()
    {
        if (_princeProcess != null)
        {
            _logger?.LogInformation("External Prince process info: Process {ProcessId} running for {ProcessRuntime}, Working set: {CurrentWorkingSet} (max was {MaxWorkingSet}), currently using {Threads} threads, consumed {TotalProcessorTime} processor time",
                _princeProcess.Id,
                DateTime.Now - _princeProcess.StartTime,
                _princeProcess.WorkingSet64,
                _princeProcess.MaxWorkingSet,
                _princeProcess.Threads.Count,
                _princeProcess.TotalProcessorTime
            );

            Task.Delay(_configuration.ProcessLoggingInterval).ContinueWith(_ => LogProcessInfo());
        }
        else
        {
            _logger?.LogDebug("Process has stopped. Stopping logging of process info.");
        }
    }

    #region Dispose pattern

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Stop();
            _communicator.Dispose();
        }
    }

    #endregion Dispose pattern
}
