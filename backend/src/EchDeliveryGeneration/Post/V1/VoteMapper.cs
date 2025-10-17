// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using EchDeliveryGeneration.ErrorHandling;
using System.Linq;
using Ech0155_4_0;
using Ech0228_1_0;
using VoteType = Ech0228_1_0.VoteType;
using Voting.Lib.Common;


namespace EchDeliveryGeneration.Post.V1;

public class VoteMapper
{
    public VoteType MapToEchVote(EVoting.Print_1_3.VoteType voteTypeSource, EVoting.Config_6_0.VoteInformationType configInfo)
    {
        var voteDescriptions = configInfo.Vote.VoteDescription
            .Select(voteDescription => new VoteDescriptionInformationTypeVoteDescriptionInfo
            {
                Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(voteDescription.Language),
                VoteDescription = voteDescription.VoteDescription,
            })
            .ToList();

        var voteTypeBallotList = configInfo.Vote.Ballot
            .Select(x => MapToEchBallot(voteTypeSource.Question, x))
            .ToList();

        return new VoteType
        {
            VoteIdentification = voteTypeSource.VoteIdentification,
            VoteDescription = voteDescriptions,
            Ballot = voteTypeBallotList,
        };
    }

    private VoteTypeBallot MapToEchBallot(List<EVoting.Print_1_3.QuestionType> questions, EVoting.Config_6_0.BallotType ballotTypeConfig)
    {
        var voteTypeBallot = new VoteTypeBallot();
        var ballotDescriptionInfos = new List<BallotDescriptionInformationTypeBallotDescriptionInfo>();

        voteTypeBallot.BallotIdentification = ballotTypeConfig.BallotIdentification;
        voteTypeBallot.BallotPosition = ballotTypeConfig.BallotPosition.ToString();

        foreach (var ballotDescriptionInfo in ballotTypeConfig.BallotDescription)
        {
            ballotDescriptionInfos.Add(new BallotDescriptionInformationTypeBallotDescriptionInfo
            {
                Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(ballotDescriptionInfo.Language),
                BallotDescriptionLong = ballotDescriptionInfo.BallotDescriptionLong,
                BallotDescriptionShort = ballotDescriptionInfo.BallotDescriptionShort,
            });
        }

        voteTypeBallot.BallotDescription = ballotDescriptionInfos;

        if (ballotTypeConfig.StandardBallot != null)
        {
            var standardBallot = ballotTypeConfig.StandardBallot;
            var standardBallotItem = new VoteTypeBallotStandardBallot
            {
                QuestionInformation = new VoteTypeBallotStandardBallotQuestionInformation(),
            };
            var ballotQuestionInfoList = new List<BallotQuestionTypeBallotQuestionInfo>();

            standardBallotItem.QuestionInformation.QuestionIdentification = standardBallot.QuestionIdentification;

            foreach (var ballotQuestionTypeBallotQuestionInfo in standardBallot.BallotQuestion)
            {
                if (string.IsNullOrEmpty(ballotQuestionTypeBallotQuestionInfo.BallotQuestionTitle))
                {
                    ballotQuestionInfoList.Add(new BallotQuestionTypeBallotQuestionInfo
                    {
                        Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(ballotQuestionTypeBallotQuestionInfo.Language),
                        BallotQuestion = ballotQuestionTypeBallotQuestionInfo.BallotQuestion,
                    });
                }
                else
                {
                    ballotQuestionInfoList.Add(new BallotQuestionTypeBallotQuestionInfo
                    {
                        Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(ballotQuestionTypeBallotQuestionInfo.Language),
                        BallotQuestionTitle = ballotQuestionTypeBallotQuestionInfo.BallotQuestionTitle,
                        BallotQuestion = ballotQuestionTypeBallotQuestionInfo.BallotQuestion,
                    });
                }
            }

            standardBallotItem.QuestionInformation.BallotQuestion = ballotQuestionInfoList;

            var questionsConfig = questions.SingleOrDefault(x =>
                x.QuestionIdentification == standardBallot.QuestionIdentification)
                ?? throw new TransformationException(TransformationErrorCode.QuestionNotFound, standardBallot.QuestionIdentification);

            standardBallotItem.QuestionInformation.AnswerOption = MapToEchAnswers(questionsConfig, standardBallot.Answer);

            voteTypeBallot.StandardBallot = standardBallotItem;
        }
        else if (ballotTypeConfig.VariantBallot != null)
        {
            var variantBallot = ballotTypeConfig.VariantBallot;
            var variantBallotItem = new VoteTypeBallotVariantBallot();

            var standardQuestionList = new List<VoteTypeBallotVariantBallotQuestionInformation>();
            var tieBreakQuestionList = new List<VoteTypeBallotVariantBallotTieBreakInformation>();

            foreach (var standardQuestionType in variantBallot.StandardQuestion)
            {
                var ballotQuestionInfoList = new List<BallotQuestionTypeBallotQuestionInfo>();

                foreach (var ballotQuestionTypeBallotQuestionInfo in standardQuestionType.BallotQuestion)
                {
                    var ballotQuestion = new BallotQuestionTypeBallotQuestionInfo
                    {
                        Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(ballotQuestionTypeBallotQuestionInfo.Language),
                        BallotQuestion = ballotQuestionTypeBallotQuestionInfo.BallotQuestion
                    };

                    if (ballotQuestionTypeBallotQuestionInfo.BallotQuestionTitle != null)
                    {
                        ballotQuestion.BallotQuestionTitle = ballotQuestionTypeBallotQuestionInfo.BallotQuestionTitle;
                    }

                    ballotQuestionInfoList.Add(ballotQuestion);
                }

                var questionConfig = questions.SingleOrDefault(x =>
                    x.QuestionIdentification == standardQuestionType.QuestionIdentification)
                    ?? throw new TransformationException(TransformationErrorCode.QuestionNotFound, standardQuestionType.QuestionIdentification);

                standardQuestionList.Add(new VoteTypeBallotVariantBallotQuestionInformation()
                {
                    BallotQuestion = ballotQuestionInfoList,
                    BallotQuestionNumber = standardQuestionType.QuestionNumber,
                    QuestionIdentification = standardQuestionType.QuestionIdentification,
                    AnswerOption = MapToEchAnswers(questionConfig, standardQuestionType.Answer),
                    QuestionPosition = standardQuestionType.QuestionPosition.ToString()
                });
            }

            variantBallotItem.QuestionInformation = standardQuestionList;

            foreach (var tieBreakQuestionType in variantBallot.TieBreakQuestion)
            {
                var ballotQuestionInfoList = new List<TieBreakQuestionTypeTieBreakQuestionInfo>();

                foreach (var ballotQuestionTypeBallotQuestionInfo in tieBreakQuestionType.BallotQuestion)
                {
                    var tieBreakQuestion = new TieBreakQuestionTypeTieBreakQuestionInfo
                    {
                        Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(ballotQuestionTypeBallotQuestionInfo.Language),
                        TieBreakQuestion = ballotQuestionTypeBallotQuestionInfo.BallotQuestion
                    };
                    ballotQuestionInfoList.Add(tieBreakQuestion);
                }

                var questionConfig = questions.SingleOrDefault(x =>
                    x.QuestionIdentification == tieBreakQuestionType.QuestionIdentification)
                    ?? throw new TransformationException(TransformationErrorCode.QuestionNotFound, tieBreakQuestionType.QuestionIdentification);

                tieBreakQuestionList.Add(new VoteTypeBallotVariantBallotTieBreakInformation()
                {
                    TieBreakQuestion = ballotQuestionInfoList,
                    TieBreakQuestionNumber = tieBreakQuestionType.QuestionNumber,
                    QuestionIdentification = tieBreakQuestionType.QuestionIdentification,
                    AnswerOption = MapToEchAnswers(questionConfig, tieBreakQuestionType.Answer),
                    QuestionPosition = tieBreakQuestionType.QuestionPosition.ToString(),
                });
            }

            variantBallotItem.TieBreakInformation = tieBreakQuestionList;

            voteTypeBallot.VariantBallot = variantBallotItem;
        }

        return voteTypeBallot;
    }

    private List<AnswerOptionType> MapToEchAnswers(EVoting.Print_1_3.QuestionType question, List<EVoting.Config_6_0.StandardAnswerType> answers)
    {
        var answerList = new List<AnswerOptionType>();
        foreach (var answerType in answers)
        {
            var answerTextInfoList = new List<AnswerOptionTypeAnswerTextInformation>();

            foreach (var answerText in answerType.AnswerInfo)
            {
                answerTextInfoList.Add(new AnswerOptionTypeAnswerTextInformation()
                {
                    Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(answerText.Language),
                    AnswerText = answerText.Answer
                });
            }
            var answere = question.Answer
                .SingleOrDefault(x => x.AnswerIdentification == answerType.AnswerIdentification)
                ?? throw new TransformationException(TransformationErrorCode.AnswereNotFound, answerType.AnswerIdentification);

            answerList.Add(new AnswerOptionType()
            {
                AnswerIdentification = answerType.AnswerIdentification,
                AnswerSequenceNumber = answerType.AnswerPosition.ToString(),
                AnswerTextInformation = answerTextInfoList,
                IndividualVoteVerificationCode = answere.ChoiceReturnCode,
            });
        }
        return answerList;
    }

    private List<AnswerOptionType> MapToEchAnswers(EVoting.Print_1_3.QuestionType question, List<EVoting.Config_6_0.TiebreakAnswerType> answers)
    {
        var answerList = new List<AnswerOptionType>();
        foreach (var answerType in answers)
        {
            var answerTextInfoList = new List<AnswerOptionTypeAnswerTextInformation>();

            foreach (var answerText in answerType.AnswerInfo)
            {
                answerTextInfoList.Add(new AnswerOptionTypeAnswerTextInformation()
                {
                    Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(answerText.Language),
                    AnswerText = answerText.Answer
                });
            }
            var answer = question.Answer
                .SingleOrDefault(x => x.AnswerIdentification == answerType.AnswerIdentification)
                ?? throw new TransformationException(TransformationErrorCode.AnswereNotFound, answerType.AnswerIdentification);

            answerList.Add(new AnswerOptionType()
            {
                AnswerIdentification = answerType.AnswerIdentification,
                AnswerSequenceNumber = answerType.AnswerPosition.ToString(),
                AnswerTextInformation = answerTextInfoList,
                IndividualVoteVerificationCode = answer.ChoiceReturnCode,
            });
        }
        return answerList;
    }
}

