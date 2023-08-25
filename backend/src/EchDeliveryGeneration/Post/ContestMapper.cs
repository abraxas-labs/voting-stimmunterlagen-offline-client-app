using eCH_0155_4_0;
using System.Collections.Generic;

namespace EchDeliveryGeneration.Post;

public class ContestMapper
{
    public ContestType MapToEchContest(contestType contest)
    {
        var contestType = ContestType.Create(contest.contestIdentification, contest.contestDate);
        var contestDescriptionInfo = new List<ContestDescriptionInfo>();

        foreach (var description in contest.contestDescription)
        {
            contestDescriptionInfo.Add(ContestDescriptionInfo.Create(description.language.ToString(), description.contestDescription));
        }

        contestType.ContestDescription = ContestDescriptionInformation.Create(contestDescriptionInfo);
        contestType.EvotingPeriod = EvotingPeriodType.Create(contest.evotingFromDate, contest.evotingToDate);
        return contestType;
    }
}
