namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when a timestamp cannot be converted to the database-compatible precision/range
/// required by DS02.
/// Ref: DS02-BL – Eccezioni: TimestampConversionException.
/// </summary>
public class TimestampConversionException : Exception
{
    public TimestampConversionException(string message) : base(message) { }

    public TimestampConversionException(string message, Exception innerException)
        : base(message, innerException) { }
}