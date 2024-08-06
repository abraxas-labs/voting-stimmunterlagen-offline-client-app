// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Ech0007_6_0;
using Ech0010_6_0;
using Ech0044_4_1;
using Ech0155_4_0;
using EchDeliveryGeneration.Configurations.Country;
using EchDeliveryGeneration.Ech0045;
using EchDeliveryGeneration.ErrorHandling;
using EchDeliveryGeneration.Models;
using Microsoft.Extensions.Logging;
using ForeignerPersonType = Ech0228_1_0.ForeignerPersonType;
using ForeignerType = Ech0228_1_0.ForeignerType;
using PersonIdentificationType = Ech0228_1_0.PersonIdentificationType;
using SwissAbroadType = Ech0228_1_0.SwissAbroadType;
using SwissDomesticType = Ech0228_1_0.SwissDomesticType;
using SwissPersonType = Ech0228_1_0.SwissPersonType;
using VotingPersonType = Ech0228_1_0.VotingPersonType;
using VotingPersonTypeDomainOfInfluenceInfo = Ech0228_1_0.VotingPersonTypeDomainOfInfluenceInfo;
using VotingPersonTypePerson = Ech0228_1_0.VotingPersonTypePerson;
using SexType = Ech0044_4_1.SexType;
using LanguageType = Ech0045_4_0.LanguageType;
using MrMrsType = Ech0010_6_0.MrMrsType;
using CountingCircleType = Ech0155_4_0.CountingCircleType;
using DomainOfInfluenceType = Ech0155_4_0.DomainOfInfluenceType;

namespace EchDeliveryGeneration.Post;

public class VoterMapper
{
    private readonly ILogger<VoterMapper> _logger;

    public VoterMapper(ILogger<VoterMapper> logger)
    {
        _logger = logger;
    }

    public VotingPersonType MapToEchVoter(EVoting.Config.VoterType voterType, EVoting.Config.AuthorizationType authorization, Ech0045VoterExtension? echVoter, string printingRef, string municipalityRef, string municipalityName)
    {
        var personInfo = voterType.Person;
        var nationality = default(VotingPersonTypePerson);
        var country10 = new CountryType
        {
            CountryIdIso2 = voterType.Person.ResidenceCountryId,
            CountryNameShort = voterType.Person.ResidenceCountryId,
        };
        var country08 = new Ech0008_3_0.CountryType()
        {
            CountryIdIso2 = voterType.Person.ResidenceCountryId,
            CountryNameShort = voterType.Person.ResidenceCountryId,
        };
        var sexType = personInfo.Sex == EVoting.Config.PersonTypeSex.Item1 ? SexType.Item1 : SexType.Item2;
        var dateOfBirth = new DatePartiallyKnownType { YearMonthDay = personInfo.DateOfBirth };

        var physicalAddressCountryConfig = CountryConfigurationProvider.GetCountryConfiguration(voterType.Person.PhysicalAddress.Country)
            ?? throw new TransformationException(TransformationErrorCode.CountryNotFound, voterType.Person.PhysicalAddress.Country);

        var namedPersonId = new NamedPersonIdType
        {
            PersonIdCategory = "voting",
            PersonId = voterType.VoterIdentification,
        };
        var personIdentification = new PersonIdentificationType
        {
            LocalPersonId = namedPersonId,
            OfficialName = personInfo.OfficialName,
            FirstName = personInfo.FirstName,
            Sex = sexType,
            DateOfBirth = dateOfBirth,

        };

        var swissPersonType = new SwissPersonType
        {
            PersonIdentification = personIdentification,
            LanguageOfCorrespondance = (LanguageType)personInfo.LanguageOfCorrespondance,
        };

        var addressExtension = GetAddressExtension(voterType, echVoter, physicalAddressCountryConfig);
        swissPersonType.Extension = new PersonTypeExtension(addressExtension, printingRef, municipalityRef, municipalityName);

        var swissMunicipality = new SwissMunicipalityType
        {
            MunicipalityName = personInfo.Municipality.MunicipalityName,
            MunicipalityId = personInfo.Municipality.MunicipalityId
        };


        var mrMrs = (MrMrsType?)personInfo.PhysicalAddress.MrMrs;

        var personMailAddressInfo = new PersonMailAddressInfoType
        {
            MrMrs = mrMrs,
            Title = personInfo.PhysicalAddress.Title,
            FirstName = personInfo.PhysicalAddress.FirstName,
            LastName = personInfo.PhysicalAddress.LastName

        };

        var addressInformationType = new AddressInformationType
        {
            Town = personInfo.PhysicalAddress.Town,
            Country = country10,
            DwellingNumber = voterType.Person.PhysicalAddress.DwellingNumber,
            HouseNumber = voterType.Person.PhysicalAddress.HouseNumber,
            Street = voterType.Person.PhysicalAddress.Street
        };

        if (uint.TryParse(voterType.Person.PhysicalAddress.ZipCode, out var zipCode) && zipCode > 999 && zipCode < 10000)
            addressInformationType.SwissZipCode = zipCode;

        switch (voterType.VoterTypeProperty)
        {
            case EVoting.Config.VoterTypeType.Swissresident:
                var swissDomesticType = new SwissDomesticType
                {
                    SwissDomesticPerson = swissPersonType,
                    Municipality = swissMunicipality,
                };

                nationality = new VotingPersonTypePerson { Swiss = swissDomesticType };
                break;
            case EVoting.Config.VoterTypeType.Swissabroad:
                var swissAbroadType = new SwissAbroadType
                {
                    SwissAbroadPerson = swissPersonType,
                    ResidenceCountry = country08,
                    Municipality = swissMunicipality,
                };

                addressInformationType.ForeignZipCode = voterType.Person.PhysicalAddress.ZipCode;

                nationality = new VotingPersonTypePerson { SwissAbroad = swissAbroadType };
                break;
            case EVoting.Config.VoterTypeType.Foreigner:
                var foreignerPerson = new ForeignerPersonType
                {
                    PersonIdentification = personIdentification,
                    LanguageOfCorrespondance = (LanguageType)personInfo.LanguageOfCorrespondance,
                    Extension = new PersonTypeExtension(null, printingRef, municipalityRef, municipalityName)
                };

                nationality = new VotingPersonTypePerson
                {
                    Foreigner = new ForeignerType
                    {
                        ForeignerPerson = foreignerPerson,
                        Municipality = swissMunicipality,
                    },
                };
                break;
        }

        var personMailAddress = new PersonMailAddressType
        {
            AddressInformation = addressInformationType,
            Person = personMailAddressInfo,
        };

        var domainOfInfluenceList = new List<VotingPersonTypeDomainOfInfluenceInfo>();

        foreach (var authorizationObjectType in authorization.AuthorizationObject)
        {
            domainOfInfluenceList.Add(new VotingPersonTypeDomainOfInfluenceInfo()
            {
                CountingCircle = new CountingCircleType
                {
                    CountingCircleId = authorizationObjectType.CountingCircle.CountingCircleIdentification,
                    CountingCircleName = authorizationObjectType.CountingCircle.CountingCircleName,
                },
                DomainOfInfluence = new DomainOfInfluenceType
                {
                    DomainOfInfluenceTypeProperty = (DomainOfInfluenceTypeType)authorizationObjectType.DomainOfInfluence.DomainOfInfluenceTypeProperty,
                    LocalDomainOfInfluenceIdentification = authorizationObjectType.DomainOfInfluence.DomainOfInfluenceIdentification,
                    DomainOfInfluenceName = authorizationObjectType.DomainOfInfluence.DomainOfInfluenceName ?? "?",
                },
            });
        }

        return new VotingPersonType
        {
            Person = nationality,
            DeliveryAddress = personMailAddress,
            DomainOfInfluenceInfo = domainOfInfluenceList,
        };
    }

    private PersonTypeAddressExtension GetAddressExtension(
        EVoting.Config.VoterType voterType,
        Ech0045VoterExtension? echVoter,
        CountryConfiguration physicalAddressCountryConfig)
    {
        var addressExtension = new PersonTypeAddressExtension();

        if (voterType.VoterTypeProperty == EVoting.Config.VoterTypeType.Swissabroad)
        {
            if (echVoter?.SwissAbroadPersonExtensionAddress != null)
            {
                addressExtension.Line1 = echVoter.SwissAbroadPersonExtensionAddress.Line1;
                addressExtension.Line2 = echVoter.SwissAbroadPersonExtensionAddress.Line2;
                addressExtension.Line3 = echVoter.SwissAbroadPersonExtensionAddress.Line3;
                addressExtension.Line4 = echVoter.SwissAbroadPersonExtensionAddress.Line4;
                addressExtension.Line5 = echVoter.SwissAbroadPersonExtensionAddress.Line5;
                addressExtension.Line6 = echVoter.SwissAbroadPersonExtensionAddress.Line6;
                addressExtension.Line7 = echVoter.SwissAbroadPersonExtensionAddress.Line7;
                return addressExtension;
            }
            else
            {
                _logger.LogInformation("Voter with id {VoterId} has no Ech-0045 swiss abroad address extension. Fallback address is used", voterType.VoterIdentification);
            }
        }

        var mrMrsString = voterType.Person.PhysicalAddress.MrMrs switch
        {
            EVoting.Config.MrMrsType.Item1 => "Frau",
            EVoting.Config.MrMrsType.Item2 => "Herr",
            EVoting.Config.MrMrsType.Item3 => "Fräulein",
            _ => string.Empty,
        };

        var townLine = physicalAddressCountryConfig.ZipCodeTownControl
            ? $"{voterType.Person.PhysicalAddress.Town} {voterType.Person.PhysicalAddress.ZipCode}"
            : $"{voterType.Person.PhysicalAddress.ZipCode} {voterType.Person.PhysicalAddress.Town}";

        var streetLine = physicalAddressCountryConfig.StreetNrControl
            ? $"{voterType.Person.PhysicalAddress.HouseNumber} {voterType.Person.PhysicalAddress.Street}"
            : $"{voterType.Person.PhysicalAddress.Street} {voterType.Person.PhysicalAddress.HouseNumber}";

        var postOfficeLine = !string.IsNullOrEmpty(voterType.Person.PhysicalAddress.PostOfficeBoxText)
            ? voterType.Person.PhysicalAddress.PostOfficeBoxText + " " +
                (voterType.Person.PhysicalAddress.PostOfficeBoxNumber == 0 ? "" : voterType.Person.PhysicalAddress.PostOfficeBoxNumber.ToString())
            : string.Empty;

        addressExtension.Line1 = mrMrsString;
        addressExtension.Line2 = $"{voterType.Person.PhysicalAddress.FirstName} {voterType.Person.PhysicalAddress.LastName}";
        addressExtension.Line4 = streetLine;

        if (voterType.VoterTypeProperty != EVoting.Config.VoterTypeType.Swissabroad)
        {
            if (voterType.Person.PhysicalAddress.BelowNameLine != null &&
                voterType.Person.PhysicalAddress.BelowNameLine.Count > 0)
            {
                addressExtension.Line3 = string.Join(" ", voterType.Person.PhysicalAddress.BelowNameLine);
            }

            if (voterType.Person.PhysicalAddress.BelowStreetLine != null &&
                voterType.Person.PhysicalAddress.BelowStreetLine.Count > 0)
            {
                addressExtension.Line5 = string.Join(" ", voterType.Person.PhysicalAddress.BelowStreetLine);
            }

            addressExtension.Line6 = postOfficeLine;
            addressExtension.Line7 = townLine;
        }
        else
        {
            addressExtension.Line1 = string.Empty;
            addressExtension.Line5 = postOfficeLine;
            addressExtension.Line6 = townLine;
            addressExtension.Line7 = physicalAddressCountryConfig.Name.ToUpperInvariant();
        }

        return addressExtension;
    }
}

