// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace EchDeliveryGeneration.Configurations.Country;

internal class CountryConfiguration
{
    public string Iso2 { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// If true then the address line with the street is displayed as "{Nr} {Street}".
    /// Otherwise it is displayed as "{Street} {Nr}"
    /// </summary>
    public bool StreetNrControl { get; set; }

    /// <summary>
    /// If true then the address line with the town is displayed as "{Town} {Zip}".
    /// Otherwise it is displayed as "{Zip} {Town}"
    /// </summary>
    public bool ZipCodeTownControl { get; set; }
}
