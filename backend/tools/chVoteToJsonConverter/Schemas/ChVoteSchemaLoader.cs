using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace chVoteToJsonConverter.Schemas;

internal static class ChVoteSchemaLoader
{
    private static readonly Dictionary<string, string> Schemas = new()
    {
        ["http://www.evoting.ch/xmlns/print/1"] = "evoting-print-1-2.xsd",
        ["http://www.evoting.ch/xmlns/config/4"] = "evoting-config-4-4.xsd"
    };

    public static XmlSchemaSet LoadChVoteSchemas()
    {
        var schemaDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Schemas");
        var xmlSchemaSet = new XmlSchemaSet();
        foreach (var (schemaName, schemaFileName) in Schemas)
        {
            var schemaPath = Path.Combine(schemaDirectory, schemaFileName);
            using var xmlReader = XmlReader.Create(schemaPath);
            xmlSchemaSet.Add(schemaName, xmlReader);
        }

        return xmlSchemaSet;
    }
}
