namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when the atomic UPDATE transaction exceeds the configured maximum duration.
/// Ref: DS05-BL – Eccezioni: TransactionTimeoutException.
/// </summary>
public class TransactionTimeoutException : Exception
{
    public TransactionTimeoutException(string message) : base(message) { }

    public TransactionTimeoutException(string message, Exception innerException) : base(message, innerException) { }
}
