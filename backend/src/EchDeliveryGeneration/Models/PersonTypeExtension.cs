namespace EchDeliveryGeneration.Models;

public class PersonTypeExtension
{
    public PersonTypeExtension(PersonTypeAddressExtension? address, string printingRef, string municipalityRef, string municipalityName)
    {
        Address = address;
        PrintingRef = printingRef;
        MunicipalityRef = municipalityRef;
        MunicipalityName = municipalityName;
    }

    public PersonTypeAddressExtension? Address { get; }
    public string PrintingRef { get; }
    public string MunicipalityRef { get; }
    public string MunicipalityName { get; }
}
