namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when an operation is attempted on the JSON buffer in an invalid state
/// (e.g., adding an entry after rollback).
/// Ref: DS04-BL – Eccezioni: CollectionStateException.
/// </summary>
public class CollectionStateException : Exception
{
    public CollectionStateException(string message) : base(message) { }

    public CollectionStateException(string message, Exception innerException) : base(message, innerException) { }
}
