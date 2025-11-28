// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using EchDeliveryGeneration.ErrorHandling;
using System.Collections.Generic;
using System.Linq;
using Ech0045_4_0;
using Ech0155_4_0;
using Ech0228_1_0;
using Voting.Lib.Common;

namespace EchDeliveryGeneration.Post.V2;

public class MajorityElectionMapper
{
    private const string DefaultWriteInCodeDesignation = "Andere wählbare Person";

    public void MapToEchElection(ElectionInformationType election, EVoting.Print_2_0.ElectionType electionTypeSource, EVoting.Config_7_0.ElectionInformationType config)
    {
        var candidates = electionTypeSource.Candidate.ToList();
        var multipleNumberOfMandates = int.TryParse(election.Election.NumberOfMandates, out var numberOfMandates) && numberOfMandates > 1;

        if (electionTypeSource.List != null)
        {
            candidates.AddRange(electionTypeSource.List
                .SelectMany(l => l.Candidate?.ToList() ?? new List<EVoting.Print_2_0.CandidateListType>())
                .Select(c => new EVoting.Print_2_0.CandidateType { CandidateIdentification = c.CandidateListIdentification, ChoiceReturnCode = c.ChoiceReturnCode }));
        }

        election.Candidate = candidates
            .Select(candidate => MapToEchCandidate(candidate, electionTypeSource, config, multipleNumberOfMandates))
            .ToList();

        if (electionTypeSource.EmptyList != null)
        {
            var emptyCandidates = electionTypeSource.EmptyList.EmptyPosition
                .Select(p => MapToEchCandidate(p, electionTypeSource, config, multipleNumberOfMandates))
                .ToList();

            election.Candidate.AddRange(emptyCandidates);
        }

        election.WriteInCodes = electionTypeSource.WriteInPosition
            .Select((writeInPosition, index) => MapToEchWriteInCodes(index, writeInPosition, multipleNumberOfMandates))
            .ToList();
    }

    private ElectionInformationTypeCandidate MapToEchCandidate(
        EVoting.Print_2_0.CandidateType candidateType,
        EVoting.Print_2_0.ElectionType electionTypeSource,
        EVoting.Config_7_0.ElectionInformationType config,
        bool multipleNumberOfMandates)
    {
        var candidateConfig = config.Candidate.SingleOrDefault(x =>
            x.CandidateIdentification == candidateType.CandidateIdentification)
            ?? throw new TransformationException(TransformationErrorCode.CandidateNotFound, candidateType.CandidateIdentification);

        var candidateTextOnPositionList = candidateConfig.CandidateText
            .Select(x => new CandidateTextInformationTypeCandidateTextInfo
            {
                Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(x.Language),
                CandidateText = x.CandidateText,
            })
            .ToList();

        var candidateText = candidateConfig.FamilyName + " " + candidateConfig.CallName;
        var candidateTextOnPosition = Languages.All
            .Select(lang => new CandidateTextInformationTypeCandidateTextInfo
            {
                Language = lang,
                CandidateText = candidateText,
            })
            .ToList();

        return new ElectionInformationTypeCandidate
        {
            CandidateIdentification = candidateConfig.CandidateIdentification,
            CandidateText = candidateTextOnPositionList,
            CandidateReference = new List<ElectionInformationTypeCandidateCandidateReference>
                {
                    new()
                    {
                        CandidateReferenceOnPosition = candidateConfig.ReferenceOnPosition,
                        CandidateTextOnPosition = candidateTextOnPosition,
                        Occurences = electionTypeSource.Candidate.Select(x =>
                                x.CandidateIdentification == candidateType.CandidateIdentification).Count()
                            .ToString(),
                        IndividualCandidateVerificationCode = new List<NamedCodeType>(candidateType.ChoiceReturnCode
                            .Select(code => new NamedCodeType { CodeDesignation = EchDeliveryGenerationConstants.ChoiceCode, CodeValue = code })
                            .ToArray()),
                    },
                },
        };
    }

    private ElectionInformationTypeCandidate MapToEchCandidate(
        EVoting.Print_2_0.EmptyPositionType emptyPosition,
        EVoting.Print_2_0.ElectionType electionTypeSource,
        EVoting.Config_7_0.ElectionInformationType config,
        bool multipleNumberOfMandates)
    {
        var emptyPositionConfig = config.EmptyList.EmptyPosition.SingleOrDefault(x =>
            x.EmptyPositionIdentification == emptyPosition.EmptyPositionIdentification)
            ?? throw new TransformationException(TransformationErrorCode.CandidateNotFound, emptyPosition.EmptyPositionIdentification);

        var emptyPositionTextInfo = emptyPositionConfig.EmptyPositionTextInfo
            .Select(x => new CandidateTextInformationTypeCandidateTextInfo
            {
                Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(x.Language),
                CandidateText = multipleNumberOfMandates ? $"{x.PositionTextLong} Position {emptyPositionConfig.PositionOnList}" : x.PositionTextLong,
            })
            .ToList();

        return new ElectionInformationTypeCandidate
        {
            CandidateIdentification = emptyPositionConfig.EmptyPositionIdentification,
            CandidateText = emptyPositionTextInfo,
            CandidateReference = new List<ElectionInformationTypeCandidateCandidateReference>
                {
                    new()
                    {
                        CandidateReferenceOnPosition = "99." + emptyPositionConfig.PositionOnList.ToString("00"),
                        CandidateTextOnPosition = emptyPositionTextInfo,
                        Occurences = "1",
                        IndividualCandidateVerificationCode = new List<NamedCodeType>
                        {
                            new NamedCodeType { CodeDesignation = EchDeliveryGenerationConstants.ChoiceCode, CodeValue = emptyPosition.ChoiceReturnCode }
                        },
                    },
                },
        };
    }

    private ElectionInformationTypeWriteInCodes MapToEchWriteInCodes(int index, EVoting.Print_2_0.WriteInPositionType writeInCandidate, bool multipleNumberOfMandates)
    {
        var position = (index + 1).ToString();

        return new ElectionInformationTypeWriteInCodes
        {
            Position = position,
            IndividualWriteInVerificationCode = writeInCandidate.ChoiceReturnCode,
            WriteInCodeDesignation = new List<ElectionInformationTypeWriteInCodesWriteInCodeDesignation>
            {
                new() { Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(LanguageType.De), CodeDesignationText = multipleNumberOfMandates
                    ? $"{DefaultWriteInCodeDesignation} Position {position}"
                    : DefaultWriteInCodeDesignation,
                }
            },
        };
    }
}
