namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when one of the synchronisation log tables required by DS02 cannot be accessed.
/// Ref: DS02-BL – Eccezioni: LogAccessException.
/// </summary>
public class LogAccessException : Exception
{
    public LogAccessException(string message) : base(message) { }

    public LogAccessException(string message, Exception innerException)
        : base(message, innerException) { }
}