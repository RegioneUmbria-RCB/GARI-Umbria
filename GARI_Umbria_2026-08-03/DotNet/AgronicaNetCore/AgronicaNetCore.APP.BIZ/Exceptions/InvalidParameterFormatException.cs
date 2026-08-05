namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when the API call parameters received from the client are not valid JSON.
/// Ref: DS07-BL – Eccezioni: InvalidParameterFormatException.
/// </summary>
public class InvalidParameterFormatException : Exception
{
    public InvalidParameterFormatException(string message) : base(message) { }

    public InvalidParameterFormatException(string message, Exception innerException) : base(message, innerException) { }
}
