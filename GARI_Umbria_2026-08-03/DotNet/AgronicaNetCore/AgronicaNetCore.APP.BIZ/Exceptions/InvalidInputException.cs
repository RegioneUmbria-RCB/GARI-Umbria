namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when the DS03 inputs are incomplete or internally inconsistent.
/// Ref: DS03-BL – Eccezioni: InvalidInputException.
/// </summary>
public class InvalidInputException : Exception
{
    public InvalidInputException(string message) : base(message) { }

    public InvalidInputException(string message, Exception innerException)
        : base(message, innerException) { }
}