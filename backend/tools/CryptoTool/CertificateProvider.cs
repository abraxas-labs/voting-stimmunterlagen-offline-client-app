// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;

namespace CryptoTool;

public class CertificateProvider : ICertificateProvider
{
    private readonly Arguments _arguments;
    private readonly WindowsCertificateService _windowsCertificateService;

    public ICertificate? SenderCertificate { get; private set; }

    public IReadOnlyCollection<ICertificate> ReceiverCertificates { get; private set; } = null!;

    public CertificateProvider(Arguments arguments, WindowsCertificateService windowsCertificateService)
    {
        _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        _windowsCertificateService = windowsCertificateService;
    }

    public void InitializeCertificates()
    {
        SenderCertificate = GetSenderCertificate();
        ReceiverCertificates = GetReceiverCertificates();
    }

    private ICertificate? GetSenderCertificate()
    {
        if (!string.IsNullOrWhiteSpace(_arguments.SenderCertificateFile))
        {
            return BouncyCastleCertificateParser.ParseP12Certificate(File.ReadAllBytes(_arguments.SenderCertificateFile), _arguments.SenderCertificateFilePassword);
        }

        if (string.IsNullOrWhiteSpace(_arguments.SenderCertificateSubject))
        {
            return null;
        }

        var certificate = _windowsCertificateService
                   .GetPrivateSigningCertificates()
                   .FirstOrDefault(c => c.Subject.Equals(_arguments.SenderCertificateSubject, StringComparison.InvariantCultureIgnoreCase))
               ?? throw new InvalidOperationException("Sender certificate subject provided but certificate not found");
        return new NativeCertificate(certificate);
    }

    private List<ICertificate> GetReceiverCertificates()
    {
        return _arguments.ReceiverCertificateFiles
            .Select(receiverCertificateFile => BouncyCastleCertificateParser.ParsePemCertificate(File.ReadAllBytes(receiverCertificateFile)))
            .OfType<ICertificate>()
            .ToList();
    }
}
