namespace TranscriptAnalyzer.Domain.Enums;

public enum AuthorizationStatus
{
    Draft = 0,
    PendingSignature = 1,
    Signed = 2,
    SubmittedToCaf = 3,
    Active = 4,
    Expired = 5,
    Revoked = 6
}
