using System;
using System.Collections.Generic;
using System.Linq;
using chVoteToJsonConverter.ErrorHandling;
using eCH_0008_3_0;
using eCH_0010_5_1;
using eCH_0010_6_0;
using eCH_0011_8_1;
using eCH_0021_7_0;
using eCH_0044_4_1;
using eCH_0045_4_0;
using eCH_0155_4_0;
using eCH_0228;
using CantonAbbreviation = eCH_0007_5_0.CantonAbbreviation;
using ContestType = eCH_0155_4_0.ContestType;
using SwissMunicipality = eCH_0007_6_0.SwissMunicipality;
using Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;
using chVoteToJsonConverter.Configurations.Country;

namespace chVoteToJsonConverter;

public static class DataTransformer
{
    public static Delivery Transform(List<configuration> configuration, List<votingCardList> votingCardList, Configuration jsonConfig)
    {
        var delivery = new Delivery
        {
            VotingCardDelivery = new(),
        };

        var votingCardDataList = new List<votingCardDataType>();

        foreach (var cardList in votingCardList)
        {
            var referencesConfigs = configuration
                .Where(x => x.contest.contestIdentification.Equals(cardList.contest.contestIdentification))
                .ToList();

            if (referencesConfigs.Count > 1)
                throw new TransformationException(TransformationErrorCode.ContestDuplicates, cardList.contest.contestIdentification);

            var referenceConfig = referencesConfigs.SingleOrDefault();
            if (referenceConfig == null)
                throw new TransformationException(TransformationErrorCode.ContestNotFound, cardList.contest.contestIdentification);

            votingCardDataList.AddRange(GetVotingCards(cardList, referenceConfig, jsonConfig));

            AddContestDataToDelivery(ref delivery, referenceConfig.contest);
        }
        delivery.VotingCardDelivery.VotingCardData = votingCardDataList;

        SetDeliveryExtension(delivery, jsonConfig);

        return delivery;
    }

    public static Delivery Transform(List<VoterDelivery> voterDeliverys, Configuration jsonConfig)
    {

        var delivery = new Delivery
        {
            VotingCardDelivery = new(),
        };
        var votingCardDataList = new List<votingCardDataType>();

        foreach (var voterDelivery in voterDeliverys)
        {
            if (voterDelivery.ElementTypeName == VoterChoiceIdentifier.voterList)
            {
                foreach (var voter in ((VoterListType)voterDelivery.VoterChoice).Voter)
                {
                    var bfs = GetBfsFromNationality(voter.Person);
                    var printing = jsonConfig.Printings.FirstOrDefault(x =>
                        x.Municipalities.Any(y => y.Bfs.Equals(bfs))) ?? throw new TransformationException(TransformationErrorCode.PrintingNotFoundInZip, bfs);

                    votingCardDataList.Add(GetVotingCardDataType(voter, printing, bfs));
                }
            }

            delivery.VotingCardDelivery.VotingCardData = votingCardDataList;


            delivery.VotingCardDelivery.ContestData = GetContestDataType(voterDelivery);
        }

        SetDeliveryExtension(delivery, jsonConfig);

        return delivery;

    }

    private static void SetDeliveryExtension(Delivery delivery, Configuration jsonConfiguration)
    {
        var deliveryExtension = new DeliveryExtension
        {
            Certificates = jsonConfiguration.Certificates,
        };

        foreach (var jsonConfigurationPrinting in jsonConfiguration.Printings)
        {
            if (!deliveryExtension.Printings.ContainsKey(jsonConfigurationPrinting.Name))
                deliveryExtension.Printings.Add(jsonConfigurationPrinting.Name, new SmallPrinting(jsonConfigurationPrinting));

            foreach (var municipality in jsonConfigurationPrinting.Municipalities)
            {
                if (!deliveryExtension.Municipalities.ContainsKey(municipality.Bfs))
                    deliveryExtension.Municipalities.Add(municipality.Bfs, municipality);
            }
        }

        delivery.Extension = deliveryExtension;
    }

    private static ContestDataType GetContestDataType(VoterDelivery voterDelivery)
    {
        var contestInfo = ((VoterListType)voterDelivery.VoterChoice).Contest;
        var contestDescriptionInformation = contestInfo.ContestDescription;

        var contestType = ContestType.Create(
            Guid.NewGuid().ToString(),
            contestInfo.ContestDate,
            contestDescriptionInformation,
            EvotingPeriodType.Create(DateTime.Now, DateTime.Now));

        return new ContestDataType()
        {
            Item = contestType,
        };
    }

    private static string GetBfsFromNationality(Nationality voterNationality)
    {
        string? bfs = null;

        switch (voterNationality.ElementTypeName)
        {
            case NationalityChoiceIdentifier.swiss:
                bfs = ((SwissDomesticType)voterNationality.NationalityChoice).Municipality.MunicipalityId?.ToString();
                break;
            case NationalityChoiceIdentifier.swissAbroad:
                var swissAbroadType = ((SwissAbroadType)voterNationality.NationalityChoice);
                if (swissAbroadType.ElementTypeName == PlaceChoiceIdentifier.municipality)
                    bfs = ((SwissMunicipality)swissAbroadType.PlaceChoice).MunicipalityId?.ToString();
                break;
            case NationalityChoiceIdentifier.foreigner:
                bfs = ((ForeignerType)voterNationality.NationalityChoice).Municipality.MunicipalityId?.ToString();
                break;
        }

        return bfs ?? "0000";
    }

    private static void AddAddressExtension(VotingPersonType votingPersonType)
    {
        var addressExtension = GetAddressExtension(votingPersonType.ElectoralAddress, votingPersonType.Person.ElementTypeName);

        switch (votingPersonType.Person.ElementTypeName)
        {
            case NationalityChoiceIdentifier.swiss:
                if (((SwissDomesticType)votingPersonType.Person.NationalityChoice).SwissDomesticPerson.Extension == null)
                    ((SwissDomesticType)votingPersonType.Person.NationalityChoice).SwissDomesticPerson.Extension = new AddressOnEnvelope() { Address = addressExtension }; ;
                break;
            case NationalityChoiceIdentifier.swissAbroad:
                if (((SwissAbroadType)votingPersonType.Person.NationalityChoice).SwissAbroadPerson.Extension == null)
                    ((SwissAbroadType)votingPersonType.Person.NationalityChoice).SwissAbroadPerson.Extension = new AddressOnEnvelope() { Address = addressExtension };
                break;
            case NationalityChoiceIdentifier.foreigner:
                if (((ForeignerType)votingPersonType.Person.NationalityChoice).ForeignerPerson.Extension == null)
                    ((ForeignerType)votingPersonType.Person.NationalityChoice).ForeignerPerson.Extension = new AddressOnEnvelope() { Address = addressExtension };
                break;
        }

    }

    private static votingCardDataType GetVotingCardDataType(VotingPersonType votingPersonType, Printing printing, string votingPersonMuicipalityBfs)
    {
        var votingCard = new votingCardDataType();

        var municipality = printing.Municipalities.First(x => x.Bfs.Equals(votingPersonMuicipalityBfs));

        votingCard.votingCardSequenceNumber = Guid.NewGuid().ToString();
        AddAddressExtension(votingPersonType);
        votingCard.Item = votingPersonType;

        var printingJsonConfig = printing;
        var municipaityJsonConfig =
            printingJsonConfig.Municipalities.Single(x => x.Bfs.Equals(votingPersonMuicipalityBfs));

        municipaityJsonConfig.PollOpening = municipality.PollOpening;
        municipaityJsonConfig.PollClosing = municipality.PollClosing;

        if (municipaityJsonConfig.ReturnDeliveryAddress != null)
        {
            var organisationMailAddressInfo = OrganisationMailAddressInfo.Create(municipaityJsonConfig.Name);
            var addressInformation = AddressInformation.Create(municipaityJsonConfig.Name, "CH");
            addressInformation.AddressLine1 = municipaityJsonConfig.ReturnDeliveryAddress.AddressField1;
            addressInformation.AddressLine2 = municipaityJsonConfig.ReturnDeliveryAddress.AddressField2;
            addressInformation.Street = municipaityJsonConfig.ReturnDeliveryAddress.Street;
            if (int.TryParse(municipaityJsonConfig.ReturnDeliveryAddress.Plz, out var plz) && plz > 999 &&
                plz < 10000)
                addressInformation.SwissZipCode = plz;

            votingCard.VotingCardReturnAddress = OrganisationMailAddress.Create(organisationMailAddressInfo, addressInformation);
        }

        votingCard.Extension = new eCH228JsonConfig(printing.Name, municipaityJsonConfig.Bfs);

        return votingCard;
    }

    private static void AddContestDataToDelivery(ref Delivery delivery, contestType contest)
    {
        var contestType = ContestType.Create(contest.contestIdentification, contest.contestDate);

        var contestDescriptionInfo = new List<ContestDescriptionInfo>();

        foreach (var description in contest.contestDescription)
        {
            contestDescriptionInfo.Add(ContestDescriptionInfo.Create(description.language.ToString(), description.contestDescription));
        }
        contestType.ContestDescription = ContestDescriptionInformation.Create(contestDescriptionInfo);

        contestType.EvotingPeriod = EvotingPeriodType.Create(contest.evotingFromDate, contest.evotingToDate);

        delivery.VotingCardDelivery.ContestData = new ContestDataType()
        {
            Item = contestType,
        };
    }

    private static votingCardDataType[] GetVotingCards(votingCardList votingCardList,
        configuration configuration, Configuration jsonConfig)
    {
        var votingCards = new List<votingCardDataType>();

        foreach (var votingCardType in votingCardList.contest.votingCard)
        {
            var votingCard = new votingCardDataType();

            var personConfig = configuration.register.FirstOrDefault(x =>
                x.voterIdentification.Equals(votingCardType.voterIdentification));
            if (personConfig == null)
                throw new TransformationException(TransformationErrorCode.VoterNotFound, votingCardType.voterIdentification);

            var authorization = configuration.authorizations.SingleOrDefault(x =>
                x.authorizationIdentification.Equals(personConfig.authorization));
            if (authorization == null)
                throw new TransformationException(TransformationErrorCode.AuthorizationNotFound, personConfig.authorization);

            votingCard.votingCardSequenceNumber = votingCardType.voterIdentification;
            votingCard.Item = CreatePerson(personConfig, authorization);

            AddVotesAndEletion(ref votingCard, votingCardType, configuration);

            var votingPersonMuicipalityBfs = personConfig.person.municipality.municipalityId;
            var printingJsonConfig = jsonConfig.Printings.FirstOrDefault(x =>
                x.Municipalities.Any(y => y.Bfs.Equals(votingPersonMuicipalityBfs)));
            if (printingJsonConfig == null)
                throw new TransformationException(TransformationErrorCode.PrintingNotFoundInZip, votingPersonMuicipalityBfs);
            var municipaityJsonConfig =
                printingJsonConfig.Municipalities.Single(x => x.Bfs.Equals(votingPersonMuicipalityBfs));

            municipaityJsonConfig.PollOpening = authorization.authorizationFromDate.ToString();
            municipaityJsonConfig.PollClosing = authorization.authorizationToDate.ToString();

            if (municipaityJsonConfig.ReturnDeliveryAddress != null)
            {
                var organisationMailAddressInfo = OrganisationMailAddressInfo.Create(municipaityJsonConfig.Name);
                var addressInformation = AddressInformation.Create(municipaityJsonConfig.Name, "CH");
                addressInformation.AddressLine1 = municipaityJsonConfig.ReturnDeliveryAddress.AddressField1;
                addressInformation.AddressLine2 = municipaityJsonConfig.ReturnDeliveryAddress.AddressField2;
                addressInformation.Street = municipaityJsonConfig.ReturnDeliveryAddress.Street;
                if (int.TryParse(municipaityJsonConfig.ReturnDeliveryAddress.Plz, out var plz) && plz > 999 &&
                    plz < 10000)
                    addressInformation.SwissZipCode = plz;

                votingCard.VotingCardReturnAddress = OrganisationMailAddress.Create(organisationMailAddressInfo, addressInformation);
            }

            votingCard.Extension = new eCH228JsonConfig(printingJsonConfig.Name, municipaityJsonConfig.Bfs);

            votingCards.Add(votingCard);
        }

        return votingCards.ToArray();
    }


    private static object CreatePerson(voterType voterType, authorizationType authorization)
    {
        var personInfo = voterType.person;
        var nationality = default(Nationality);
        var country10 = CountryType.Create(null, voterType.person.residenceCountryId, voterType.person.residenceCountryId);
        var country08 = Country.Create(null, voterType.person.residenceCountryId, voterType.person.residenceCountryId);

        var physicalAddressCountryConfig = CountryConfigurationProvider.GetCountryConfiguration(voterType.person.physicalAddress.country)
            ?? throw new TransformationException(TransformationErrorCode.CountryNotFound, voterType.person.physicalAddress.country);

        var sexType = personInfo.sex == personTypeSex.Item1 ? SexType.Männlich : SexType.Weiblich;
        var dateOfBirth = DatePartiallyKnown.Create(personInfo.dateOfBirth);

        var namedPersonId = NamedPersonId.Create("voting", voterType.voterIdentification);
        var personIdentification = PersonIdentification.Create(namedPersonId, personInfo.officialName,
            personInfo.firstName, sexType, dateOfBirth);

        var swissPersonType =
            SwissPersonType.Create(personIdentification, (LanguageType)personInfo.languageOfCorrespondance, new List<PlaceOfOrigin>() { PlaceOfOrigin.Create("Beispielstadt", CantonAbbreviation.SG) });

        var addressExtension = GetAddressExtension(voterType, physicalAddressCountryConfig);

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
                var swissAbroadType = SwissAbroadType.Create(swissPersonType, DateTime.Now, country08, swissMunicipality);

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
                CountingCircle = CountingCircleType.Create(authorizationObjectType.countingCircle.id, authorizationObjectType.countingCircle.name),
                DomainOfInfluence = DomainOfInfluenceType.Create(GetDomainOfInfluenceTypeFromString(authorizationObjectType.domainOfInfluence.type), authorizationObjectType.domainOfInfluence.localId, authorizationObjectType.domainOfInfluence.id),
            });
        }

        var votingPerson = VotingPersonType.Create(nationality, DataLockType.KeineSperre, personMailAddress,
            domainOfInfluenceList);

        return votingPerson;
    }

    private static PersonTypeAddressExtension GetAddressExtension(voterType voterType, CountryConfiguration physicalAddressCountryConfig)
    {
        var addressExtension = new PersonTypeAddressExtension();

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

        var postOfficeLine = voterType.person.physicalAddress.postOfficeBoxText + " " +
                                     (voterType.person.physicalAddress.postOfficeBoxNumber == 0
                                         ? ""
                                         : voterType.person.physicalAddress.postOfficeBoxNumber.ToString());

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

    private static PersonTypeAddressExtension GetAddressExtension(PersonMailAddressType personMailAddressType, NationalityChoiceIdentifier nationality)
    {
        var addressExtension = new PersonTypeAddressExtension();
        var addressInformation = personMailAddressType.AddressInformation;
        var personMailInfo = personMailAddressType.PersonMailAddressInfo;

        var mrMrsString = string.Empty;
        if (personMailInfo.MrMrs.HasValue)
        {
            var mrMrsHerrString = personMailInfo.MrMrs.Value == MrMrsType.Herr
                ? "Herr"
                : "Fräulein";

            mrMrsString = personMailInfo.MrMrs.Value == MrMrsType.Frau
                ? "Frau"
                : mrMrsHerrString;
        }

        addressExtension.Line1 = mrMrsString;
        addressExtension.Line2 = personMailInfo.LastName + " " +
                                 personMailInfo.FirstName;
        addressExtension.Line3 = addressInformation.AddressLine1;
        addressExtension.Line4 = addressInformation.AddressLine2;
        addressExtension.Line5 = addressInformation.Street + " " +
                                 addressInformation.HouseNumber;
        addressExtension.Line6 = addressInformation.PostOfficeBoxText + " " +
                                 addressInformation.PostOfficeBoxNumber;
        if (nationality == NationalityChoiceIdentifier.swissAbroad)
        {
            addressExtension.Line7 = addressInformation.ForeignZipCode + " " +
                                     addressInformation.Town;
            addressExtension.Line7 += " " + addressInformation.Country.CountryNameShort;
        }
        else
        {
            addressExtension.Line7 = addressInformation.SwissZipCode + " " +
                                     addressInformation.Town;
        }
        return addressExtension;
    }

    private static void AddVotesAndEletion(ref votingCardDataType votingCardDataType, votingCardType votingCardSource,
        configuration configSource)
    {
        var individualCodes = new votingCardIndividualCodesType();
        var voteList = new List<eCH_0228.voteType>();
        var electionList = new List<votingCardIndividualCodesTypeElectionGroupBallot>();

        individualCodes.individualContestCodes = new namedCodeType[]
        {
            new namedCodeType() {codeDesignation = "startVotingKey", codeValue = votingCardSource.startVotingKey},
            new namedCodeType() {codeDesignation = "ballotCastingKey", codeValue = votingCardSource.ballotCastingKey},
            new namedCodeType() {codeDesignation = "voteCastCode", codeValue = votingCardSource.voteCastCode},
            new namedCodeType() {codeDesignation = "votingCardId", codeValue = votingCardSource.votingCardId},
        };

        if (votingCardSource.vote != null)
        {
            foreach (var vote in votingCardSource.vote)
            {
                var config = configSource.contest.voteInformation.SingleOrDefault(x =>
                    x.vote.voteIdentification.Equals(vote.voteIdentification));
                if (config == null)
                    throw new TransformationException(TransformationErrorCode.VoteNotFoud, vote.voteIdentification);

                voteList.Add(GetVote(vote, config));
            }
        }

        if (votingCardSource.election != null)
        {
            foreach (var election in votingCardSource.election)
            {
                var config = configSource.contest.electionInformation.SingleOrDefault(x =>
                    x.election.electionIdentification.Equals(election.electionIdentification));
                if (config == null)
                    throw new TransformationException(TransformationErrorCode.ElectionNotFound, election.electionIdentification);

                election.writeInsChoiceCode ??= Array.Empty<string>();

                electionList.Add(GetElection(election, config));
            }
        }

        individualCodes.vote = voteList.ToArray();
        votingCardDataType.votingCardIndividualCodes = individualCodes;
        individualCodes.electionGroupBallot = electionList.ToArray();
    }

    private static eCH_0228.voteType GetVote(voteType1 voteTypeSource, voteInformationType configInfo)
    {
        var vote = new eCH_0228.voteType
        {
            VoteIdentification = voteTypeSource.voteIdentification
        };

        var voteDescriptions = new List<VoteDescriptionInfoType>();
        foreach (var voteDescription in configInfo.vote.voteDescription)
        {
            voteDescriptions.Add(VoteDescriptionInfoType.Create(voteDescription.language.ToString(), voteDescription.voteDescription));
        }

        vote.VoteDescription = VoteDescriptionInformationType.Create(voteDescriptions);

        var voteTypeBallotList = new List<voteTypeBallot>();

        foreach (var ballotType in configInfo.vote.ballot)
        {
            voteTypeBallotList.Add(GetVoteTypeBallots(voteTypeSource.question, ballotType));
        }

        vote.ballot = voteTypeBallotList.ToArray();

        return vote;
    }

    private static voteTypeBallot GetVoteTypeBallots(questionType[] questions, ballotType ballotTypeConfig)
    {
        var voteTypeBallot = new voteTypeBallot();
        var ballotDescriptionInfos = new List<BallotDescriptionInfo>();

        voteTypeBallot.BallotIdentification = ballotTypeConfig.ballotIdentification;
        voteTypeBallot.BallotPosition = int.Parse(ballotTypeConfig.ballotPosition);

        foreach (var ballotDescriptionInfo in ballotTypeConfig.ballotDescription)
        {
            ballotDescriptionInfos.Add(BallotDescriptionInfo.Create(ballotDescriptionInfo.language.ToString(), ballotDescriptionInfo.ballotDescriptionLong, ballotDescriptionInfo.ballotDescriptionShort));
        }

        voteTypeBallot.BallotDescription = BallotDescriptionInformation.Create(ballotDescriptionInfos);

        if (ballotTypeConfig.Item is standardBallotType standardBallot)
        {
            var standardBallotItem = new voteTypeBallotStandardBallot();
            var ballotQuestionInfoList = new List<BallotQuestionInfo>();

            standardBallotItem.QuestionIdentification = standardBallot.questionIdentification;

            foreach (var ballotQuestionTypeBallotQuestionInfo in standardBallot.ballotQuestion)
            {
                if (string.IsNullOrEmpty(ballotQuestionTypeBallotQuestionInfo.ballotQuestionTitle))
                {
                    ballotQuestionInfoList.Add(BallotQuestionInfo.Create(
                        ballotQuestionTypeBallotQuestionInfo.language.ToString(),
                        ballotQuestionTypeBallotQuestionInfo.ballotQuestion));
                }
                else
                {
                    ballotQuestionInfoList.Add(BallotQuestionInfo.Create(
                        ballotQuestionTypeBallotQuestionInfo.language.ToString(),
                        ballotQuestionTypeBallotQuestionInfo.ballotQuestionTitle,
                        ballotQuestionTypeBallotQuestionInfo.ballotQuestion));
                }
            }

            standardBallotItem.BallotQuestion = BallotQuestion.Create(ballotQuestionInfoList);

            var questionsConfig = questions.SingleOrDefault(x =>
                x.questionIdentification.Equals(standardBallot.questionIdentification));
            if (questionsConfig == null)
                throw new TransformationException(TransformationErrorCode.QuestionNotFound, standardBallot.questionIdentification);

            standardBallotItem.answerOption = GetAnswerList(questionsConfig, standardBallot.answer).ToArray();

            voteTypeBallot.Item = standardBallotItem;
        }
        else if (ballotTypeConfig.Item is variantBallotType variantBallot)
        {
            var variantBallotItem = new voteTypeBallotVariantBallot();

            var standardQuestionList = new List<voteTypeBallotVariantBallotQuestionInformation>();
            var tieBreakQuestionList = new List<voteTypeBallotVariantBallotTieBreakInformation>();

            foreach (var standardQuestionType in variantBallot.standardQuestion)
            {
                var ballotQuestionInfoList = new List<BallotQuestionInfo>();

                foreach (var ballotQuestionTypeBallotQuestionInfo in standardQuestionType.ballotQuestion)
                {
                    var ballotQuestion = BallotQuestionInfo.Create(
                        ballotQuestionTypeBallotQuestionInfo.language.ToString(),
                        ballotQuestionTypeBallotQuestionInfo.ballotQuestion);
                    if (ballotQuestionTypeBallotQuestionInfo.ballotQuestionTitle != null)
                    {
                        ballotQuestion.BallotQuestionTitle = ballotQuestionTypeBallotQuestionInfo.ballotQuestionTitle;
                    }
                    ballotQuestionInfoList.Add(ballotQuestion);
                }

                int.TryParse(standardQuestionType.questionNumber, out var questionNumber);
                int.TryParse(standardQuestionType.questionPosition, out var questionPosition);

                var questionConfig = questions.SingleOrDefault(x =>
                    x.questionIdentification.Equals(standardQuestionType.questionIdentification));
                if (questionConfig == null)
                    throw new TransformationException(TransformationErrorCode.QuestionNotFound, standardQuestionType.questionIdentification);

                standardQuestionList.Add(new voteTypeBallotVariantBallotQuestionInformation()
                {
                    BallotQuestion = BallotQuestion.Create(ballotQuestionInfoList),
                    BallotQuestionNumber = questionNumber,
                    QuestionIdentification = standardQuestionType.questionIdentification,
                    questionInformation = GetAnswerList(questionConfig, standardQuestionType.answer).ToArray(),
                    QuestionPosition = questionPosition
                });
            }

            variantBallotItem.questionInformation = standardQuestionList.ToArray();

            foreach (var tieBreakQuestionType in variantBallot.tieBreakQuestion)
            {
                var ballotQuestionInfoList = new List<TieBreakQuestionInfo>();

                foreach (var ballotQuestionTypeBallotQuestionInfo in tieBreakQuestionType.ballotQuestion)
                {
                    var tieBreakQuestion = TieBreakQuestionInfo.Create(
                        ballotQuestionTypeBallotQuestionInfo.language.ToString(),
                        ballotQuestionTypeBallotQuestionInfo.ballotQuestion);
                    ballotQuestionInfoList.Add(tieBreakQuestion);
                }

                int.TryParse(tieBreakQuestionType.questionNumber, out var questionNumber);
                int.TryParse(tieBreakQuestionType.questionPosition, out var questionPosition);

                var questionConfig = questions.SingleOrDefault(x =>
                    x.questionIdentification.Equals(tieBreakQuestionType.questionIdentification));
                if (questionConfig == null)
                    throw new TransformationException(TransformationErrorCode.QuestionNotFound, tieBreakQuestionType.questionIdentification);

                tieBreakQuestionList.Add(new voteTypeBallotVariantBallotTieBreakInformation()
                {
                    BallotQuestion = TieBreakQuestion.Create(ballotQuestionInfoList),
                    TieBreakQuestionNumber = questionNumber,
                    QuestionIdentification = tieBreakQuestionType.questionIdentification,
                    questionInformation = GetAnswerList(questionConfig, tieBreakQuestionType.answer).ToArray(),
                    QuestionPosition = questionPosition
                });
            }

            variantBallotItem.tieBreakInformation = tieBreakQuestionList.ToArray();

            voteTypeBallot.Item = variantBallotItem;
        }

        return voteTypeBallot;
    }

    private static List<answerOptionType> GetAnswerList(questionType question, standardAnswerType[] answers)
    {
        var answerList = new List<answerOptionType>();
        foreach (var answerType in answers)
        {
            var answerTextInfoList = new List<answerOptionTypeAnswerTextInformation>();

            foreach (var answerText in answerType.answerInfo)
            {
                answerTextInfoList.Add(new answerOptionTypeAnswerTextInformation()
                {
                    Language = (LanguageType)answerText.language,
                    answerText = answerText.answer
                });
            }
            var answere = question.answer
                .SingleOrDefault(x => x.answerIdentification.Equals(answerType.answerIdentification));
            if (answere == null)
                throw new TransformationException(TransformationErrorCode.AnswereNotFound, answerType.answerIdentification);

            answerList.Add(new answerOptionType()
            {
                AnswerIdentification = answerType.answerIdentification,
                answerSequenceNumber = answerType.answerPosition,
                answerTextInformation = answerTextInfoList.ToArray(),
                individualVoteVerificationCode = answere.choiceCode
            });
        }
        return answerList;
    }

    private static List<answerOptionType> GetAnswerList(questionType question, tiebreakAnswerType[] answers)
    {
        var answerList = new List<answerOptionType>();
        foreach (var answerType in answers)
        {
            var answerTextInfoList = new List<answerOptionTypeAnswerTextInformation>();

            foreach (var answerText in answerType.answerInfo)
            {
                answerTextInfoList.Add(new answerOptionTypeAnswerTextInformation()
                {
                    Language = (LanguageType)answerText.language,
                    answerText = answerText.answer
                });
            }
            var answere = question.answer
                .SingleOrDefault(x => x.answerIdentification.Equals(answerType.answerIdentification));
            if (answere == null)
                throw new TransformationException(TransformationErrorCode.AnswereNotFound, answerType.answerIdentification);

            answerList.Add(new answerOptionType()
            {
                AnswerIdentification = answerType.answerIdentification,
                answerSequenceNumber = answerType.answerPosition,
                answerTextInformation = answerTextInfoList.ToArray(),
                individualVoteVerificationCode = answere.choiceCode
            });
        }
        return answerList;
    }

    private static votingCardIndividualCodesTypeElectionGroupBallot GetElection(electionType1 electionTypeSource, electionInformationType config)
    {
        var electionGroup = new votingCardIndividualCodesTypeElectionGroupBallot();
        var election = new eCH_0228.electionInformationType();

        int.TryParse(config.election.numberOfMandates, out var numberOfMandates);

        election.election = ElectionType.Create(electionTypeSource.electionIdentification, (TypeOfElectionType)config.election.typeOfElection, numberOfMandates);

        var electionDescriptionList = new List<ElectionDescriptionInfoType>();

        foreach (var electionDescriptionInfo in config.election.electionDescription)
        {
            electionDescriptionList.Add(ElectionDescriptionInfoType.Create(electionDescriptionInfo.language.ToString(), electionDescriptionInfo.electionDescription, electionDescriptionInfo.electionDescriptionShort));
        }

        election.election.ElectionDescription = ElectionDescriptionInformationType.Create(electionDescriptionList);

        //Majorz
        if (config.election.typeOfElection == electionTypeTypeOfElection.Item2)
        {
            var candidateList = new List<electionInformationTypeCandidate>();
            var candidates = electionTypeSource.candidate.ToList();

            if (electionTypeSource.list != null)
            {
                candidates.AddRange(electionTypeSource.list
                    .SelectMany(l => l.candidate?.ToList() ?? new List<candidateListType>())
                    .Select(c => new candidateType2 { candidateIdentification = c.candidateListIdentification, choiceCode = c.choiceCode }));
            }

            foreach (var candidateType2 in candidates)
            {
                var candidateConfig = config.candidate.SingleOrDefault(x =>
                    x.candidateIdentification.Equals(candidateType2.candidateIdentification));

                if (candidateConfig != null)
                {
                    var candidateTextOnPositionList = new List<CandidateTextInfo>();
                    var candidateReferenceList = new List<electionInformationTypeCandidateCandidateReference>();


                    foreach (var textInfo in candidateConfig.candidateText)
                    {
                        candidateTextOnPositionList.Add(CandidateTextInfo.Create(textInfo.language.ToString(),
                            textInfo.candidateText));
                    }
                    var candidateTextInformation = CandidateTextInformation.Create(candidateTextOnPositionList);
                    var candidateText = candidateConfig.familyName + " " + candidateConfig.callName;
                    var candidateTextOnPosition = CandidateTextInformation.Create(new List<CandidateTextInfo>()
                    {
                        CandidateTextInfo.Create("de", candidateText),
                        CandidateTextInfo.Create("fr", candidateText),
                        CandidateTextInfo.Create("it", candidateText),
                        CandidateTextInfo.Create("rm", candidateText),
                        CandidateTextInfo.Create("en", candidateText),
                    });

                    var choiceCodes = new List<namedCodeType>();

                    foreach (var code in candidateType2.choiceCode)
                    {
                        choiceCodes.Add(new namedCodeType() { codeDesignation = "choiceCode", codeValue = code });
                    }

                    candidateReferenceList.Add(new electionInformationTypeCandidateCandidateReference()
                    {
                        CandidateReference = candidateConfig.referenceOnPosition,
                        CandidateTextOnPosition = candidateTextOnPosition,
                        Occurences = electionTypeSource.candidate.Select(x =>
                                x.candidateIdentification.Equals(candidateType2.candidateIdentification)).Count()
                            .ToString(),
                        individualCandidateVerificationCode = choiceCodes.ToArray(),
                    });

                    candidateList.Add(new electionInformationTypeCandidate()
                    {
                        CandidateIdentification = candidateConfig.candidateIdentification,
                        CandidateTextinformation = candidateTextInformation,
                        candidateReference = candidateReferenceList.ToArray(),
                    });
                }
                else
                {
                    if (config.list != null && config.list.Length > 0)
                    {
                        var emptyCandidateConfig = config.list.FirstOrDefault()?.candidatePosition?.FirstOrDefault(
                            x => x.candidateListIdentification.Equals(candidateType2.candidateIdentification));

                        if (emptyCandidateConfig == null)
                            throw new TransformationException(TransformationErrorCode.CandidateNotFound, candidateType2.candidateIdentification);

                        var candidateTextOnPositionList = new List<CandidateTextInfo>();
                        var candidateReferenceList =
                            new List<electionInformationTypeCandidateCandidateReference>();

                        foreach (var textInfo in emptyCandidateConfig.candidateTextOnPosition)
                        {
                            candidateTextOnPositionList.Add(CandidateTextInfo.Create(
                                textInfo.language.ToString(),
                                textInfo.candidateText));
                        }
                        var candidateTextInformation =
                            CandidateTextInformation.Create(candidateTextOnPositionList);

                        var choiceCodes = new List<namedCodeType>();

                        foreach (var code in candidateType2.choiceCode)
                        {
                            choiceCodes.Add(new namedCodeType()
                            {
                                codeDesignation = "choiceCode",
                                codeValue = code
                            });
                        }

                        candidateReferenceList.Add(new electionInformationTypeCandidateCandidateReference()
                        {
                            CandidateReference = emptyCandidateConfig.candidateReferenceOnPosition,
                            CandidateTextOnPosition = candidateTextInformation,
                            Occurences = electionTypeSource.candidate.Select(x =>
                                    x.candidateIdentification.Equals(candidateType2
                                        .candidateIdentification))
                                .Count()
                                .ToString(),
                            individualCandidateVerificationCode = choiceCodes.ToArray(),
                        });

                        candidateList.Add(new electionInformationTypeCandidate()
                        {
                            CandidateIdentification = emptyCandidateConfig.candidateIdentification,
                            CandidateTextinformation = candidateTextInformation,
                            candidateReference = candidateReferenceList.ToArray(),
                        });
                    }
                }

                var writeInCodeList = new List<electionInformationTypeWriteInCodes>();

                for (int index = 0; electionTypeSource.writeInsChoiceCode.Length > index; index++)
                {
                    writeInCodeList.Add(new electionInformationTypeWriteInCodes()
                    {
                        position = (index + 1).ToString(),
                        individualWriteInVerificationCode = electionTypeSource.writeInsChoiceCode[index],
                        writeInCodeDesignation = new[]
                        {
                            new electionInformationTypeWriteInCodesWriteInCodeDesignation()
                                { Language = LanguageType.de, codeDesignationText = "Anderer wählbarer Kandidat"},
                        }
                    });
                }
                election.writeInCodes = writeInCodeList.ToArray();
                election.candidate = candidateList.ToArray();
            }
        }
        //Proporz
        else if (config.election.typeOfElection == electionTypeTypeOfElection.Item1)
        {
            var lists = new List<electionInformationTypeList>();
            foreach (var listSource in electionTypeSource.list)
            {
                var sourceConfig =
                    config.list.SingleOrDefault(x => x.listIdentification.Equals(listSource.listIdentification));
                if (sourceConfig == null)
                    throw new TransformationException(TransformationErrorCode.ListNotFound,
                        listSource.listIdentification);

                var listDescriptionList = new List<eCH_0155_4_0.ListDescriptionInfo>();
                int.TryParse(sourceConfig.listOrderOfPrecedence, out var listOrderOfPrecedence);
                var candidatePositions = new List<electionInformationTypeListCandidatePosition>();
                var listUnionDescriptionList = new List<ListUnionDescriptionInfoType>();
                var listUnionDescription = default(ListUnionDescriptionType);

                var listUnion = config.listUnion?.SingleOrDefault
                (x =>
                    x.listUnionType1 == listUnionTypeListUnionType.Item1 &&
                    x.referencedList.Any(y => y.Equals(listSource.listIdentification))
                );

                foreach (var description in sourceConfig.listDescription)
                {
                    listDescriptionList.Add(ListDescriptionInfo.Create(description.language.ToString(),
                        description.listDescription, description.listDescriptionShort));
                }

                if (listUnion != null && listUnion.listUnionDescription != null &&
                    listUnion.listUnionDescription.Length > 0)
                {
                    foreach (var listUnionDescriptionInfo in listUnion.listUnionDescription)
                    {
                        listUnionDescriptionList.Add(ListUnionDescriptionInfoType.Create(
                            listUnionDescriptionInfo.language.ToString(),
                            listUnionDescriptionInfo.listUnionDescription));
                    }
                    listUnionDescription = ListUnionDescriptionType.Create(listUnionDescriptionList);
                }

                foreach (var candidateListType in listSource.candidate)
                {
                    var listCandidatesChoiceCodes = new List<namedCodeType>();
                    var candidateListConfiguration = sourceConfig.candidatePosition.FirstOrDefault(x =>
                        x.candidateListIdentification.Equals(candidateListType.candidateListIdentification));
                    if (candidateListConfiguration == null)
                        throw new TransformationException(TransformationErrorCode.CandidateNotFound, candidateListType.candidateListIdentification);
                    var candidateConfiguration = config.candidate.FirstOrDefault(x =>
                        x.candidateIdentification.Equals(candidateListConfiguration.candidateIdentification));
                    int.TryParse(candidateListConfiguration.positionOnList, out var candidatePositionOnList);

                    var candidateOccurences = 1;

                    if (!sourceConfig.listEmpty)
                        candidateOccurences = sourceConfig.candidatePosition.Count(x =>
                            x.candidateIdentification.Equals(candidateListConfiguration.candidateIdentification));

                    foreach (var choiceCode in candidateListType.choiceCode)
                    {
                        listCandidatesChoiceCodes.Add(new namedCodeType()
                        {
                            codeDesignation = "choiceCode",
                            codeValue = choiceCode
                        });
                    }

                    candidatePositions.Add(new electionInformationTypeListCandidatePosition()
                    {
                        CandidateIdentification = candidateListConfiguration.candidateIdentification,
                        CandidateReferenceOnPosition = candidateListConfiguration.candidateReferenceOnPosition,
                        PositionOnList = candidatePositionOnList,
                        occurences = candidateOccurences,
                        individualCandidateVerificationCode = listCandidatesChoiceCodes.ToArray()
                    });
                }

                lists.Add(new electionInformationTypeList()
                {
                    IsEmptyList = sourceConfig.listEmpty,
                    ListDescriptionInformation = ListDescriptionInformation.Create(listDescriptionList),
                    ListIdentification = listSource.listIdentification,
                    ListIndentureNumber = sourceConfig.listIndentureNumber,
                    ListOrderOfPrecedence = listOrderOfPrecedence,
                    individualListVerificationCodes = new[]
                        {new namedCodeType() {codeDesignation = "choiceCode", codeValue = listSource.choiceCode}},
                    ListUnionBallotText = listUnionDescription,
                    candidatePosition = candidatePositions.ToArray(),
                });
            }

            election.list = lists.ToArray();
        }

        electionGroup.electionInformation = new[] { election };

        return electionGroup;
    }

    private static DomainOfInfluenceTypeType GetDomainOfInfluenceTypeFromString(string type)
    {
        var returnValue = default(DomainOfInfluenceTypeType);
        switch (type)
        {
            case "CH":
                returnValue = DomainOfInfluenceTypeType.CH;
                break;
            case "CT":
                returnValue = DomainOfInfluenceTypeType.CT;
                break;
            case "BZ":
                returnValue = DomainOfInfluenceTypeType.BZ;
                break;
            case "MU":
                returnValue = DomainOfInfluenceTypeType.MU;
                break;
            case "SC":
                returnValue = DomainOfInfluenceTypeType.SC;
                break;
            case "KI":
                returnValue = DomainOfInfluenceTypeType.KI;
                break;
            case "OG":
                returnValue = DomainOfInfluenceTypeType.OG;
                break;
            case "KO":
                returnValue = DomainOfInfluenceTypeType.KO;
                break;
            case "SK":
                returnValue = DomainOfInfluenceTypeType.SK;
                break;
            case "AN":
                returnValue = DomainOfInfluenceTypeType.AN;
                break;
        }

        return returnValue;
    }
}
