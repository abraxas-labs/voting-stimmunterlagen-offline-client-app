// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace EchDeliveryGeneration.Validation
{
    public class PostSignatureValidationResult
    {
        public PostSignatureValidationResult(string code) : this(code, string.Empty)
        {
        }

        public PostSignatureValidationResult(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public string Code { get; }

        public string Message { get; }
    }
}
