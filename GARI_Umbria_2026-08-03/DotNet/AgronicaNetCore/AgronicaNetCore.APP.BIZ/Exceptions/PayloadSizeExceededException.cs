namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when the aggregated JSON payload exceeds the configured maximum allowed size.
/// Ref: DS09-BL – Eccezioni: PayloadSizeExceededException.
/// </summary>
public class PayloadSizeExceededException : Exception
{
    public PayloadSizeExceededException(string message) : base(message) { }

    public PayloadSizeExceededException(string message, Exception innerException) : base(message, innerException) { }
}
