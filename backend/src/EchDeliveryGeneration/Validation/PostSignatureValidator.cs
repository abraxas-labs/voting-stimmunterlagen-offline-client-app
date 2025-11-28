// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using EchDeliveryGeneration.Post;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ArgumentException = System.ArgumentException;

namespace EchDeliveryGeneration.Validation
{
    public class PostSignatureValidator
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public async Task<PostSignatureValidationResult> Validate(PostSignatureValidationData data)
        {
            var configValidationResult = await Validate(data, PostSignatureValidationType.Config);
            if (configValidationResult.Code != PostSignatureValidationResultCodes.Success)
            {
                return configValidationResult;
            }

            return await Validate(data, PostSignatureValidationType.Print);
        }

        private async Task<PostSignatureValidationResult> Validate(PostSignatureValidationData data, PostSignatureValidationType validationType)
        {
            var fileNotExistsValidationResult = EnsureFilesExist(data);
            if (fileNotExistsValidationResult != null)
            {
                return fileNotExistsValidationResult;
            }

            PostSignatureValidationCliResult? cliResult;

            try
            {
                var process = new Process
                {
                    StartInfo = BuildStartInfo(data, validationType),
                };

                process.Start();
                cliResult = JsonSerializer.Deserialize<PostSignatureValidationCliResult>(
                    await process.StandardOutput.ReadToEndAsync(),
                    _jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                cliResult = new PostSignatureValidationCliResult { ExceptionMessage = ex.Message };
            }

            if (cliResult?.SignatureVerified == true)
            {
                return new PostSignatureValidationResult(PostSignatureValidationResultCodes.Success, string.Empty);
            }

            return new PostSignatureValidationResult(PostSignatureValidationResultCodes.ValidationError, cliResult?.ExceptionMessage ?? "Unknown Error");
        }
        private ProcessStartInfo BuildStartInfo(PostSignatureValidationData data, PostSignatureValidationType validationType)
        {
            var sb = new StringBuilder();

            // Set system properties based on PostPrintVersion
            if (data.PostPrintVersion == PostPrintVersion.V1)
            {
                // Version V1 is compatible with XML Sign Tool [1.4,1.5)
                sb.Append($"-Ddirect.trust.keystore.location=\"{data.KeystoreCertificatePath}\" ");
                sb.Append($"-Ddirect.trust.keystore.password.location=\"{data.KeystorePasswordPath}\" ");
            }
            else
            {
                // Version V2+ is compatible with XML Sign Tool [1.5,)
                sb.Append($"-Ddirect-trust.keystore.location=\"{data.KeystoreCertificatePath}\" ");
                sb.Append($"-Ddirect-trust.password.location=\"{data.KeystorePasswordPath}\" ");
            }

            sb.Append($"-jar \"{data.ValidatorPath}\" ");

            switch (validationType)
            {
                case PostSignatureValidationType.Config:
                    sb.Append($"CONFIG VERIFY \"{data.PostConfigPath}\" ");
                    break;
                case PostSignatureValidationType.Print:
                    sb.Append($"PRINT VERIFY \"{data.PostPrintPath}\" ");
                    break;
                default:
                    throw new ArgumentException("Invalid validation type");
            }

            var arguments = sb.ToString();

            return new ProcessStartInfo
            {
                FileName = data.JavaRuntimePath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
        }

        private PostSignatureValidationResult? EnsureFilesExist(PostSignatureValidationData args)
        {
            if (!File.Exists(args.JavaRuntimePath))
            {
                return new PostSignatureValidationResult(PostSignatureValidationResultCodes.FileNotFound, nameof(args.JavaRuntimePath));
            }

            if (!File.Exists(args.ValidatorPath))
            {
                return new PostSignatureValidationResult(PostSignatureValidationResultCodes.FileNotFound, nameof(args.ValidatorPath));
            }

            if (!File.Exists(args.KeystoreCertificatePath))
            {
                return new PostSignatureValidationResult(PostSignatureValidationResultCodes.FileNotFound, nameof(args.KeystoreCertificatePath));
            }

            if (!File.Exists(args.KeystorePasswordPath))
            {
                return new PostSignatureValidationResult(PostSignatureValidationResultCodes.FileNotFound, nameof(args.KeystorePasswordPath));
            }

            if (!File.Exists(args.PostConfigPath))
            {
                return new PostSignatureValidationResult(PostSignatureValidationResultCodes.FileNotFound, nameof(args.PostConfigPath));
            }

            if (!File.Exists(args.PostPrintPath))
            {
                return new PostSignatureValidationResult(PostSignatureValidationResultCodes.FileNotFound, nameof(args.PostPrintPath));
            }

            return null;
        }
    }
}
