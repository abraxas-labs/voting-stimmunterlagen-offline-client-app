﻿using CsvHelper.Configuration;

namespace chVoteToJsonConverter.Configurations.Country;

internal class CountryConfigurationMap : ClassMap<CountryConfiguration>
{
    public CountryConfigurationMap()
    {
        Map(m => m.Iso2).Name("ISO2");
        Map(m => m.Name).Name("Name");
        Map(m => m.StreetNrControl).Name("StreetNr")
            .TypeConverterOption.BooleanValues(true, true, "X")
            .TypeConverterOption.BooleanValues(false, true, "");
        Map(m => m.ZipCodeTownControl).Name("ZipCode")
            .TypeConverterOption.BooleanValues(true, true, "X")
            .TypeConverterOption.BooleanValues(false, true, "");

    }
}
