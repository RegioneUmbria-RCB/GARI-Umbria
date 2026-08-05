namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when the client-supplied timestamp is in the future
/// (greater than server UTC NOW() + 1 minute) or has an invalid format.
/// Ref: DS08-BL – Eccezioni: InvalidTimestampException.
/// Ref: DS02-BL – Eccezioni: InvalidTimestampException.
/// </summary>
public class InvalidTimestampException : Exception
{
    public InvalidTimestampException(string message) : base(message) { }

    public InvalidTimestampException(string message, Exception innerException) : base(message, innerException) { }
}
