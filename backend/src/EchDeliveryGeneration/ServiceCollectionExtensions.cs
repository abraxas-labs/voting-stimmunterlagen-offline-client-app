// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using EchDeliveryGeneration;
using EchDeliveryGeneration.Ech0045;
using EchDeliveryGeneration.Post;
using EchDeliveryGeneration.Validation;
using Voting.Lib.Ech.Ech0045_4_0.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEchDeliveryGeneration(this IServiceCollection services)
    {
        return services.AddSingleton<Ech0045Reader>()
            .AddEch0045()
            .AddSingleton<EchDeliveryGenerator>()
            .AddSingleton<PostDataTransformer>()
            .AddSingleton<ContestMapper>()
            .AddSingleton<ElectionGroupBallotMapper>()
            .AddSingleton<ElectionMapper>()
            .AddSingleton<MajorityElectionMapper>()
            .AddSingleton<ProportionalElectionMapper>()
            .AddSingleton<VoteMapper>()
            .AddSingleton<VoterMapper>()
            .AddSingleton<VotingCardMapper>()
            .AddSingleton<PostSignatureValidator>();
    }
}

