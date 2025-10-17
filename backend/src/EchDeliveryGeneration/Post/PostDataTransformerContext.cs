// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using EchDeliveryGeneration.Ech0045;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

namespace EchDeliveryGeneration.Post;

public class PostDataTransformerContext
{
    public string? PostConfigPath { get; set; }

    public PostConfigVersion PostConfigVersion { get; set; }

    public string? PostPrintPath { get; set; }

    public PostPrintVersion PostPrintVersion { get; set; }

    public Dictionary<string, Ech0045VoterExtension>? EchVoterByPersonId { get; set; }

    public Configuration? JsonConfig { get; set; }

    public void EnsureIsValid()
    {
        if (JsonConfig == null)
        {
            throw new ValidationException("Json config file is required");
        }

        if (PostConfigVersion is PostConfigVersion.Unspecified || PostPrintVersion is PostPrintVersion.Unspecified)
        {
            throw new ValidationException("Post print and post config files are required");
        }

        if (!((PostConfigVersion is PostConfigVersion.V6 && PostPrintVersion is PostPrintVersion.V1)
            || (PostConfigVersion is PostConfigVersion.V7 && PostPrintVersion is PostPrintVersion.V2)))
        {
            throw new ValidationException("Post print version is not supported with the provided post config version");
        }
    }
}
