namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when the INSERT or UPDATE operation on
/// <c>app_param_utente_daticomuni_web2app</c> fails.
/// Ref: DS10-BL – Eccezioni: UpsertFailedException.
/// </summary>
public class UpsertFailedException : Exception
{
    public UpsertFailedException(string message) : base(message) { }

    public UpsertFailedException(string message, Exception innerException) : base(message, innerException) { }
}
