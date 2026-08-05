namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when visibility filters or permissions cannot be read from one of the four
/// visibility/permission sources (setup system or IImpostazioniAppService).
/// Ref: DS07-BL – Eccezioni: SetupSystemUnavailableException.
/// </summary>
public class SetupSystemUnavailableException : Exception
{
    public SetupSystemUnavailableException(string message) : base(message) { }

    public SetupSystemUnavailableException(string message, Exception innerException) : base(message, innerException) { }
}
