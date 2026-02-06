namespace TranscriptAnalyzer.Domain.Enums;

public enum AuditAction
{
    Create = 0,
    Read = 1,
    Update = 2,
    Delete = 3,
    Login = 4,
    Logout = 5,
    LoginFailed = 6,
    SignatureRequested = 7,
    SignatureSigned = 8,
    TranscriptUploaded = 9,
    TranscriptDownloaded = 10,
    DataExported = 11,
    DataDeleted = 12
}
