namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when the semaphore state query exceeds the maximum allowed execution time of 20 ms.
/// Ref: DS11-BL – Eccezioni: SemaphoreQueryTimeoutException.
/// </summary>
public class SemaphoreQueryTimeoutException : Exception
{
    public SemaphoreQueryTimeoutException(string message) : base(message) { }
    public SemaphoreQueryTimeoutException(string message, Exception innerException) : base(message, innerException) { }
}
