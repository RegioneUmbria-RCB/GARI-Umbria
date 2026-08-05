namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when DS05-BL cannot generate the SHA256-based ETag for the final response payload.
/// Ref: DS05-BL – Eccezioni: ETagGenerationException.
/// </summary>
public class ETagGenerationException : Exception
{
    public ETagGenerationException(string message) : base(message) { }

    public ETagGenerationException(string message, Exception innerException)
        : base(message, innerException) { }
}