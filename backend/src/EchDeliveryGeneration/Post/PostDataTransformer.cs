using System.Collections.Generic;
using System.Linq;
using eCH_0228;
using Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;
using EchDeliveryGeneration.Ech0045;
using EchDeliveryGeneration.ErrorHandling;
using EchDeliveryGeneration.Models;

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
        List<configuration> configuration,
        List<votingCardList> votingCardList,
        Configuration jsonConfig,
        Dictionary<string, Ech0045VoterExtension> echVoterByPersonId)
    {
        var delivery = new Delivery
        {
            VotingCardDelivery = new(),
        };

        var votingCardDataList = new List<votingCardDataType>();

        foreach (var cardList in votingCardList)
        {
            var referencesConfigs = configuration
                .Where(x => x.contest.contestIdentification == cardList.contest.contestIdentification)
                .ToList();

            if (referencesConfigs.Count > 1)
                throw new TransformationException(TransformationErrorCode.ContestDuplicates, cardList.contest.contestIdentification);

            var referenceConfig = referencesConfigs.SingleOrDefault()
                ?? throw new TransformationException(TransformationErrorCode.ContestNotFound, cardList.contest.contestIdentification);

            votingCardDataList.AddRange(cardList.contest.votingCard.Select(vc =>
                _votingCardMapper.MapToEchVotingCard(vc, referenceConfig, jsonConfig, echVoterByPersonId)));

            AddContestDataToDelivery(ref delivery, referenceConfig.contest);
        }
        delivery.VotingCardDelivery.VotingCardData = votingCardDataList;
        SetDeliveryExtension(delivery, jsonConfig);
        return delivery;
    }

    private void AddContestDataToDelivery(ref Delivery delivery, contestType contest)
    {
        delivery.VotingCardDelivery.ContestData = new ContestDataType()
        {
            Item = _contestMapper.MapToEchContest(contest),
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
            if (!deliveryExtension.Printings.ContainsKey(jsonConfigurationPrinting.Name))
                deliveryExtension.Printings.Add(jsonConfigurationPrinting.Name, new SmallPrinting(jsonConfigurationPrinting));

            foreach (var municipality in jsonConfigurationPrinting.Municipalities)
            {
                if (!deliveryExtension.Municipalities.ContainsKey(municipality.Bfs))
                    deliveryExtension.Municipalities.Add(municipality.Bfs, municipality);
            }
        }

        delivery.Extension = deliveryExtension;
    }
}
