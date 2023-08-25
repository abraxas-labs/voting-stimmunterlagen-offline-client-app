using eCH_0008_3_0;
using eCH_0010_6_0;
using eCH_0011_8_1;
using eCH_0021_7_0;
using eCH_0044_4_1;
using eCH_0045_4_0;
using eCH_0155_4_0;
using eCH_0228;
using EchDeliveryGeneration.Ech0045;
using System;
using System.Collections.Generic;
using SwissMunicipality = eCH_0007_6_0.SwissMunicipality;
using CantonAbbreviation = eCH_0007_5_0.CantonAbbreviation;
using Microsoft.Extensions.Logging;
using EchDeliveryGeneration.Configurations.Country;
using EchDeliveryGeneration.ErrorHandling;

namespace EchDeliveryGeneration.Post;

public class VoterMapper
{
    private readonly ILogger<VoterMapper> _logger;

    public VoterMapper(ILogger<VoterMapper> logger)
    {
        _logger = logger;
    }

    public VotingPersonType MapToEchVoter(voterType voterType, authorizationType authorization, Ech0045VoterExtension? echVoter)
    {
        var personInfo = voterType.person;
        var nationality = default(Nationality);
        var country10 = CountryType.Create(null, voterType.person.residenceCountryId, voterType.person.residenceCountryId);
        var country08 = Country.Create(null, voterType.person.residenceCountryId, voterType.person.residenceCountryId);
        var sexType = personInfo.sex == personTypeSex.Item1 ? SexType.Männlich : SexType.Weiblich;
        var dateOfBirth = DatePartiallyKnown.Create(personInfo.dateOfBirth);

        var physicalAddressCountryConfig = CountryConfigurationProvider.GetCountryConfiguration(voterType.person.physicalAddress.country)
            ?? throw new TransformationException(TransformationErrorCode.CountryNotFound, voterType.person.physicalAddress.country);

        var namedPersonId = NamedPersonId.Create("voting", voterType.voterIdentification);
        var personIdentification = PersonIdentification.Create(namedPersonId, personInfo.officialName,
            personInfo.firstName, sexType, dateOfBirth);

        var swissPersonType =
            SwissPersonType.Create(
                personIdentification,
                (LanguageType)personInfo.languageOfCorrespondance,
                new List<PlaceOfOrigin>() { PlaceOfOrigin.Create(EchDeliveryGenerationConstants.DefaultPlaceholder, CantonAbbreviation.SG) });

        var addressExtension = GetAddressExtension(voterType, echVoter, physicalAddressCountryConfig);
        swissPersonType.Extension = new AddressOnEnvelope() { Address = addressExtension };

        var swissMunicipality = SwissMunicipality.Create(personInfo.municipality.municipalityName);
        if (int.TryParse(personInfo.municipality.municipalityId, out var bfs))
            swissMunicipality.MunicipalityId = bfs;

        var mrMrs = (MrMrsType)personInfo.physicalAddress.mrMrs;

        var personMailAddressInfo = PersonMailAddressInfoType.Create(mrMrs, personInfo.physicalAddress.title,
            personInfo.physicalAddress.firstName, personInfo.physicalAddress.lastName);

        var addressInformationType = AddressInformationType.Create(personInfo.physicalAddress.town, country10);
        addressInformationType.DwellingNumber = voterType.person.physicalAddress.dwellingNumber;
        addressInformationType.HouseNumber = voterType.person.physicalAddress.houseNumber;
        addressInformationType.Street = voterType.person.physicalAddress.street;

        if (int.TryParse(voterType.person.physicalAddress.zipCode, out var zipCode) && zipCode > 999 && zipCode < 10000)
            addressInformationType.SwissZipCode = zipCode;

        switch (voterType.voterType1)
        {
            case voterTypeType.SWISSRESIDENT:
                var swissDomesticType = SwissDomesticType.Create(swissPersonType, swissMunicipality);

                nationality = Nationality.Create(swissDomesticType);
                break;
            case voterTypeType.SWISSABROAD:
                var swissAbroadType = SwissAbroadType.Create(swissPersonType, DateTime.MinValue, country08, swissMunicipality);

                addressInformationType.ForeignZipCode = voterType.person.physicalAddress.zipCode;

                nationality = Nationality.Create(swissAbroadType);
                break;
            case voterTypeType.FOREIGNER:
                var foreignerPerson = ForeignerPersonType.Create(personIdentification,
                    (LanguageType)personInfo.languageOfCorrespondance);

                nationality = Nationality.Create(ForeignerType.Create(foreignerPerson, swissMunicipality));
                break;
        }

        var personMailAddress = PersonMailAddressType.Create(personMailAddressInfo, addressInformationType);

        var domainOfInfluenceList = new List<DomainOfInfluenceInfoType>();

        foreach (var authorizationObjectType in authorization.authorizationObject)
        {
            domainOfInfluenceList.Add(new DomainOfInfluenceInfoType()
            {
                CountingCircle = CountingCircleType.Create(
                    authorizationObjectType.countingCircle.countingCircleIdentification,
                    authorizationObjectType.countingCircle.countingCircleName),
                DomainOfInfluence = DomainOfInfluenceType.Create(
                    GetDomainOfInfluenceTypeFromType(authorizationObjectType.domainOfInfluence.domainOfInfluenceType1),
                    authorizationObjectType.domainOfInfluence.domainOfInfluenceIdentification,
                    authorizationObjectType.domainOfInfluence.domainOfInfluenceName ?? "?"),
            });
        }

        var votingPerson = VotingPersonType.Create(nationality, DataLockType.KeineSperre, personMailAddress,
            domainOfInfluenceList);

        return votingPerson;
    }

    private PersonTypeAddressExtension GetAddressExtension(
        voterType voterType,
        Ech0045VoterExtension? echVoter,
        CountryConfiguration physicalAddressCountryConfig)
    {
        var addressExtension = new PersonTypeAddressExtension();

        if (voterType.voterType1 == voterTypeType.SWISSABROAD)
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
                _logger.LogInformation("Voter with id {VoterId} has no Ech-0045 swiss abroad address extension. Fallback address is used", voterType.voterIdentification);
            }
        }

        var mrMrsItem2String = voterType.person.physicalAddress.mrMrs == mrMrsType.Item2
            ? "Herr"
            : "Fräulein";

        var mrMrsString = voterType.person.physicalAddress.mrMrs == mrMrsType.Item1
            ? "Frau"
            : mrMrsItem2String;

        var townLine = physicalAddressCountryConfig.ZipCodeTownControl
            ? $"{voterType.person.physicalAddress.town} {voterType.person.physicalAddress.zipCode}"
            : $"{voterType.person.physicalAddress.zipCode} {voterType.person.physicalAddress.town}";

        var streetLine = physicalAddressCountryConfig.StreetNrControl
            ? $"{voterType.person.physicalAddress.houseNumber} {voterType.person.physicalAddress.street}"
            : $"{voterType.person.physicalAddress.street} {voterType.person.physicalAddress.houseNumber}";

        var postOfficeLine = !string.IsNullOrEmpty(voterType.person.physicalAddress.postOfficeBoxText)
            ? voterType.person.physicalAddress.postOfficeBoxText + " " +
                (voterType.person.physicalAddress.postOfficeBoxNumber == 0 ? "" : voterType.person.physicalAddress.postOfficeBoxNumber.ToString())
            : string.Empty;

        addressExtension.Line1 = mrMrsString;
        addressExtension.Line2 = $"{voterType.person.physicalAddress.firstName} {voterType.person.physicalAddress.lastName}";
        addressExtension.Line4 = streetLine;

        if (voterType.voterType1 != voterTypeType.SWISSABROAD)
        {
            if (voterType.person.physicalAddress.belowNameLine != null &&
                voterType.person.physicalAddress.belowNameLine.Length > 0)
            {
                addressExtension.Line3 = string.Join(" ", voterType.person.physicalAddress.belowNameLine);
            }

            if (voterType.person.physicalAddress.belowStreetLine != null &&
                voterType.person.physicalAddress.belowStreetLine.Length > 0)
            {
                addressExtension.Line5 = string.Join(" ", voterType.person.physicalAddress.belowStreetLine);
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


    private static DomainOfInfluenceTypeType GetDomainOfInfluenceTypeFromType(domainOfInfluenceTypeDomainOfInfluenceType type)
    {
        var returnValue = default(DomainOfInfluenceTypeType);
        switch (type)
        {
            case domainOfInfluenceTypeDomainOfInfluenceType.CH:
                returnValue = DomainOfInfluenceTypeType.CH;
                break;
            case domainOfInfluenceTypeDomainOfInfluenceType.CT:
                returnValue = DomainOfInfluenceTypeType.CT;
                break;
            case domainOfInfluenceTypeDomainOfInfluenceType.BZ:
                returnValue = DomainOfInfluenceTypeType.BZ;
                break;
            case domainOfInfluenceTypeDomainOfInfluenceType.MU:
                returnValue = DomainOfInfluenceTypeType.MU;
                break;
            case domainOfInfluenceTypeDomainOfInfluenceType.SC:
                returnValue = DomainOfInfluenceTypeType.SC;
                break;
            case domainOfInfluenceTypeDomainOfInfluenceType.KI:
                returnValue = DomainOfInfluenceTypeType.KI;
                break;
            case domainOfInfluenceTypeDomainOfInfluenceType.OG:
                returnValue = DomainOfInfluenceTypeType.OG;
                break;
            case domainOfInfluenceTypeDomainOfInfluenceType.KO:
                returnValue = DomainOfInfluenceTypeType.KO;
                break;
            case domainOfInfluenceTypeDomainOfInfluenceType.SK:
                returnValue = DomainOfInfluenceTypeType.SK;
                break;
            case domainOfInfluenceTypeDomainOfInfluenceType.AN:
                returnValue = DomainOfInfluenceTypeType.AN;
                break;
        }

        return returnValue;
    }
}

