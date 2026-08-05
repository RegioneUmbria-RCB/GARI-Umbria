namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when reading from <c>app_param_utente_daticomuni_web2app</c> or from one of the
/// visibility filter sources fails at the data-access layer.
/// Ref: DS07-BL – Eccezioni: DataAccessException.
/// </summary>
public class DataAccessException : Exception
{
    public DataAccessException(string message) : base(message) { }

    public DataAccessException(string message, Exception innerException) : base(message, innerException) { }
}
