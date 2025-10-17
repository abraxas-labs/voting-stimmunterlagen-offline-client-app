// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;

namespace EchDeliveryGeneration;

public static class XmlSchemas
{
    public const string PostConfigV6Schema = "http://www.evoting.ch/xmlns/config/6";
    public const string PostConfigV7Schema = "http://www.evoting.ch/xmlns/config/7";

    public const string PostPrintV1Schema = "http://www.evoting.ch/xmlns/print/1";
    public const string PostPrintV2Schema = "http://www.evoting.ch/xmlns/print/2";

    public const string Ech0045V4Schema = "http://www.ech.ch/xmlns/eCH-0045/4";
    public const string Ech0045V6Schema = "http://www.ech.ch/xmlns/eCH-0045/6";

    public static readonly string[] PostConfigSchemas = new[] { PostConfigV6Schema, PostConfigV7Schema };
    public static readonly string[] PostPrintSchemas = new[] { PostPrintV1Schema, PostPrintV2Schema };
    public static readonly string[] Ech0045Schemas = new[] { Ech0045V4Schema, Ech0045V6Schema };

    public static string[] All => PostConfigSchemas
        .Concat(PostPrintSchemas)
        .Concat(Ech0045Schemas)
        .ToArray();
}
