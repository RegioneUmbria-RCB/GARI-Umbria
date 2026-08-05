namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when a JSON parameter string exceeds the configured character-length limit.
/// Ref: DS10-BL – Eccezioni: ParameterSizeExceededException.
/// </summary>
public class ParameterSizeExceededException : Exception
{
    public ParameterSizeExceededException(string message) : base(message) { }

    public ParameterSizeExceededException(string message, Exception innerException) : base(message, innerException) { }
}
