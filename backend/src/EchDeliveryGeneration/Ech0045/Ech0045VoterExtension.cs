using Voting.Lib.Ech.Ech0045_4_0.Models;

namespace EchDeliveryGeneration.Ech0045
{
    /// <summary>
    /// A simplified Ech-0045 voter model to include informations which the Post config does not include such as the swiss abroad extension with the address.
    /// The main informations of the voter should be read from the Post config file.
    /// </summary>
    public class Ech0045VoterExtension
    {
        public string PersonId { get; set; } = string.Empty;

        public SwissAbroadPersonExtensionAddress? SwissAbroadPersonExtensionAddress { get; set; }
    }
}
