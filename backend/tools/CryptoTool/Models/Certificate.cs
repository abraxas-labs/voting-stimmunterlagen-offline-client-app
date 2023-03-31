using System.Security.Cryptography.X509Certificates;

namespace CryptoTool.Models;

internal class Certificate
{
    public Certificate(X509Certificate2 certificate)
    {
        Subject = certificate.Subject;
        Thumbprint = certificate.Thumbprint;
        CommonName = certificate.GetNameInfo(X509NameType.SimpleName, false);
    }

    public string Subject { get; init; }

    public string CommonName { get; init; }

    public string Thumbprint { get; init; }
}
