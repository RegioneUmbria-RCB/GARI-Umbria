namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when the watchdog UPDATE to reset a stuck semaphore record fails.
/// Ref: DS12-BL – Eccezioni: ResetFailedException.
/// </summary>
public class ResetFailedException : Exception
{
    public ResetFailedException(string message) : base(message) { }
    public ResetFailedException(string message, Exception innerException) : base(message, innerException) { }
}
