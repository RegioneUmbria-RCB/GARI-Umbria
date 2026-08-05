namespace AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel
{
    /// <summary>
    /// DS03-BL §Eccezioni: Thrown when <c>pageSize</c> is less than 1.
    /// Maps to HTTP 400 Bad Request.
    /// </summary>
    public class InvalidPageSizeException : Exception
    {
        /// <summary>The invalid pageSize value that caused the exception.</summary>
        public int PageSize { get; }

        /// <summary>
        /// DS03-BL §Regole pageSize: Creates a new instance for a pageSize value &lt; 1.
        /// </summary>
        /// <param name="pageSize">The invalid pageSize value received from the client.</param>
        public InvalidPageSizeException(int pageSize)
            : base($"Il valore '{pageSize}' non è un pageSize valido: deve essere un intero >= 1.")
        {
            PageSize = pageSize;
        }
    }
}
