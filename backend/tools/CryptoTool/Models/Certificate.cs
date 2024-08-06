// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;

namespace CryptoTool.Models;

internal class Certificate
{
    public Certificate(ICertificate certificate)
    {
        Subject = certificate.Subject;
        Thumbprint = certificate.Thumbprint;
        CommonName = certificate.CommonName;
    }

    public string Subject { get; init; }

    public string CommonName { get; init; }

    public string Thumbprint { get; init; }
}
