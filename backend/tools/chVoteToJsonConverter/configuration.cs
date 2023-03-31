using System.Collections.Generic;
using Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

namespace chVoteToJsonConverter;

public class eCH228JsonConfig
{
    public eCH228JsonConfig(string printingRef, string municipalityRef)
    {
        PrintingRef = printingRef;
        MunicipalityRef = municipalityRef;
    }

    public string PrintingRef { get; }
    public string MunicipalityRef { get; }
}

public class SmallPrinting
{
    public string name { get; }

    public SmallPrinting(Printing printing)
    {
        name = printing.Name;
    }
}

public class DeliveryExtension
{
    public Dictionary<string, Municipality> Municipalities { get; } = new();
    public Dictionary<string, SmallPrinting> Printings { get; } = new();
    public List<string> Certificates { get; init; } = new();
}
