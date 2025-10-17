// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Ech0228_1_0;

namespace EchDeliveryGeneration.Post.V1;

public class ElectionGroupBallotMapper
{
    private readonly ElectionMapper _electionMapper;

    public ElectionGroupBallotMapper(ElectionMapper electionMapper)
    {
        _electionMapper = electionMapper;
    }

    public VotingCardIndividualCodesTypeElectionGroupBallot MapToEchElectionGroupBallot(EVoting.Print_1_3.ElectionType electionTypeSource, EVoting.Config_6_0.ElectionInformationType config)
    {
        return new VotingCardIndividualCodesTypeElectionGroupBallot
        {
            ElectionInformation = new List<ElectionInformationType> { _electionMapper.MapToEchElection(electionTypeSource, config) },
        };
    }
}
