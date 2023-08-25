using eCH_0155_4_0;
using System.Collections.Generic;

namespace EchDeliveryGeneration.Post;

public class ElectionMapper
{
    private readonly ProportionalElectionMapper _proportionalElectionMapper;
    private readonly MajorityElectionMapper _majorityElectionMapper;

    public ElectionMapper(ProportionalElectionMapper proportionalElectionMapper, MajorityElectionMapper majorityElectionMapper)
    {
        _proportionalElectionMapper = proportionalElectionMapper;
        _majorityElectionMapper = majorityElectionMapper;
    }

    public eCH_0228.electionInformationType MapToEchElection(electionType1 electionTypeSource, electionInformationType config)
    {
        var election = new eCH_0228.electionInformationType();

        int.TryParse(config.election.numberOfMandates, out var numberOfMandates);
        election.election = ElectionType.Create(electionTypeSource.electionIdentification, (TypeOfElectionType)config.election.typeOfElection, numberOfMandates);

        var electionDescriptionList = new List<ElectionDescriptionInfoType>();
        foreach (var electionDescriptionInfo in config.election.electionDescription)
        {
            electionDescriptionList.Add(ElectionDescriptionInfoType.Create(electionDescriptionInfo.language.ToString(), electionDescriptionInfo.electionDescription, electionDescriptionInfo.electionDescriptionShort));
        }

        election.election.ElectionDescription = ElectionDescriptionInformationType.Create(electionDescriptionList);

        if (config.election.typeOfElection == electionTypeTypeOfElection.Item1)
        {
            _proportionalElectionMapper.MapToEchElection(election, electionTypeSource, config);
        }
        else if (config.election.typeOfElection == electionTypeTypeOfElection.Item2)
        {
            _majorityElectionMapper.MapToEchElection(election, electionTypeSource, config);
        }

        return election;
    }
}
