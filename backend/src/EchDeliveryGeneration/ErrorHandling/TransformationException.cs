using System;
using System.Collections.Generic;

namespace EchDeliveryGeneration.ErrorHandling;

public class TransformationException : Exception
{
    private readonly string _errorCode;
    private readonly string _relatedId;

    private readonly Dictionary<string, string> _errorTexts;

    public TransformationException(string errorCode, string relatedId, string? message = null, Exception? innerException = null) : base(message, innerException)
    {
        _errorCode = errorCode;
        _relatedId = relatedId;

        _errorTexts = new Dictionary<string, string>(new[] {
            new KeyValuePair<string, string>(TransformationErrorCode.CandidateNotFound, "Candidate with id {0} could not be found"),
            new KeyValuePair<string, string>(TransformationErrorCode.ElectionNotFound, "Election with id {0} could not be found"),
            new KeyValuePair<string, string>(TransformationErrorCode.ListNotFound, "List with id {0} could not be found"),
            new KeyValuePair<string, string>(TransformationErrorCode.VoteNotFoud, "Vote with id {0} could not be found"),
            new KeyValuePair<string, string>(TransformationErrorCode.VoterNotFound, "Voter with id {0} could not be found"),
            new KeyValuePair<string, string>(TransformationErrorCode.PrintingNotFoundInZip, "Printing for Municipality wit BFS {0} could not be found"),
            new KeyValuePair<string, string>(TransformationErrorCode.ConfigNotFound, "Config with id {0} could not be found"),
            new KeyValuePair<string, string>(TransformationErrorCode.QuestionNotFound, "Questions with id {0} could not be found"),
            new KeyValuePair<string, string>(TransformationErrorCode.ContestNotFound, "Contest with id {0} could not be found"),
            new KeyValuePair<string, string>(TransformationErrorCode.AnswereNotFound, "Answere with id {0} could not be found"),
            new KeyValuePair<string, string>(TransformationErrorCode.AuthorizationNotFound, "Authorization with id {0} could not be found"),
            new KeyValuePair<string, string>(TransformationErrorCode.ContestDuplicates, "Contest duplicates with id {0}"),
            new KeyValuePair<string, string>(TransformationErrorCode.CountryNotFound, "Country {0} could not be found"),
            new KeyValuePair<string, string>(TransformationErrorCode.VoterDuplicates, "Voter duplicates with id {0}"),
        });
    }

    public TransformationError GetError() => new(_errorCode, string.Format(_errorTexts[_errorCode], _relatedId), _relatedId);
}
