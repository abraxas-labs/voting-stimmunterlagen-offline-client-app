// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using Ech0155_4_0;
using Voting.Lib.Common;

namespace EchDeliveryGeneration.Post.V2;

public class ContestMapper
{
    public ContestType MapToEchContest(EVoting.Config_7_0.ContestType contest)
    {
        var contestDescriptionInfos = contest.ContestDescription
                .Select(x => new ContestDescriptionInformationTypeContestDescriptionInfo
                {
                    Language = XmlUtil.GetXmlEnumAttributeValueFromEnum(x.Language),
                    ContestDescription = x.ContestDescription,
                })
                .ToList();

        return new ContestType
        {
            ContestIdentification = contest.ContestIdentification,
            ContestDate = contest.ContestDate,
            EVotingPeriod = new EVotingPeriodType
            {
                EVotingPeriodFrom = contest.EvotingFromDate,
                EVotingPeriodTill = contest.EvotingToDate,
            },
            ContestDescription = contestDescriptionInfos,
        };
    }
}
