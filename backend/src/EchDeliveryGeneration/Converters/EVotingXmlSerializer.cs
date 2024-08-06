// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;

namespace EchDeliveryJsonConverter.Converters;

internal static class EVotingXmlSerializer
{
    internal static T DeserializeXml<T>(XmlReader xmlReader)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T?)serializer.Deserialize(xmlReader)
                   ?? throw new ValidationException("Deserialization returned null");
        }
        catch (InvalidOperationException ex) when (ex.InnerException != null)
        {
            // The XmlSerializer wraps all exceptions into an InvalidOperationException.
            // Unwrap it to surface the "correct" exception type.
            throw ex.InnerException;
        }
    }
}
