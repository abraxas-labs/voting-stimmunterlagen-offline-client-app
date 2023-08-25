using eCH_0045_4_0;
using EchDeliveryGeneration.ErrorHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Voting.Lib.Ech.Ech0045.Converter;
using Voting.Lib.Ech.Ech0045.Models;

namespace EchDeliveryGeneration.Ech0045
{
    public class Ech0045Reader
    {
        private readonly Ech0045Deserializer _deserializer;

        public Ech0045Reader(Ech0045Deserializer deserializer)
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

            switch (votingPersonType.Person.NationalityChoice)
            {
                case SwissDomesticType swiss:
                    voter.PersonId = swiss.SwissDomesticPerson.PersonIdentification.LocalPersonId.PersonId;
                    return voter;
                case SwissAbroadType swissAbroad:
                    voter.PersonId = swissAbroad.SwissAbroadPerson.PersonIdentification.LocalPersonId.PersonId;
                    voter.SwissAbroadPersonExtensionAddress = GetExtension(swissAbroad.SwissAbroadPerson.Extension)?.Address;
                    return voter;
                case ForeignerType foreign:
                    voter.PersonId = foreign.ForeignerPerson.PersonIdentification.LocalPersonId.PersonId;
                    return voter;
                default:
                    throw new InvalidOperationException("Invalid ech-0045 nationality choice type");
            }
        }

        private SwissPersonExtension GetExtension(object extension)
        {
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
