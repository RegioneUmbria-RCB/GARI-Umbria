namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when the in-memory JSON buffer allocation exceeds the configured maximum threshold.
/// Ref: DS04-BL – Eccezioni: MemoryExceededException.
/// </summary>
public class MemoryExceededException : Exception
{
    public MemoryExceededException(string message) : base(message) { }

    public MemoryExceededException(string message, Exception innerException) : base(message, innerException) { }
}
