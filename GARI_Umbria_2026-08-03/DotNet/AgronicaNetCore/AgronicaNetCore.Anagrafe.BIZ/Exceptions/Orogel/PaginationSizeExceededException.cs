namespace AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel
{
    /// <summary>
    /// DS04-BL §Eccezioni: Thrown when <c>pageSize</c> exceeds the maximum allowed value (1000)
    /// despite prior normalization by DS03-BL. This is a defensive re-validation.
    /// Maps to HTTP 400 Bad Request.
    /// </summary>
    public class PaginationSizeExceededException : Exception
    {
        /// <summary>The pageSize value that exceeded the maximum.</summary>
        public int PageSize { get; }

        /// <param name="pageSize">The pageSize value that exceeded the maximum of 1000.</param>
        public PaginationSizeExceededException(int pageSize)
            : base($"Il valore pageSize '{pageSize}' supera il limite massimo consentito di 1000.")
        {
            PageSize = pageSize;
        }
    }
}
