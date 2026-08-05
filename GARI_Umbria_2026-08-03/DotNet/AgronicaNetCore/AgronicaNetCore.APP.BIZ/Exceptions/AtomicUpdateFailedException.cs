namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when one or more UPDATE statements fail during the atomic transaction and
/// a full rollback has been performed.
/// Ref: DS05-BL – Eccezioni: AtomicUpdateFailedException.
/// </summary>
public class AtomicUpdateFailedException : Exception
{
    public AtomicUpdateFailedException(string message) : base(message) { }

    public AtomicUpdateFailedException(string message, Exception innerException) : base(message, innerException) { }
}
