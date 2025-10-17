// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using Ech0010_6_0;
using Ech0228_1_0;
using SharedContestConfiguration = Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration.Configuration;
using EchDeliveryGeneration.Ech0045;
using EchDeliveryGeneration.ErrorHandling;
using VoteType = Ech0228_1_0.VoteType;

namespace EchDeliveryGeneration.Post.V1;

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

    public VotingCardDataType MapToEchVotingCard(
        EVoting.Print_1_3.VotingCardType votingCardType,
        EVoting.Config_6_0.Configuration configuration,
        SharedContestConfiguration jsonConfig,
        Dictionary<string, Ech0045VoterExtension>? echVoterByPersonId)
    {

        var votingCard = new VotingCardDataType();

        var personConfig = configuration.Register.FirstOrDefault(x =>
            x.VoterIdentification == votingCardType.VoterIdentification)
            ?? throw new TransformationException(TransformationErrorCode.VoterNotFound, votingCardType.VoterIdentification);

        var echVoter = echVoterByPersonId?.GetValueOrDefault(votingCardType.VoterIdentification);

        var authorization = configuration.Authorizations.SingleOrDefault(x =>
            x.AuthorizationIdentification == personConfig.Authorization)
            ?? throw new TransformationException(TransformationErrorCode.AuthorizationNotFound, personConfig.Authorization);

        var votingPersonMuicipalityBfs = personConfig.Person.Municipality.MunicipalityId.ToString();
        var printingJsonConfig = jsonConfig.Printings.FirstOrDefault(x =>
                                     x.Municipalities.Any(y => y.Bfs == votingPersonMuicipalityBfs))
                                 ?? throw new TransformationException(TransformationErrorCode.PrintingNotFoundInZip, votingPersonMuicipalityBfs);

        var municipaityJsonConfig =
            printingJsonConfig.Municipalities.Single(x => x.Bfs == votingPersonMuicipalityBfs);

        municipaityJsonConfig.PollOpening = ConvertDateTimeToString(authorization.AuthorizationFromDate);
        municipaityJsonConfig.PollClosing = ConvertDateTimeToString(authorization.AuthorizationToDate);

        votingCard.VotingCardSequenceNumber = votingCardType.VoterIdentification;
        votingCard.VotingPerson = _voterMapper.MapToEchVoter(personConfig, authorization, echVoter, printingJsonConfig.Name, municipaityJsonConfig.Bfs, municipaityJsonConfig.Name);

        AddVotesAndElection(votingCard, votingCardType, configuration);

        if (municipaityJsonConfig.ReturnDeliveryAddress != null)
        {
            var organisationMailAddressInfo = new OrganisationMailAddressInfoType
            {
                OrganisationName = municipaityJsonConfig.Name
            };
            var addressInformation = new AddressInformationType
            {
                Town = municipaityJsonConfig.Name,
                Country = new CountryType { CountryNameShort = "CH" },
                AddressLine1 = municipaityJsonConfig.ReturnDeliveryAddress.AddressField1,
                AddressLine2 = municipaityJsonConfig.ReturnDeliveryAddress.AddressField2,
                Street = municipaityJsonConfig.ReturnDeliveryAddress.Street
            };
            if (uint.TryParse(municipaityJsonConfig.ReturnDeliveryAddress.Plz, out var plz) && plz > 999 &&
                plz < 10000)
                addressInformation.SwissZipCode = plz;

            votingCard.VotingCardReturnAddress = new List<OrganisationMailAddressType>
            {
                new()
                {
                    AddressInformation = addressInformation,
                    Organisation = organisationMailAddressInfo,
                }
            };
        }

        return votingCard;
    }


    private void AddVotesAndElection(
        VotingCardDataType votingCardDataType,
        EVoting.Print_1_3.VotingCardType votingCardSource,
        EVoting.Config_6_0.Configuration configSource)
    {
        var individualCodes = new VotingCardIndividualCodesType();
        var voteList = new List<VoteType>();
        var electionList = new List<VotingCardIndividualCodesTypeElectionGroupBallot>();

        individualCodes.IndividualContestCodes = new List<NamedCodeType>
        {
            new() { CodeDesignation = "startVotingKey", CodeValue = votingCardSource.StartVotingKey },
            new() { CodeDesignation = "ballotCastingKey", CodeValue = votingCardSource.BallotCastingKey },
            new() { CodeDesignation = "voteCastCode", CodeValue = votingCardSource.VoteCastReturnCode },
            new() { CodeDesignation = "votingCardId", CodeValue = votingCardSource.VotingCardId },
        };

        if (votingCardSource.Vote != null)
        {
            foreach (var vote in votingCardSource.Vote)
            {
                var config = configSource.Contest.VoteInformation.SingleOrDefault(x =>
                    x.Vote.VoteIdentification == vote.VoteIdentification)
                    ?? throw new TransformationException(TransformationErrorCode.VoteNotFoud, vote.VoteIdentification);

                voteList.Add(_voteMapper.MapToEchVote(vote, config));
            }
        }

        if (votingCardSource.Election != null)
        {
            foreach (var election in votingCardSource.Election)
            {
                var config = configSource.Contest.ElectionGroupBallot.SelectMany(e => e.ElectionInformation).SingleOrDefault(x =>
                    x.Election.ElectionIdentification == election.ElectionIdentification)
                    ?? throw new TransformationException(TransformationErrorCode.ElectionNotFound, election.ElectionIdentification);

                config.Candidate ??= new List<EVoting.Config_6_0.CandidateType>();
                election.WriteInCandidate ??= new List<EVoting.Print_1_3.WriteInCandidateType>();
                election.Candidate ??= new List<EVoting.Print_1_3.CandidateType>();
                electionList.Add(_electionGroupBallotMapper.MapToEchElectionGroupBallot(election, config));
            }
        }

        individualCodes.Vote = voteList;
        individualCodes.ElectionGroupBallot = electionList;
        votingCardDataType.VotingCardIndividualCodes = individualCodes;
    }

    private static string ConvertDateTimeToString(DateTime dateTime)
    {
        return dateTime.ToString("dd.MM.yyyy HH:mm:ss");
    }
}
