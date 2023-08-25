using Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

namespace EchDeliveryGeneration.Models;

public class SmallPrinting
{
    public string name { get; }

    public SmallPrinting(Printing printing)
    {
        name = printing.Name;
    }
}
