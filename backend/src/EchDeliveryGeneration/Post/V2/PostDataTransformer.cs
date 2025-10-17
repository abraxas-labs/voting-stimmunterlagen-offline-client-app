// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Linq;
using Ech0228_1_0;
using EchDeliveryGeneration.ErrorHandling;
using EchDeliveryJsonConverter.Converters;

namespace EchDeliveryGeneration.Post.V2;

public class PostDataTransformer : IPostDataTransformer
{
    private readonly EVotingXmlSerializer _eVotingXmlSerializer;
    private readonly VotingCardMapper _votingCardMapper;
    private readonly ContestMapper _contestMapper;

    public PostDataTransformer(
        EVotingXmlSerializer eVotingXmlSerializer,
        VotingCardMapper votingCardMapper,
        ContestMapper contestMapper)
    {
        _eVotingXmlSerializer = eVotingXmlSerializer;
        _votingCardMapper = votingCardMapper;
        _contestMapper = contestMapper;
    }

    public Delivery Transform(PostDataTransformerContext context)
    {
        var delivery = new Delivery
        {
            VotingCardDelivery = new(),
        };

        var (configuration, votingCardLists) = Deserialize(context);

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
                _votingCardMapper.MapToEchVotingCard(vc, referenceConfig, context.JsonConfig!, context.EchVoterByPersonId)));

            AddContestDataToDelivery(ref delivery, referenceConfig.Contest);
        }
        delivery.VotingCardDelivery.VotingCardData = votingCardDataList;
        return delivery;
    }

    private void AddContestDataToDelivery(ref Delivery delivery, EVoting.Config_7_0.ContestType contest)
    {
        delivery.VotingCardDelivery.ContestData = new ContestDataType
        {
            Contest = _contestMapper.MapToEchContest(contest),
        };
    }

    private (List<EVoting.Config_7_0.Configuration>, List<EVoting.Print_2_0.VotingCardList> votingCardLists) Deserialize(PostDataTransformerContext context)
    {
        var config = new List<EVoting.Config_7_0.Configuration>();
        var votingCardLists = new List<EVoting.Print_2_0.VotingCardList>();

        config.Add(_eVotingXmlSerializer.DeserializeXml<EVoting.Config_7_0.Configuration>(context.PostConfigPath!));
        votingCardLists.Add(_eVotingXmlSerializer.DeserializeXml<EVoting.Print_2_0.VotingCardList>(context.PostPrintPath!));

        return (config, votingCardLists);
    }
}
