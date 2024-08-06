// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.Logging;

namespace ZipTool;

public class Application
{
    private readonly ILogger<Application> _logger;
    private readonly Arguments _arguments;


    public Application(ILogger<Application> logger, Arguments arguments)
    {
        _logger = logger;
        _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
    }

    public void Run()
    {
        ZipOrUnzipData();
    }

    private void ZipOrUnzipData()
    {
        try
        {
            for (var i = 0; i < _arguments.Input.Count; i++)
            {
                var input = _arguments.Input[i];
                var output = _arguments.Output[i];

                var isDirectory = (File.GetAttributes(input) & FileAttributes.Directory) == FileAttributes.Directory;

                if (isDirectory)
                {
                    ZipData(input, output);
                }
                else if (Path.GetExtension(input).Equals(".zip", StringComparison.CurrentCultureIgnoreCase))
                {
                    UnzipData(input, output);
                }
                else
                {
                    throw new ArgumentException($"the input path {input} is not valid. Could not determine if you want to zip or unzip");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while zipping or unzipping the files.");
            throw;
        }
    }

    private void ZipData(string inputDirectory, string outFile)
    {
        _logger.LogInformation("Zipping {inputDirectory} to {outFile}.", inputDirectory, outFile);

        var outputDirectory = Path.GetDirectoryName(outFile)
                              ?? throw new ArgumentException($"Cannot get directory name from {nameof(outFile)} {outFile}");

        if (!Directory.Exists(outputDirectory))
        {
            _logger.LogWarning("The outputDirectory {outputDirectory} for the zip file {outFile} does not exist and will be created", outputDirectory, outFile);
            Directory.CreateDirectory(outputDirectory);
        }
        ZipFile.CreateFromDirectory(inputDirectory, outFile);
    }

    private void UnzipData(string inFile, string outDirectory)
    {
        _logger.LogInformation("Unzipping file {inFile} to {outDirectory}.", inFile, outDirectory);

        var isDirectory = (File.GetAttributes(outDirectory) & FileAttributes.Directory) == FileAttributes.Directory;

        if (!isDirectory)
            throw new ArgumentException($"The output path {outDirectory} is not a directory");
        if (!Directory.Exists(outDirectory))
        {
            _logger.LogWarning("The output path {outDirectory} does not exist and will be created", outDirectory);
            Directory.CreateDirectory(outDirectory);
        }

        ZipFile.ExtractToDirectory(inFile, outDirectory);
    }
}
