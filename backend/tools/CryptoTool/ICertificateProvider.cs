// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;

namespace CryptoTool;

public interface ICertificateProvider
{
    ICertificate? SenderCertificate { get; }
    IReadOnlyCollection<ICertificate> ReceiverCertificates { get; }
    void InitializeCertificates();
}
