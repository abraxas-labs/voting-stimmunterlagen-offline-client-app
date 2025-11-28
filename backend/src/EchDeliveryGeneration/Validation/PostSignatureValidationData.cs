// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using EchDeliveryGeneration.Post;

namespace EchDeliveryGeneration.Validation
{
    public class PostSignatureValidationData
    {
        public string PostConfigPath { get; set; } = string.Empty;

        public string PostPrintPath { get; set; } = string.Empty;

        public PostPrintVersion PostPrintVersion { get; set; } = PostPrintVersion.Unspecified;

        public string ValidatorPath { get; set; } = string.Empty;

        public string KeystoreCertificatePath { get; set; } = string.Empty;

        public string KeystorePasswordPath { get; set; } = string.Empty;

        public string JavaRuntimePath { get; set; } = string.Empty;

        public bool RequiredFieldsSet() =>
            !string.IsNullOrWhiteSpace(PostConfigPath)
            && !string.IsNullOrWhiteSpace(PostPrintPath)
            && !string.IsNullOrWhiteSpace(ValidatorPath)
            && !string.IsNullOrWhiteSpace(KeystoreCertificatePath)
            && !string.IsNullOrWhiteSpace(KeystorePasswordPath)
            && !string.IsNullOrWhiteSpace(JavaRuntimePath)
            && PostPrintVersion != PostPrintVersion.Unspecified;
    }
}
