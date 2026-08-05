namespace AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel
{
    /// <summary>
    /// DS03-BL §Eccezioni: Thrown when <c>anno</c> fails the cohesion re-validation confirming
    /// DS01-BL checks. Maps to HTTP 400 Bad Request.
    /// </summary>
    public class InvalidAnnoCohesionException : Exception
    {
        /// <summary>The anno value that failed cohesion validation.</summary>
        public int Anno { get; }

        /// <summary>
        /// DS03-BL §Regole anno: Creates a new instance describing the cohesion failure.
        /// </summary>
        /// <param name="anno">The anno value that failed validation.</param>
        /// <param name="motivo">Human-readable explanation of the cohesion failure.</param>
        public InvalidAnnoCohesionException(int anno, string motivo)
            : base($"L'anno '{anno}' non supera la verifica di coerenza: {motivo}.")
        {
            Anno = anno;
        }
    }
}
