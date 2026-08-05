namespace AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel
{
    /// <summary>
    /// DS04-BL §Eccezioni: Thrown when <c>nextKey</c> has an invalid format — specifically when
    /// the number of <c>§</c>-separated segments does not match the expected number of keyset
    /// ordering columns for the current API. Maps to HTTP 400 Bad Request.
    /// </summary>
    public class InvalidNextKeyException : Exception
    {
        /// <summary>The raw nextKey value received from the client.</summary>
        public string NextKey { get; }

        /// <summary>The number of segments expected (one per keyset column).</summary>
        public int ExpectedSegments { get; }

        /// <summary>The actual number of segments found after splitting by <c>§</c>.</summary>
        public int ActualSegments { get; }

        /// <param name="nextKey">The raw cursor value received from the client.</param>
        /// <param name="expectedSegments">Number of keyset columns defined for this API.</param>
        /// <param name="actualSegments">Number of segments found in the cursor.</param>
        public InvalidNextKeyException(string nextKey, int expectedSegments, int actualSegments)
            : base($"Il cursore nextKey '{nextKey}' non è valido: attesi {expectedSegments} segmenti separati da '§', trovati {actualSegments}.")
        {
            NextKey = nextKey;
            ExpectedSegments = expectedSegments;
            ActualSegments = actualSegments;
        }
    }
}
