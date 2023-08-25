using eCH_0228;

namespace EchDeliveryGeneration.Post;

public class ElectionGroupBallotMapper
{
    private readonly ElectionMapper _electionMapper;

    public ElectionGroupBallotMapper(ElectionMapper electionMapper)
    {
        _electionMapper = electionMapper;
    }

    public votingCardIndividualCodesTypeElectionGroupBallot MapToEchElectionGroupBallot(electionType1 electionTypeSource, electionInformationType config)
    {
        return new votingCardIndividualCodesTypeElectionGroupBallot
        {
            electionInformation = new[] { _electionMapper.MapToEchElection(electionTypeSource, config) },
        };
    }
}
