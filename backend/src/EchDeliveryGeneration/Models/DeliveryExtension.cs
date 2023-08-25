using System.Collections.Generic;
using Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

namespace EchDeliveryGeneration.Models;

public class DeliveryExtension
{
    public Dictionary<string, Municipality> Municipalities { get; } = new();
    public Dictionary<string, SmallPrinting> Printings { get; } = new();
    public List<string> Certificates { get; init; } = new();
}

