// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Ech0155_4_0;
using Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

namespace EchDeliveryGeneration.Models;

public class DeliveryExtension : ExtensionType
{
    public Dictionary<string, Municipality> Municipalities { get; } = new();
    public List<string> Certificates { get; init; } = new();
}

