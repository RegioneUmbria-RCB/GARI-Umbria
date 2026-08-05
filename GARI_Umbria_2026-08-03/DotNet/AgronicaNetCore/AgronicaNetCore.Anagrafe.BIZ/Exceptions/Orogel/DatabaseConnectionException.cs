namespace AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel
{
    /// <summary>
    /// DS05-BL §Eccezioni: Thrown when a connection to the archive database cannot be established.
    /// Maps to HTTP 503 Service Unavailable.
    /// </summary>
    public class DatabaseConnectionException : Exception
    {
        /// <summary>
        /// DS05-BL §Eccezioni DatabaseConnectionException: Creates a new instance wrapping the
        /// underlying connectivity failure.
        /// </summary>
        /// <param name="message">Human-readable description of the connection failure.</param>
        /// <param name="innerException">The underlying exception that caused the failure.</param>
        public DatabaseConnectionException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }
}
