namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when the validation of user parameters exceeds the maximum allowed execution time of 100 ms.
/// Ref: DS07-BL – Eccezioni: ValidationTimeoutException.
/// </summary>
public class ValidationTimeoutException : Exception
{
    public ValidationTimeoutException(string message) : base(message) { }

    public ValidationTimeoutException(string message, Exception innerException) : base(message, innerException) { }
}
