// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace EchDeliveryGeneration.Validation;

internal class PostSignatureValidationCliResult
{
    public bool? SignatureVerified { get; set; }

    public string? ExceptionMessage { get; set; }
}
