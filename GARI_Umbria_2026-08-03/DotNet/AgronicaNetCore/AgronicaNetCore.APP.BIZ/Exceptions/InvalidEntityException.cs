namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when an entity collection is null, empty, or contains null entries.
/// Ref: DS02-BL – Eccezioni: InvalidEntityException.
/// </summary>
public class InvalidEntityException : Exception
{
    public InvalidEntityException(string message) : base(message) { }

    public InvalidEntityException(string message, Exception innerException) : base(message, innerException) { }
}
