namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when the DS02 unified log queries fail or exceed the allowed execution time.
/// Ref: DS02-BL – Eccezioni: DatabaseQueryException.
/// </summary>
public class DatabaseQueryException : Exception
{
    public DatabaseQueryException(string message) : base(message) { }

    public DatabaseQueryException(string message, Exception innerException)
        : base(message, innerException) { }
}