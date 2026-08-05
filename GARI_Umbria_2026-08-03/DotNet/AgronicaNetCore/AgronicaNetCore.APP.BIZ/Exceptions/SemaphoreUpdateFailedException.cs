namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when the final UPDATE on the semaphore record in <c>app_semaforo_daticomuni_web2app</c> fails.
/// Ref: DS06-BL – Eccezioni: SemaphoreUpdateFailedException.
/// </summary>
public class SemaphoreUpdateFailedException : Exception
{
    public SemaphoreUpdateFailedException(string message) : base(message) { }
    public SemaphoreUpdateFailedException(string message, Exception innerException) : base(message, innerException) { }
}
