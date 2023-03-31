using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Encryption;
using System.Collections.Generic;
using CryptoTool.Models;
using Newtonsoft.Json.Serialization;

namespace CryptoTool;

public class Application
{
    private readonly ILogger<Application> _logger;
    private readonly Arguments _arguments;
    private readonly ICertificateProvider _certificateProvider;
    private readonly JsonSerializerSettings _jsonSerializerSettings = new() { ContractResolver = new CamelCasePropertyNamesContractResolver() };

    private readonly FileEncryptor _fileEncryptor;

    public Application(
        ILogger<Application> logger,
        Arguments arguments,
        ICertificateProvider certificateProvider,
        FileEncryptor fileEncryptor)
    {
        _logger = logger;
        _arguments = arguments;
        _certificateProvider = certificateProvider;
        _fileEncryptor = fileEncryptor;
    }

    public void Initialize()
    {
        _certificateProvider.InitializeCertificates();

        _logger.LogInformation("Sender Certificate Thumbprint: {Thumbprint}", _certificateProvider.SenderCertificate?.Thumbprint);
        foreach (var receiverCertificate in _certificateProvider.ReceiverCertificates)
        {
            _logger.LogInformation("Receiver Certificate Thumbprint: {Thumbprint}", receiverCertificate.Thumbprint);
        }
    }

    public void Run()
    {
        if (_arguments.SenderJsonList)
        {
            Console.WriteLine(JsonConvert.SerializeObject(
                new List<Certificate>
                {
                    new Certificate(_certificateProvider.SenderCertificate!)
                },
                _jsonSerializerSettings));
        }

        if (_arguments.ReceiverJsonList)
        {
            var receiverCertificateSubjects = _certificateProvider.ReceiverCertificates.Select(cert => new Certificate(cert)).ToList();
            Console.WriteLine(JsonConvert.SerializeObject(receiverCertificateSubjects, _jsonSerializerSettings));
        }

        if (_arguments.Encrypt)
        {
            EncryptData();
        }
    }

    private void EncryptData()
    {
        try
        {
            for (var i = 0; i < _arguments.InFiles.Count; i++)
            {
                var inFile = _arguments.InFiles[i];
                var outFile = _arguments.OutFiles[i];

                _logger.LogInformation("Encrypting file {InFile} to {OutFile}.", inFile, outFile);

                File.WriteAllBytes(outFile, _fileEncryptor.Encrypt(File.ReadAllBytes(inFile), _certificateProvider.SenderCertificate!, _certificateProvider.ReceiverCertificates.ToList()));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while encrypting the files.");
            throw;
        }
    }
}
