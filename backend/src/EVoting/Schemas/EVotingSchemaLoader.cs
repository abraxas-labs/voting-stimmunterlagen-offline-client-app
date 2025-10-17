// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace EVoting.Schemas;

public static class EVotingSchemaLoader
{
    private static readonly Dictionary<string, string> Schemas = new()
    {
        ["http://www.w3.org/2000/09/xmldsig#"] = "xmldsig-core-schema.xsd",
        ["http://www.evoting.ch/xmlns/print/1"] = "evoting-print-1-3.xsd",
        ["http://www.evoting.ch/xmlns/print/2"] = "evoting-print-2-0.xsd",
        ["http://www.evoting.ch/xmlns/config/6"] = "evoting-config-6-0.xsd",
        ["http://www.evoting.ch/xmlns/config/7"] = "evoting-config-7-0.xsd",
    };

    public static XmlSchemaSet LoadEVotingSchemas()
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
