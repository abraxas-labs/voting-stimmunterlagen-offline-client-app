// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using EchDeliveryGeneration;
using EchDeliveryGeneration.Ech0045;
using EchDeliveryGeneration.Post;
using V1 = EchDeliveryGeneration.Post.V1;
using V2 = EchDeliveryGeneration.Post.V2;
using EchDeliveryGeneration.Validation;
using EchDeliveryJsonConverter.Converters;
using Voting.Lib.Ech.Ech0045_4_0.DependencyInjection;
using Voting.Lib.Ech.Ech0045_6_0.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEchDeliveryGeneration(this IServiceCollection services)
    {
        return services
            .AddKeyedSingleton<IEch0045Reader, Ech0045_4_0_Reader>(Ech0045Version.V4)
            .AddKeyedSingleton<IEch0045Reader, Ech0045_6_0_Reader>(Ech0045Version.V6)
            .AddSingleton<PostDataTransformerAdapter>()
            .AddPostV1Transformer()
            .AddPostV2Transformer()
            .AddEch0045V4()
            .AddEch0045V6()
            .AddSingleton<EchDeliveryGenerator>()
            .AddSingleton<EVotingXmlSerializer>()
            .AddSingleton<PostSignatureValidator>();
    }

    private static IServiceCollection AddPostV1Transformer(this IServiceCollection services)
    {
        return services
            .AddKeyedSingleton<IPostDataTransformer, V1.PostDataTransformer>(PostPrintVersion.V1)
            .AddSingleton<V1.ContestMapper>()
            .AddSingleton<V1.ElectionGroupBallotMapper>()
            .AddSingleton<V1.ElectionMapper>()
            .AddSingleton<V1.MajorityElectionMapper>()
            .AddSingleton<V1.ProportionalElectionMapper>()
            .AddSingleton<V1.VoteMapper>()
            .AddSingleton<V1.VoterMapper>()
            .AddSingleton<V1.VotingCardMapper>();
    }

    private static IServiceCollection AddPostV2Transformer(this IServiceCollection services)
    {
        return services
            .AddKeyedSingleton<IPostDataTransformer, V2.PostDataTransformer>(PostPrintVersion.V2)
            .AddSingleton<V2.ContestMapper>()
            .AddSingleton<V2.ElectionGroupBallotMapper>()
            .AddSingleton<V2.ElectionMapper>()
            .AddSingleton<V2.MajorityElectionMapper>()
            .AddSingleton<V2.ProportionalElectionMapper>()
            .AddSingleton<V2.VoteMapper>()
            .AddSingleton<V2.VoterMapper>()
            .AddSingleton<V2.VotingCardMapper>();
    }
}

