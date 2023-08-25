using System;
using System.Collections.Generic;
using System.Linq;
using eCH_0010_5_1;
using eCH_0228;
using Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;
using EchDeliveryGeneration.Ech0045;
using EchDeliveryGeneration.ErrorHandling;
using EchDeliveryGeneration.Models;

namespace EchDeliveryGeneration.Post;

public class VotingCardMapper
{
    private readonly VoterMapper _voterMapper;
    private readonly VoteMapper _voteMapper;
    private readonly ElectionGroupBallotMapper _electionGroupBallotMapper;

    public VotingCardMapper(VoterMapper voterMapper, VoteMapper voteMapper, ElectionGroupBallotMapper electionGroupBallotMapper)
    {
        _voterMapper = voterMapper;
        _voteMapper = voteMapper;
        _electionGroupBallotMapper = electionGroupBallotMapper;
    }

    public votingCardDataType MapToEchVotingCard(
        votingCardType votingCardType,
        configuration configuration,
        Configuration jsonConfig,
        Dictionary<string, Ech0045VoterExtension> echVoterByPersonId)
    {

        var votingCard = new votingCardDataType();

        var personConfig = configuration.register.FirstOrDefault(x =>
            x.voterIdentification == votingCardType.voterIdentification)
            ?? throw new TransformationException(TransformationErrorCode.VoterNotFound, votingCardType.voterIdentification);

        var echVoter = echVoterByPersonId.GetValueOrDefault(votingCardType.voterIdentification);

        var authorization = configuration.authorizations.SingleOrDefault(x =>
            x.authorizationIdentification == personConfig.authorization)
            ?? throw new TransformationException(TransformationErrorCode.AuthorizationNotFound, personConfig.authorization);

        votingCard.votingCardSequenceNumber = votingCardType.voterIdentification;
        votingCard.Item = _voterMapper.MapToEchVoter(personConfig, authorization, echVoter);

        AddVotesAndElection(votingCard, votingCardType, configuration);

        var votingPersonMuicipalityBfs = personConfig.person.municipality.municipalityId;
        var printingJsonConfig = jsonConfig.Printings.FirstOrDefault(x =>
            x.Municipalities.Any(y => y.Bfs == votingPersonMuicipalityBfs))
            ?? throw new TransformationException(TransformationErrorCode.PrintingNotFoundInZip, votingPersonMuicipalityBfs);

        var municipaityJsonConfig =
            printingJsonConfig.Municipalities.Single(x => x.Bfs == votingPersonMuicipalityBfs);

        municipaityJsonConfig.PollOpening = ConvertDateTimeToString(authorization.authorizationFromDate);
        municipaityJsonConfig.PollClosing = ConvertDateTimeToString(authorization.authorizationToDate);

        if (municipaityJsonConfig.ReturnDeliveryAddress != null)
        {
            var organisationMailAddressInfo = OrganisationMailAddressInfo.Create(municipaityJsonConfig.Name);
            var addressInformation = AddressInformation.Create(municipaityJsonConfig.Name, "CH");
            addressInformation.AddressLine1 = municipaityJsonConfig.ReturnDeliveryAddress.AddressField1;
            addressInformation.AddressLine2 = municipaityJsonConfig.ReturnDeliveryAddress.AddressField2;
            addressInformation.Street = municipaityJsonConfig.ReturnDeliveryAddress.Street;
            if (int.TryParse(municipaityJsonConfig.ReturnDeliveryAddress.Plz, out var plz) && plz > 999 &&
                plz < 10000)
                addressInformation.SwissZipCode = plz;

            votingCard.VotingCardReturnAddress = OrganisationMailAddress.Create(organisationMailAddressInfo, addressInformation);
        }

        votingCard.Extension = new VotingCardDataExtension(printingJsonConfig.Name, municipaityJsonConfig.Bfs);
        return votingCard;
    }


    private void AddVotesAndElection(
        votingCardDataType votingCardDataType,
        votingCardType votingCardSource,
        configuration configSource)
    {
        var individualCodes = new votingCardIndividualCodesType();
        var voteList = new List<eCH_0228.voteType>();
        var electionList = new List<votingCardIndividualCodesTypeElectionGroupBallot>();

        individualCodes.individualContestCodes = new namedCodeType[]
        {
            new namedCodeType() {codeDesignation = "startVotingKey", codeValue = votingCardSource.startVotingKey},
            new namedCodeType() {codeDesignation = "ballotCastingKey", codeValue = votingCardSource.ballotCastingKey},
            new namedCodeType() {codeDesignation = "voteCastCode", codeValue = votingCardSource.voteCastReturnCode},
            new namedCodeType() {codeDesignation = "votingCardId", codeValue = votingCardSource.votingCardId},
        };

        if (votingCardSource.vote != null)
        {
            foreach (var vote in votingCardSource.vote)
            {
                var config = configSource.contest.voteInformation.SingleOrDefault(x =>
                    x.vote.voteIdentification == vote.voteIdentification)
                    ?? throw new TransformationException(TransformationErrorCode.VoteNotFoud, vote.voteIdentification);

                voteList.Add(_voteMapper.MapToEchVote(vote, config));
            }
        }

        if (votingCardSource.election != null)
        {
            foreach (var election in votingCardSource.election)
            {
                var config = configSource.contest.electionGroupBallot.SelectMany(e => e.electionInformation).SingleOrDefault(x =>
                    x.election.electionIdentification == election.electionIdentification)
                    ?? throw new TransformationException(TransformationErrorCode.ElectionNotFound, election.electionIdentification);

                config.candidate ??= Array.Empty<candidateType>();
                election.writeInCandidate ??= Array.Empty<writeInCandidateType1>();
                election.candidate ??= Array.Empty<candidateType2>();
                electionList.Add(_electionGroupBallotMapper.MapToEchElectionGroupBallot(election, config));
            }
        }

        individualCodes.vote = voteList.ToArray();
        individualCodes.electionGroupBallot = electionList.ToArray();
        votingCardDataType.votingCardIndividualCodes = individualCodes;
    }

    private static string ConvertDateTimeToString(DateTime dateTime)
    {
        return dateTime.ToString("dd.MM.yyyy HH:mm:ss");
    }
}
