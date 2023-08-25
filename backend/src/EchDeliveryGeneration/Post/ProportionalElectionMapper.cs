using eCH_0155_4_0;
using eCH_0228;
using EchDeliveryGeneration.ErrorHandling;
using System.Collections.Generic;
using System.Linq;

namespace EchDeliveryGeneration.Post;

public class ProportionalElectionMapper
{
    public void MapToEchElection(eCH_0228.electionInformationType election, electionType1 electionTypeSource, electionInformationType config)
    {
        election.list = electionTypeSource.list.Select(list => MapToEchList(list, config)).ToArray();
    }

    private electionInformationTypeList MapToEchList(listType1 listSource, electionInformationType config)
    {
        var sourceConfig = config.list.SingleOrDefault(x => x.listIdentification == listSource.listIdentification)
            ?? throw new TransformationException(TransformationErrorCode.ListNotFound, listSource.listIdentification);

        var listDescriptionList = new List<ListDescriptionInfo>();
        int.TryParse(sourceConfig.listOrderOfPrecedence, out var listOrderOfPrecedence);
        var candidatePositions = new List<electionInformationTypeListCandidatePosition>();
        var listUnionDescriptionList = new List<ListUnionDescriptionInfoType>();
        var listUnionDescription = default(ListUnionDescriptionType);

        foreach (var description in sourceConfig.listDescription)
        {
            listDescriptionList.Add(ListDescriptionInfo.Create(description.language.ToString(),
                description.listDescription, description.listDescriptionShort));
        }

        var listUnion = config.listUnion?.SingleOrDefault
        (x =>
            x.listUnionType1 == listUnionTypeListUnionType.Item1 &&
            x.referencedList.Any(y => y == listSource.listIdentification)
        );

        if (listUnion != null && listUnion.listUnionDescription != null &&
            listUnion.listUnionDescription.Length > 0)
        {
            foreach (var listUnionDescriptionInfo in listUnion.listUnionDescription)
            {
                listUnionDescriptionList.Add(ListUnionDescriptionInfoType.Create(
                    listUnionDescriptionInfo.language.ToString(),
                    listUnionDescriptionInfo.listUnionDescription));
            }
            listUnionDescription = ListUnionDescriptionType.Create(listUnionDescriptionList);
        }

        return new()
        {
            IsEmptyList = sourceConfig.listEmpty,
            ListDescriptionInformation = ListDescriptionInformation.Create(listDescriptionList),
            ListIdentification = listSource.listIdentification,
            ListIndentureNumber = sourceConfig.listIndentureNumber,
            ListOrderOfPrecedence = listOrderOfPrecedence,
            individualListVerificationCodes = new[]
                {new namedCodeType() {codeDesignation = EchDeliveryGenerationConstants.ChoiceCode, codeValue = listSource.choiceReturnCode}},
            ListUnionBallotText = listUnionDescription,
            candidatePosition = listSource.candidate
                .Select(c => MapToEchCandidate(c, config, sourceConfig))
                // a candidate must only appear once, even if they are accumulated. If it is accumulated there are multiple choice codes available on a single candidate.
                .DistinctBy(c => c.CandidateReferenceOnPosition)
                .ToArray(),
        };
    }

    private electionInformationTypeListCandidatePosition MapToEchCandidate(candidateListType candidateListType, electionInformationType config, listType sourceConfig)
    {
        var listCandidatesChoiceCodes = new List<namedCodeType>();
        var candidateListConfiguration = sourceConfig.candidatePosition.FirstOrDefault(x =>
            x.candidateListIdentification == candidateListType.candidateListIdentification)
            ?? throw new TransformationException(TransformationErrorCode.CandidateNotFound, candidateListType.candidateListIdentification);

        var candidateConfiguration = config.candidate.FirstOrDefault(x =>
            x.candidateIdentification == candidateListConfiguration.candidateIdentification);
        int.TryParse(candidateListConfiguration.positionOnList, out var candidatePositionOnList);

        var candidateOccurences = 1;

        if (!sourceConfig.listEmpty)
            candidateOccurences = sourceConfig.candidatePosition.Count(x =>
                x.candidateIdentification == candidateListConfiguration.candidateIdentification);

        foreach (var choiceCode in candidateListType.choiceReturnCode)
        {
            listCandidatesChoiceCodes.Add(new namedCodeType()
            {
                codeDesignation = EchDeliveryGenerationConstants.ChoiceCode,
                codeValue = choiceCode
            });
        }

        return new()
        {
            CandidateIdentification = candidateListConfiguration.candidateIdentification,
            CandidateReferenceOnPosition = candidateListConfiguration.candidateReferenceOnPosition,
            PositionOnList = candidatePositionOnList,
            occurences = candidateOccurences,
            individualCandidateVerificationCode = listCandidatesChoiceCodes.ToArray()
        };
    }
}
