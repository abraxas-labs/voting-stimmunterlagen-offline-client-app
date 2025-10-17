// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;
using EchDeliveryGeneration.Validation;
using EchDeliveryGeneration.Models;
using System;
using Voting.Lib.Ech;
using EchDeliveryGeneration.Post;
using System.Linq;
using EchDeliveryGeneration.Ech0045;
using Microsoft.Extensions.DependencyInjection;

namespace EchDeliveryGeneration;

public class EchDeliveryGenerator
{
    private readonly PostDataTransformerAdapter _postDataTransformer;
    private readonly PostSignatureValidator _postSignatureValidator;
    private readonly IServiceProvider _sp;


    public EchDeliveryGenerator(
        PostDataTransformerAdapter postDataTransformer,
        PostSignatureValidator postSignatureValidator,
        IServiceProvider sp)
    {
        _postDataTransformer = postDataTransformer;
        _postSignatureValidator = postSignatureValidator;
        _sp = sp;
    }

    public async Task<EchDeliveryGeneratorResult> GenerateDelivery(
        IReadOnlyCollection<string> files,
        PostSignatureValidationData? postSignatureValidationData)
    {
        var postSignatureValidationResult = new PostSignatureValidationResult(PostSignatureValidationResultCodes.Skipped);
        postSignatureValidationData ??= new();

        var jsonConfig = new Configuration();
        var dataTransformerContext = new PostDataTransformerContext();

        foreach (var file in files)
        {
            if (file.EndsWith(".xml"))
            {
                using var stream = File.OpenRead(file);
                var schema = EchSchemaFinder.GetSchema(stream, XmlSchemas.All);

                if (XmlSchemas.PostConfigSchemas.Contains(schema))
                {
                    dataTransformerContext.PostConfigPath = file;
                    postSignatureValidationData.PostConfigPath = file;
                    dataTransformerContext.PostConfigVersion = schema == XmlSchemas.PostConfigV6Schema
                        ? PostConfigVersion.V6
                        : PostConfigVersion.V7;
                }
                else if (XmlSchemas.PostPrintSchemas.Contains(schema))
                {
                    dataTransformerContext.PostPrintPath = file;
                    postSignatureValidationData.PostPrintPath = file;
                    dataTransformerContext.PostPrintVersion = schema == XmlSchemas.PostPrintV1Schema
                        ? PostPrintVersion.V1
                        : PostPrintVersion.V2;
                }
                else if (XmlSchemas.Ech0045Schemas.Contains(schema))
                {
                    var version = schema == XmlSchemas.Ech0045V4Schema
                        ? Ech0045Version.V4
                        : Ech0045Version.V6;

                    var ech0045Reader = _sp.GetRequiredKeyedService<IEch0045Reader>(version);
                    dataTransformerContext.EchVoterByPersonId = await ech0045Reader.ReadVoterExtensions(stream);

                }
            }
            else if (file.EndsWith(".json"))
            {
                using var stream = File.OpenText(file);

                JsonSerializer serializer = new();
                dataTransformerContext.JsonConfig = (Configuration?)serializer.Deserialize(stream, typeof(Configuration));
            }
        }

        var delivery = _postDataTransformer.Transform(dataTransformerContext);

        if (postSignatureValidationData.RequiredFieldsSet())
        {
            postSignatureValidationResult = await _postSignatureValidator.Validate(postSignatureValidationData);
        }


        return new EchDeliveryGeneratorResult(delivery, postSignatureValidationResult);
    }
}
