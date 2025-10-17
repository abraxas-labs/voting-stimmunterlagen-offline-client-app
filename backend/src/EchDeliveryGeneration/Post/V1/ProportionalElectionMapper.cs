// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using EchDeliveryGeneration.ErrorHandling;
using System.Linq;
using Ech0155_4_0;
using Ech0228_1_0;
using Voting.Lib.Common;

namespace EchDeliveryGeneration.Post.V1;

public class ProportionalElectionMapper
{
    public void MapToEchElection(ElectionInformationType election, EVoting.Print_1_3.ElectionType electionTypeSource, EVoting.Config_6_0.ElectionInformationType config)
    {
        election.List = electionTypeSource.List.Select(list => MapToEchList(list, config)).ToList();
    }

    private ElectionInformationTypeList MapToEchList(EVoting.Print_1_3.ListType listSource, EVoting.Config_6_0.ElectionInformationType config)
    {
        var sourceConfig = config.List.SingleOrDefault(x => x.ListIdentification == listSource.ListIdentification)
            ?? throw new TransformationException(TransformationErrorCode.ListNotFound, listSource.ListIdentification);

        var listDescriptionList = new List<ListDescriptionInformationTypeListDescriptionInfo>();
        var listUnionDescriptionList = new List<ListUnionBallotTextTypeListUnionBallotTextInfo>();

        foreach (var description in sourceConfig.ListDescription)
        {
            listDescriptionList.Add(new ListDescriptionInformationTypeListDescriptionInfo
            {
                Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(description.Language),
                ListDescription = description.ListDescription,
                ListDescriptionShort = description.ListDescriptionShort,
            });
        }

        var listUnion = config.ListUnion?.SingleOrDefault
        (x =>
            x.ListUnionTypeProperty == EVoting.Config_6_0.ListUnionTypeListUnionType.Item1 &&
            x.ReferencedList.Any(y => y == listSource.ListIdentification)
        );

        if (listUnion != null && listUnion.ListUnionDescription != null &&
            listUnion.ListUnionDescription.Count > 0)
        {
            foreach (var listUnionDescriptionInfo in listUnion.ListUnionDescription)
            {
                listUnionDescriptionList.Add(new ListUnionBallotTextTypeListUnionBallotTextInfo
                {
                    Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(listUnionDescriptionInfo.Language),
                    ListUnionBalloText = listUnionDescriptionInfo.ListUnionDescription,
                });
            }
        }

        return new ElectionInformationTypeList
        {
            IsEmptyList = sourceConfig.ListEmpty,
            ListDescription = listDescriptionList,
            ListIdentification = listSource.ListIdentification,
            ListIndentureNumber = sourceConfig.ListIndentureNumber,
            ListOrderOfPrecedence = sourceConfig.ListOrderOfPrecedence.ToString(),
            IndividualListVerificationCodes = new List<NamedCodeType>
            {
                new() { CodeDesignation = EchDeliveryGenerationConstants.ChoiceCode, CodeValue = listSource.ChoiceReturnCode}
            },
            ListUnionBallotText = listUnionDescriptionList,
            CandidatePosition = listSource.Candidate
                .Select(c => MapToEchCandidate(c, config, sourceConfig))
                // a candidate must only appear once, even if they are accumulated. If it is accumulated there are multiple choice codes available on a single candidate.
                .DistinctBy(c => c.CandidateReferenceOnPosition)
                .ToList(),
        };
    }

    private ElectionInformationTypeListCandidatePosition MapToEchCandidate(EVoting.Print_1_3.CandidateListType candidateListType, EVoting.Config_6_0.ElectionInformationType config, EVoting.Config_6_0.ListType sourceConfig)
    {
        var listCandidatesChoiceCodes = new List<NamedCodeType>();
        var candidateListConfiguration = sourceConfig.CandidatePosition.FirstOrDefault(x =>
            x.CandidateListIdentification == candidateListType.CandidateListIdentification)
            ?? throw new TransformationException(TransformationErrorCode.CandidateNotFound, candidateListType.CandidateListIdentification);

        var candidateOccurrences = 1;

        if (!sourceConfig.ListEmpty)
            candidateOccurrences = sourceConfig.CandidatePosition.Count(x =>
                x.CandidateIdentification == candidateListConfiguration.CandidateIdentification);

        foreach (var choiceCode in candidateListType.ChoiceReturnCode)
        {
            listCandidatesChoiceCodes.Add(new NamedCodeType()
            {
                CodeDesignation = EchDeliveryGenerationConstants.ChoiceCode,
                CodeValue = choiceCode
            });
        }

        return new ElectionInformationTypeListCandidatePosition
        {
            CandidateIdentification = candidateListConfiguration.CandidateIdentification,
            CandidateReferenceOnPosition = candidateListConfiguration.CandidateReferenceOnPosition,
            PositionOnList = candidateListConfiguration.PositionOnList.ToString(),
            Occurences = candidateOccurrences.ToString(),
            IndividualCandidateVerificationCode = listCandidatesChoiceCodes,
        };
    }
}
