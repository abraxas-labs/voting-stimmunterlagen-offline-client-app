// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Linq;
using Ech0228_1_0;
using EchDeliveryGeneration.Ech0045;
using EchDeliveryGeneration.ErrorHandling;
using EchDeliveryGeneration.Models;
using Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

namespace EchDeliveryGeneration.Post;

public class PostDataTransformer
{
    private readonly VotingCardMapper _votingCardMapper;
    private readonly ContestMapper _contestMapper;

    public PostDataTransformer(VotingCardMapper votingCardMapper, ContestMapper contestMapper)
    {
        _votingCardMapper = votingCardMapper;
        _contestMapper = contestMapper;
    }

    public Delivery Transform(
        List<EVoting.Config.Configuration> configuration,
        List<EVoting.Print.VotingCardList> votingCardLists,
        Configuration jsonConfig,
        Dictionary<string, Ech0045VoterExtension> echVoterByPersonId)
    {
        var delivery = new Delivery
        {
            VotingCardDelivery = new(),
        };

        var votingCardDataList = new List<VotingCardDataType>();

        foreach (var votingCardList in votingCardLists)
        {
            var referencesConfigs = configuration
                .Where(x => x.Contest.ContestIdentification == votingCardList.Contest.ContestIdentification)
                .ToList();

            if (referencesConfigs.Count > 1)
                throw new TransformationException(TransformationErrorCode.ContestDuplicates, votingCardList.Contest.ContestIdentification);

            var referenceConfig = referencesConfigs.SingleOrDefault()
                ?? throw new TransformationException(TransformationErrorCode.ContestNotFound, votingCardList.Contest.ContestIdentification);

            votingCardDataList.AddRange(votingCardList.Contest.VotingCard.Select(vc =>
                _votingCardMapper.MapToEchVotingCard(vc, referenceConfig, jsonConfig, echVoterByPersonId)));

            AddContestDataToDelivery(ref delivery, referenceConfig.Contest);
        }
        delivery.VotingCardDelivery.VotingCardData = votingCardDataList;
        SetDeliveryExtension(delivery, jsonConfig);
        return delivery;
    }

    private void AddContestDataToDelivery(ref Delivery delivery, EVoting.Config.ContestType contest)
    {
        delivery.VotingCardDelivery.ContestData = new ContestDataType
        {
            Contest = _contestMapper.MapToEchContest(contest),
        };
    }

    private void SetDeliveryExtension(Delivery delivery, Configuration jsonConfiguration)
    {
        var deliveryExtension = new DeliveryExtension
        {
            Certificates = jsonConfiguration.Certificates,
        };

        foreach (var jsonConfigurationPrinting in jsonConfiguration.Printings)
        {
            foreach (var municipality in jsonConfigurationPrinting.Municipalities)
            {
                deliveryExtension.Municipalities.TryAdd(municipality.Bfs, municipality);
            }
        }

        delivery.VotingCardDelivery.Extension = deliveryExtension;
    }
}
