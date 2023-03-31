using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace CryptoTool;

public interface ICertificateProvider
{
    X509Certificate2? SenderCertificate { get; }
    IReadOnlyCollection<X509Certificate2> ReceiverCertificates { get; }
    void InitializeCertificates();
}
