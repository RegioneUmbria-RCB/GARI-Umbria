namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when an entity contains a property type not serializable to JSON.
/// Ref: DS02-BL – Eccezioni: InvalidDataTypeException.
/// </summary>
public class InvalidDataTypeException : Exception
{
    public InvalidDataTypeException(string message) : base(message) { }

    public InvalidDataTypeException(string message, Exception innerException) : base(message, innerException) { }
}
