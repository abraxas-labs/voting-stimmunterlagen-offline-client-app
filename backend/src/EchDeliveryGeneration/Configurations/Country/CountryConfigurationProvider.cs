// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EchDeliveryGeneration.Configurations.Country;

internal static class CountryConfigurationProvider
{
    private static Dictionary<string, CountryConfiguration> _countryConfigurationByIso;

    static CountryConfigurationProvider()
    {
        var mappingIsoCsvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Country_Mapping_ISO.csv");

        using var reader = new StreamReader(mappingIsoCsvPath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        });


        csv.Context.RegisterClassMap<CountryConfigurationMap>();

        var configs = csv.GetRecords<CountryConfiguration>().DistinctBy(c => c.Iso2).ToList();
        _countryConfigurationByIso = configs.ToDictionary(c => c.Iso2);
    }

    public static CountryConfiguration? GetCountryConfiguration(string countryIso2)
    {
        return _countryConfigurationByIso.GetValueOrDefault(countryIso2);
    }
}
