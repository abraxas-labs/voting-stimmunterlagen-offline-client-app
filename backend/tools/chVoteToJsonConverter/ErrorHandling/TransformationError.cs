namespace chVoteToJsonConverter.ErrorHandling;

public class TransformationError
{
    public TransformationError(string errorCode, string errorText, string relatedId)
    {
        ErrorCode = errorCode;
        ErrorText = errorText;
        RelatedId = relatedId;
    }


    public string ErrorCode { get; }

    public string ErrorText { get; }

    public string RelatedId { get; }
}

public static class TransformationErrorCode
{
    public static string VoterNotFound => "VOTER_NOT_FOUND";
    public static string CandidateNotFound => "CANDIDATE_NOT_FOUND";
    public static string ListNotFound => "LIST_NOT_FOUND";
    public static string ElectionNotFound => "ELECTION_NOT_FOUND";
    public static string VoteNotFoud => "VOTE_NOT_FOUND";
    public static string ConfigNotFound => "CONFIG_NOT_FOUND";
    public static string ContestNotFound => "CONTEST_NOT_FOUND";
    public static string AuthorizationNotFound => "AUTHORIZATION_NOT_FOUND";
    public static string PrintingNotFoundInZip => "PRINTING_NOT_FOUND_IN_ZIP";
    public static string QuestionNotFound => "QUESTION_NOT_FOUND";
    public static string AnswereNotFound => "ANSWERE_NOT_FOUND";
    public static string ContestDuplicates => "CONTEST_DUPLICATES";
    public static string CountryNotFound => "COUNTRY_NOT_FOUND";
}
