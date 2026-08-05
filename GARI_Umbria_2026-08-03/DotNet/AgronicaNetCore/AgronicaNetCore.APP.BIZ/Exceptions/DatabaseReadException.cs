namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when reading from <c>app_preparazione_daticomuni_web2app</c> fails during the
/// JSON comparison phase.
/// Ref: DS03-BL – Eccezioni: DatabaseReadException.
/// </summary>
public class DatabaseReadException : Exception
{
    public DatabaseReadException(string message) : base(message) { }
    public DatabaseReadException(string message, Exception innerException) : base(message, innerException) { }
}
