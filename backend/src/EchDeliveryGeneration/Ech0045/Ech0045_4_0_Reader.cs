// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using EchDeliveryGeneration.ErrorHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Ech0045_4_0;
using Voting.Lib.Ech.Ech0045_4_0.Converter;
using Voting.Lib.Ech.Ech0045_4_0.Models;

namespace EchDeliveryGeneration.Ech0045
{
    public class Ech0045_4_0_Reader : IEch0045Reader
    {
        private readonly Ech0045Deserializer _deserializer;

        public Ech0045_4_0_Reader(Ech0045Deserializer deserializer)
        {
            _deserializer = deserializer;
        }

        public async Task<Dictionary<string, Ech0045VoterExtension>> ReadVoterExtensions(Stream stream)
        {
            var voterDictionary = new Dictionary<string, Ech0045VoterExtension>();

            await foreach (var voter in _deserializer.ReadVoters(stream, default))
            {
                var mappedVoter = MapVoter(voter.Voter);

                if (voterDictionary.ContainsKey(mappedVoter.PersonId))
                {
                    throw new TransformationException(TransformationErrorCode.VoterDuplicates, mappedVoter.PersonId);
                }

                voterDictionary.Add(mappedVoter.PersonId, mappedVoter);
            }

            return voterDictionary;
        }

        private Ech0045VoterExtension MapVoter(VotingPersonType votingPersonType)
        {
            var voter = new Ech0045VoterExtension();

            if (votingPersonType.Person.Swiss != null)
            {
                voter.PersonId = votingPersonType.Person.Swiss.SwissDomesticPerson.PersonIdentification.LocalPersonId.PersonId;
                return voter;
            }

            if (votingPersonType.Person.SwissAbroad != null)
            {
                voter.PersonId = votingPersonType.Person.SwissAbroad.SwissAbroadPerson.PersonIdentification.LocalPersonId.PersonId;
                voter.SwissAbroadPersonExtensionAddress = GetExtension(votingPersonType.Person.SwissAbroad.SwissAbroadPerson.Extension)?.Address;
                return voter;
            }

            if (votingPersonType.Person.Foreigner != null)
            {
                voter.PersonId = votingPersonType.Person.Foreigner.ForeignerPerson.PersonIdentification.LocalPersonId.PersonId;
                return voter;
            }

            throw new InvalidOperationException("Invalid ech-0045 nationality choice type");
        }

        private SwissPersonExtension? GetExtension(object? extension)
        {
            if (extension == null)
            {
                return null;
            }

            var extensionChildNodes = extension as XmlNode[]
                ?? throw new InvalidOperationException("Swiss person extension not set as XML node");
            var extensionNode = extensionChildNodes.FirstOrDefault()?.ParentNode
                ?? throw new InvalidOperationException("Swiss person extension child node has no parent node");

            using var reader = new StringReader(extensionNode.OuterXml);
            var swissPersonExtension = DeserializeXmlNode<SwissPersonExtension>(reader);

            return new()
            {
                Address = swissPersonExtension?.Address == null ?
                    null :
                    new()
                    {
                        Line1 = swissPersonExtension.Address.Line1,
                        Line2 = swissPersonExtension.Address.Line2,
                        Line3 = swissPersonExtension.Address.Line3,
                        Line4 = swissPersonExtension.Address.Line4,
                        Line5 = swissPersonExtension.Address.Line5,
                        Line6 = swissPersonExtension.Address.Line6,
                        Line7 = swissPersonExtension.Address.Line7,
                    },
            };
        }

        private T? DeserializeXmlNode<T>(TextReader reader)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T?)serializer.Deserialize(reader);
        }
    }
}
