namespace AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel
{
    /// <summary>
    /// DS05-BL §Eccezioni: Thrown when the SQL query against the <c>Imprese</c> table
    /// fails during execution. Maps to HTTP 500 Internal Server Error.
    /// </summary>
    public class QueryExecutionException : Exception
    {
        /// <summary>
        /// DS05-BL §Eccezioni QueryExecutionException: Creates a new instance wrapping the
        /// underlying query execution failure.
        /// </summary>
        /// <param name="message">Human-readable description of the query failure.</param>
        /// <param name="innerException">The underlying exception that caused the failure.</param>
        public QueryExecutionException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }
}
