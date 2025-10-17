// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EchDeliveryGeneration.Ech0045;

public interface IEch0045Reader
{
    Task<Dictionary<string, Ech0045VoterExtension>> ReadVoterExtensions(Stream stream);
}
