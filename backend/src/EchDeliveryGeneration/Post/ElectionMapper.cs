using System.Linq;
using Ech0155_4_0;
using Ech0228_1_0;
using Voting.Lib.Common;

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

    public ElectionInformationType MapToEchElection(EVoting.Print.ElectionType electionTypeSource, EVoting.Config.ElectionInformationType config)
    {
        var electionDescriptionList = config.Election.ElectionDescription
            .Select(electionDescriptionInfo => new ElectionDescriptionInformationTypeElectionDescriptionInfo
            {
                Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(electionDescriptionInfo.Language),
                ElectionDescription = electionDescriptionInfo.ElectionDescription,
                ElectionDescriptionShort = electionDescriptionInfo.ElectionDescriptionShort,
            })
            .ToList();

        var election = new ElectionInformationType
        {
            Election = new ElectionInformationTypeElection
            {
                ElectionIdentification = electionTypeSource.ElectionIdentification,
                NumberOfMandates = config.Election.NumberOfMandates.ToString(),
                ElectionDescription = electionDescriptionList,
            }
        };

        if (config.Election.TypeOfElection == EVoting.Config.ElectionTypeTypeOfElection.Item1)
        {
            _proportionalElectionMapper.MapToEchElection(election, electionTypeSource, config);
        }
        else if (config.Election.TypeOfElection == EVoting.Config.ElectionTypeTypeOfElection.Item2)
        {
            _majorityElectionMapper.MapToEchElection(election, electionTypeSource, config);
        }

        return election;
    }
}
