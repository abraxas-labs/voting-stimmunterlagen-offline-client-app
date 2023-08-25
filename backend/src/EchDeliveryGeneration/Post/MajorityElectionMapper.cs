using eCH_0045_4_0;
using eCH_0155_4_0;
using eCH_0228;
using EchDeliveryGeneration.ErrorHandling;
using System.Collections.Generic;
using System.Linq;
using Voting.Lib.Common;

namespace EchDeliveryGeneration.Post;

public class MajorityElectionMapper
{
    public void MapToEchElection(eCH_0228.electionInformationType election, electionType1 electionTypeSource, electionInformationType config)
    {
        var candidates = electionTypeSource.candidate.ToList();

        if (electionTypeSource.list != null)
        {
            candidates.AddRange(electionTypeSource.list
                .SelectMany(l => l.candidate?.ToList() ?? new List<candidateListType>())
                .Select(c => new candidateType2 { candidateIdentification = c.candidateListIdentification, choiceReturnCode = c.choiceReturnCode }));
        }

        election.candidate = candidates
            .Select(candidate => MapToEchCandidate(candidate, electionTypeSource, config))
            .ToArray();

        election.writeInCodes = electionTypeSource.writeInCandidate
            .Select((writeInCandidate, index) => MapToEchWriteInCodes(index, writeInCandidate))
            .ToArray();
    }

    private electionInformationTypeCandidate MapToEchCandidate(candidateType2 candidateType2, electionType1 electionTypeSource, electionInformationType config)
    {
        var candidateConfig = config.candidate.SingleOrDefault(x =>
            x.candidateIdentification == candidateType2.candidateIdentification);

        if (candidateConfig != null)
        {
            var candidateTextOnPositionList = new List<CandidateTextInfo>();
            var candidateReferenceList = new List<electionInformationTypeCandidateCandidateReference>();


            foreach (var textInfo in candidateConfig.candidateText)
            {
                candidateTextOnPositionList.Add(CandidateTextInfo.Create(textInfo.language.ToString(),
                    textInfo.candidateText));
            }
            var candidateTextInformation = CandidateTextInformation.Create(candidateTextOnPositionList);
            var candidateText = candidateConfig.familyName + " " + candidateConfig.callName;
            var candidateTextOnPosition = CandidateTextInformation.Create(
                Languages.All.Select(lang => CandidateTextInfo.Create(lang, candidateText)).ToList());

            var choiceCodes = new List<namedCodeType>();

            foreach (var code in candidateType2.choiceReturnCode)
            {
                choiceCodes.Add(new namedCodeType() { codeDesignation = EchDeliveryGenerationConstants.ChoiceCode, codeValue = code });
            }

            candidateReferenceList.Add(new electionInformationTypeCandidateCandidateReference()
            {
                CandidateReference = candidateConfig.referenceOnPosition,
                CandidateTextOnPosition = candidateTextOnPosition,
                Occurences = electionTypeSource.candidate.Select(x =>
                        x.candidateIdentification == candidateType2.candidateIdentification).Count()
                    .ToString(),
                individualCandidateVerificationCode = choiceCodes.ToArray(),
            });

            return new()
            {
                CandidateIdentification = candidateConfig.candidateIdentification,
                CandidateTextinformation = candidateTextInformation,
                candidateReference = candidateReferenceList.ToArray(),
            };
        }
        else
        {
            var emptyCandidateConfig = config.list.FirstOrDefault()?.candidatePosition?.FirstOrDefault(
                x => x.candidateListIdentification == candidateType2.candidateIdentification)
                ?? throw new TransformationException(TransformationErrorCode.CandidateNotFound, candidateType2.candidateIdentification);

            var candidateTextOnPositionList = new List<CandidateTextInfo>();
            var candidateReferenceList =
                new List<electionInformationTypeCandidateCandidateReference>();

            foreach (var textInfo in emptyCandidateConfig.candidateTextOnPosition)
            {
                candidateTextOnPositionList.Add(CandidateTextInfo.Create(
                    textInfo.language.ToString(),
                    textInfo.candidateText));
            }
            var candidateTextInformation =
                CandidateTextInformation.Create(candidateTextOnPositionList);

            var choiceCodes = new List<namedCodeType>();

            foreach (var code in candidateType2.choiceReturnCode)
            {
                choiceCodes.Add(new namedCodeType()
                {
                    codeDesignation = EchDeliveryGenerationConstants.ChoiceCode,
                    codeValue = code
                });
            }

            candidateReferenceList.Add(new electionInformationTypeCandidateCandidateReference()
            {
                CandidateReference = emptyCandidateConfig.candidateReferenceOnPosition,
                CandidateTextOnPosition = candidateTextInformation,
                Occurences = electionTypeSource.candidate.Select(x =>
                        x.candidateIdentification == candidateType2.candidateIdentification)
                    .Count()
                    .ToString(),
                individualCandidateVerificationCode = choiceCodes.ToArray(),
            });

            return new()
            {
                CandidateIdentification = emptyCandidateConfig.candidateIdentification,
                CandidateTextinformation = candidateTextInformation,
                candidateReference = candidateReferenceList.ToArray(),
            };
        }
    }

    private electionInformationTypeWriteInCodes MapToEchWriteInCodes(int index, writeInCandidateType1 writeInCandidate)
    {
        return new()
        {
            position = (index + 1).ToString(),
            individualWriteInVerificationCode = writeInCandidate.choiceReturnCode,
            writeInCodeDesignation = new[]
                {
                    new electionInformationTypeWriteInCodesWriteInCodeDesignation()
                        { Language = LanguageType.de, codeDesignationText = "Anderer wählbarer Kandidat"},
                }
        };
    }
}
