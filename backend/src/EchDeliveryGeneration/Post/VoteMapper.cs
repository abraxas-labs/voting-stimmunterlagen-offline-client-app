using eCH_0045_4_0;
using eCH_0155_4_0;
using eCH_0228;
using EchDeliveryGeneration.ErrorHandling;
using System.Collections.Generic;
using System.Linq;


namespace EchDeliveryGeneration.Post;

public class VoteMapper
{
    public eCH_0228.voteType MapToEchVote(voteType1 voteTypeSource, voteInformationType configInfo)
    {
        var vote = new eCH_0228.voteType
        {
            VoteIdentification = voteTypeSource.voteIdentification
        };

        var voteDescriptions = new List<VoteDescriptionInfoType>();
        foreach (var voteDescription in configInfo.vote.voteDescription)
        {
            voteDescriptions.Add(VoteDescriptionInfoType.Create(voteDescription.language.ToString(), voteDescription.voteDescription));
        }

        vote.VoteDescription = VoteDescriptionInformationType.Create(voteDescriptions);

        var voteTypeBallotList = new List<voteTypeBallot>();

        foreach (var ballotType in configInfo.vote.ballot)
        {
            voteTypeBallotList.Add(MapToEchBallot(voteTypeSource.question, ballotType));
        }

        vote.ballot = voteTypeBallotList.ToArray();

        return vote;
    }

    private voteTypeBallot MapToEchBallot(questionType[] questions, ballotType ballotTypeConfig)
    {
        var voteTypeBallot = new voteTypeBallot();
        var ballotDescriptionInfos = new List<BallotDescriptionInfo>();

        voteTypeBallot.BallotIdentification = ballotTypeConfig.ballotIdentification;
        voteTypeBallot.BallotPosition = int.Parse(ballotTypeConfig.ballotPosition);

        foreach (var ballotDescriptionInfo in ballotTypeConfig.ballotDescription)
        {
            ballotDescriptionInfos.Add(BallotDescriptionInfo.Create(ballotDescriptionInfo.language.ToString(), ballotDescriptionInfo.ballotDescriptionLong, ballotDescriptionInfo.ballotDescriptionShort));
        }

        voteTypeBallot.BallotDescription = BallotDescriptionInformation.Create(ballotDescriptionInfos);

        if (ballotTypeConfig.Item is standardBallotType standardBallot)
        {
            var standardBallotItem = new voteTypeBallotStandardBallot();
            var ballotQuestionInfoList = new List<BallotQuestionInfo>();

            standardBallotItem.QuestionIdentification = standardBallot.questionIdentification;

            foreach (var ballotQuestionTypeBallotQuestionInfo in standardBallot.ballotQuestion)
            {
                if (string.IsNullOrEmpty(ballotQuestionTypeBallotQuestionInfo.ballotQuestionTitle))
                {
                    ballotQuestionInfoList.Add(BallotQuestionInfo.Create(
                        ballotQuestionTypeBallotQuestionInfo.language.ToString(),
                        ballotQuestionTypeBallotQuestionInfo.ballotQuestion));
                }
                else
                {
                    ballotQuestionInfoList.Add(BallotQuestionInfo.Create(
                        ballotQuestionTypeBallotQuestionInfo.language.ToString(),
                        ballotQuestionTypeBallotQuestionInfo.ballotQuestionTitle,
                        ballotQuestionTypeBallotQuestionInfo.ballotQuestion));
                }
            }

            standardBallotItem.BallotQuestion = BallotQuestion.Create(ballotQuestionInfoList);

            var questionsConfig = questions.SingleOrDefault(x =>
                x.questionIdentification == standardBallot.questionIdentification)
                ?? throw new TransformationException(TransformationErrorCode.QuestionNotFound, standardBallot.questionIdentification);

            standardBallotItem.answerOption = MapToEchAnswers(questionsConfig, standardBallot.answer).ToArray();

            voteTypeBallot.Item = standardBallotItem;
        }
        else if (ballotTypeConfig.Item is variantBallotType variantBallot)
        {
            var variantBallotItem = new voteTypeBallotVariantBallot();

            var standardQuestionList = new List<voteTypeBallotVariantBallotQuestionInformation>();
            var tieBreakQuestionList = new List<voteTypeBallotVariantBallotTieBreakInformation>();

            foreach (var standardQuestionType in variantBallot.standardQuestion)
            {
                var ballotQuestionInfoList = new List<BallotQuestionInfo>();

                foreach (var ballotQuestionTypeBallotQuestionInfo in standardQuestionType.ballotQuestion)
                {
                    var ballotQuestion = BallotQuestionInfo.Create(
                        ballotQuestionTypeBallotQuestionInfo.language.ToString(),
                        ballotQuestionTypeBallotQuestionInfo.ballotQuestion);
                    if (ballotQuestionTypeBallotQuestionInfo.ballotQuestionTitle != null)
                    {
                        ballotQuestion.BallotQuestionTitle = ballotQuestionTypeBallotQuestionInfo.ballotQuestionTitle;
                    }
                    ballotQuestionInfoList.Add(ballotQuestion);
                }

                int.TryParse(standardQuestionType.questionNumber, out var questionNumber);
                int.TryParse(standardQuestionType.questionPosition, out var questionPosition);

                var questionConfig = questions.SingleOrDefault(x =>
                    x.questionIdentification == standardQuestionType.questionIdentification)
                    ?? throw new TransformationException(TransformationErrorCode.QuestionNotFound, standardQuestionType.questionIdentification);

                standardQuestionList.Add(new voteTypeBallotVariantBallotQuestionInformation()
                {
                    BallotQuestion = BallotQuestion.Create(ballotQuestionInfoList),
                    BallotQuestionNumber = questionNumber,
                    QuestionIdentification = standardQuestionType.questionIdentification,
                    questionInformation = MapToEchAnswers(questionConfig, standardQuestionType.answer).ToArray(),
                    QuestionPosition = questionPosition
                });
            }

            variantBallotItem.questionInformation = standardQuestionList.ToArray();

            foreach (var tieBreakQuestionType in variantBallot.tieBreakQuestion)
            {
                var ballotQuestionInfoList = new List<TieBreakQuestionInfo>();

                foreach (var ballotQuestionTypeBallotQuestionInfo in tieBreakQuestionType.ballotQuestion)
                {
                    var tieBreakQuestion = TieBreakQuestionInfo.Create(
                        ballotQuestionTypeBallotQuestionInfo.language.ToString(),
                        ballotQuestionTypeBallotQuestionInfo.ballotQuestion);
                    ballotQuestionInfoList.Add(tieBreakQuestion);
                }

                int.TryParse(tieBreakQuestionType.questionNumber, out var questionNumber);
                int.TryParse(tieBreakQuestionType.questionPosition, out var questionPosition);

                var questionConfig = questions.SingleOrDefault(x =>
                    x.questionIdentification == tieBreakQuestionType.questionIdentification)
                    ?? throw new TransformationException(TransformationErrorCode.QuestionNotFound, tieBreakQuestionType.questionIdentification);

                tieBreakQuestionList.Add(new voteTypeBallotVariantBallotTieBreakInformation()
                {
                    BallotQuestion = TieBreakQuestion.Create(ballotQuestionInfoList),
                    TieBreakQuestionNumber = questionNumber,
                    QuestionIdentification = tieBreakQuestionType.questionIdentification,
                    questionInformation = MapToEchAnswers(questionConfig, tieBreakQuestionType.answer).ToArray(),
                    QuestionPosition = questionPosition
                });
            }

            variantBallotItem.tieBreakInformation = tieBreakQuestionList.ToArray();

            voteTypeBallot.Item = variantBallotItem;
        }

        return voteTypeBallot;
    }

    private List<answerOptionType> MapToEchAnswers(questionType question, standardAnswerType[] answers)
    {
        var answerList = new List<answerOptionType>();
        foreach (var answerType in answers)
        {
            var answerTextInfoList = new List<answerOptionTypeAnswerTextInformation>();

            foreach (var answerText in answerType.answerInfo)
            {
                answerTextInfoList.Add(new answerOptionTypeAnswerTextInformation()
                {
                    Language = (LanguageType)answerText.language,
                    answerText = answerText.answer
                });
            }
            var answere = question.answer
                .SingleOrDefault(x => x.answerIdentification == answerType.answerIdentification)
                ?? throw new TransformationException(TransformationErrorCode.AnswereNotFound, answerType.answerIdentification);

            answerList.Add(new answerOptionType()
            {
                AnswerIdentification = answerType.answerIdentification,
                answerSequenceNumber = answerType.answerPosition,
                answerTextInformation = answerTextInfoList.ToArray(),
                individualVoteVerificationCode = answere.choiceReturnCode,
            });
        }
        return answerList;
    }

    private List<answerOptionType> MapToEchAnswers(questionType question, tiebreakAnswerType[] answers)
    {
        var answerList = new List<answerOptionType>();
        foreach (var answerType in answers)
        {
            var answerTextInfoList = new List<answerOptionTypeAnswerTextInformation>();

            foreach (var answerText in answerType.answerInfo)
            {
                answerTextInfoList.Add(new answerOptionTypeAnswerTextInformation()
                {
                    Language = (LanguageType)answerText.language,
                    answerText = answerText.answer
                });
            }
            var answere = question.answer
                .SingleOrDefault(x => x.answerIdentification == answerType.answerIdentification)
                ?? throw new TransformationException(TransformationErrorCode.AnswereNotFound, answerType.answerIdentification);

            answerList.Add(new answerOptionType()
            {
                AnswerIdentification = answerType.answerIdentification,
                answerSequenceNumber = answerType.answerPosition,
                answerTextInformation = answerTextInfoList.ToArray(),
                individualVoteVerificationCode = answere.choiceReturnCode,
            });
        }
        return answerList;
    }
}

