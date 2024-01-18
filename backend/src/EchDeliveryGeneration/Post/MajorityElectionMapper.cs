using EchDeliveryGeneration.ErrorHandling;
using System.Collections.Generic;
using System.Linq;
using Ech0045_4_0;
using Ech0155_4_0;
using Ech0228_1_0;
using Voting.Lib.Common;

namespace EchDeliveryGeneration.Post;

public class MajorityElectionMapper
{
    public void MapToEchElection(ElectionInformationType election, EVoting.Print.ElectionType electionTypeSource, EVoting.Config.ElectionInformationType config)
    {
        var candidates = electionTypeSource.Candidate.ToList();

        if (electionTypeSource.List != null)
        {
            candidates.AddRange(electionTypeSource.List
                .SelectMany(l => l.Candidate?.ToList() ?? new List<EVoting.Print.CandidateListType>())
                .Select(c => new EVoting.Print.CandidateType { CandidateIdentification = c.CandidateListIdentification, ChoiceReturnCode = c.ChoiceReturnCode }));
        }

        election.Candidate = candidates
            .Select(candidate => MapToEchCandidate(candidate, electionTypeSource, config))
            .ToList();

        election.WriteInCodes = electionTypeSource.WriteInCandidate
            .Select((writeInCandidate, index) => MapToEchWriteInCodes(index, writeInCandidate))
            .ToList();
    }

    private ElectionInformationTypeCandidate MapToEchCandidate(EVoting.Print.CandidateType candidateType, EVoting.Print.ElectionType electionTypeSource, EVoting.Config.ElectionInformationType config)
    {
        var candidateConfig = config.Candidate.SingleOrDefault(x =>
            x.CandidateIdentification == candidateType.CandidateIdentification);

        if (candidateConfig != null)
        {
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
        else
        {
            var emptyCandidateConfig = config.List.FirstOrDefault()?.CandidatePosition?.FirstOrDefault(
                x => x.CandidateListIdentification == candidateType.CandidateIdentification)
                ?? throw new TransformationException(TransformationErrorCode.CandidateNotFound, candidateType.CandidateIdentification);

            var candidateTextOnPositionList = emptyCandidateConfig.CandidateTextOnPosition
                .Select(x => new CandidateTextInformationTypeCandidateTextInfo
                {
                    Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(x.Language),
                    CandidateText = x.CandidateText,
                })
                .ToList();

            var choiceCodes = candidateType.ChoiceReturnCode
                .Select(code => new NamedCodeType
                {
                    CodeDesignation = EchDeliveryGenerationConstants.ChoiceCode,
                    CodeValue = code
                })
                .ToList();

            return new ElectionInformationTypeCandidate
            {
                CandidateIdentification = emptyCandidateConfig.CandidateIdentification,
                CandidateText = candidateTextOnPositionList,
                CandidateReference = new List<ElectionInformationTypeCandidateCandidateReference>
                {
                    new()
                    {
                        CandidateReferenceOnPosition = emptyCandidateConfig.CandidateReferenceOnPosition,
                        CandidateTextOnPosition = candidateTextOnPositionList,
                        Occurences = electionTypeSource.Candidate.Select(x =>
                                x.CandidateIdentification == candidateType.CandidateIdentification)
                            .Count()
                            .ToString(),
                        IndividualCandidateVerificationCode = choiceCodes,
                    },
                },
            };
        }
    }

    private ElectionInformationTypeWriteInCodes MapToEchWriteInCodes(int index, EVoting.Print.WriteInCandidateType writeInCandidate)
    {
        return new ElectionInformationTypeWriteInCodes
        {
            Position = (index + 1).ToString(),
            IndividualWriteInVerificationCode = writeInCandidate.ChoiceReturnCode,
            WriteInCodeDesignation = new List<ElectionInformationTypeWriteInCodesWriteInCodeDesignation>
            {
                new() { Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(LanguageType.De), CodeDesignationText = "Anderer wählbarer Kandidat"},
            }
        };
    }
}
