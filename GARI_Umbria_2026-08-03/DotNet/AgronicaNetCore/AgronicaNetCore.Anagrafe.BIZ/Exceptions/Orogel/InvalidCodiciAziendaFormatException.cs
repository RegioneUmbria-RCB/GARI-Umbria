namespace AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel
{
    /// <summary>
    /// DS03-BL §Eccezioni: Thrown when an element in <c>codiciAzienda</c> fails format validation
    /// (must be alphanumeric, length 1–20). Maps to HTTP 400 Bad Request.
    /// </summary>
    public class InvalidCodiciAziendaFormatException : Exception
    {
        /// <summary>The specific codice azienda element that failed validation.</summary>
        public string CodiceNonValido { get; }

        /// <summary>
        /// DS03-BL §Regole codiciAzienda: Creates a new instance for an element that failed format validation.
        /// </summary>
        /// <param name="codiceNonValido">The element that failed format validation.</param>
        public InvalidCodiciAziendaFormatException(string codiceNonValido)
            : base($"Il codice azienda '{codiceNonValido}' non è valido: deve essere alfanumerico con lunghezza compresa tra 1 e 20 caratteri.")
        {
            CodiceNonValido = codiceNonValido;
        }
    }
}
