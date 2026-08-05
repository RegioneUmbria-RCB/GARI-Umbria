namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when a SQL deadlock persists after the maximum number of retries.
/// Ref: DS05-BL – Eccezioni: DeadlockException.
/// </summary>
public class DeadlockException : Exception
{
    public DeadlockException(string message) : base(message) { }

    public DeadlockException(string message, Exception innerException) : base(message, innerException) { }
}
