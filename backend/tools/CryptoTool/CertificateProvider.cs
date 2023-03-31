using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography;

namespace CryptoTool;

public class CertificateProvider : ICertificateProvider
{
    private readonly Arguments _arguments;
    private readonly WindowsCertificateService _windowsCertificateService;

    public X509Certificate2? SenderCertificate { get; private set; }
    public IReadOnlyCollection<X509Certificate2> ReceiverCertificates { get; private set; } = null!;

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

    private X509Certificate2? GetSenderCertificate()
    {
        if (!string.IsNullOrWhiteSpace(_arguments.SenderCertificateFile))
        {
            return new X509Certificate2(_arguments.SenderCertificateFile, _arguments.SenderCertificateFilePassword);
        }

        if (string.IsNullOrWhiteSpace(_arguments.SenderCertificateSubject))
        {
            return null;
        }

        return _windowsCertificateService
                   .GetPrivateSigningCertificates()
                   .FirstOrDefault(c => c.Subject.Equals(_arguments.SenderCertificateSubject, StringComparison.InvariantCultureIgnoreCase))
               ?? throw new InvalidOperationException("Sender certificate subject provided but certificate not found");
    }

    private List<X509Certificate2> GetReceiverCertificates()
    {
        return _arguments.ReceiverCertificateFiles
            .ConvertAll(receiverCertificateFile => new X509Certificate2(receiverCertificateFile));
    }
}
