namespace TranscriptAnalyzer.Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public string? ErrorCode { get; }

    protected Result(bool isSuccess, string? error = null, string? errorCode = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result Success() => new(true);
    public static Result Failure(string error, string? errorCode = null) => new(false, error, errorCode);
    public static Result<T> Success<T>(T value) => new(value, true);
    public static Result<T> Failure<T>(string error, string? errorCode = null) => new(default, false, error, errorCode);

    public static Result NotFound(string message = "Resource not found") => Failure(message, "NOT_FOUND");
    public static Result<T> NotFound<T>(string message = "Resource not found") => Failure<T>(message, "NOT_FOUND");

    public static Result Forbidden(string message = "Access denied") => Failure(message, "FORBIDDEN");
    public static Result<T> Forbidden<T>(string message = "Access denied") => Failure<T>(message, "FORBIDDEN");

    public static Result ValidationError(string message) => Failure(message, "VALIDATION_ERROR");
    public static Result<T> ValidationError<T>(string message) => Failure<T>(message, "VALIDATION_ERROR");

    public static Result Conflict(string message) => Failure(message, "CONFLICT");
    public static Result<T> Conflict<T>(string message) => Failure<T>(message, "CONFLICT");
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected internal Result(T? value, bool isSuccess, string? error = null, string? errorCode = null)
        : base(isSuccess, error, errorCode)
    {
        Value = value;
    }

    public static implicit operator Result<T>(T value) => Success(value);
}
