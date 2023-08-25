namespace EchDeliveryGeneration.Models;

public class VotingCardDataExtension
{
    public VotingCardDataExtension(string printingRef, string municipalityRef)
    {
        PrintingRef = printingRef;
        MunicipalityRef = municipalityRef;
    }

    public string PrintingRef { get; }
    public string MunicipalityRef { get; }
}
