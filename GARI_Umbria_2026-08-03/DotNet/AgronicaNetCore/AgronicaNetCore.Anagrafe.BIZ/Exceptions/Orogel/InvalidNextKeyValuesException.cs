namespace AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel
{
    /// <summary>
    /// DS04-BL §Eccezioni: Thrown when a value deserialized from <c>nextKey</c> does not match
    /// the expected data type for the corresponding keyset column.
    /// Maps to HTTP 400 Bad Request.
    /// </summary>
    public class InvalidNextKeyValuesException : Exception
    {
        /// <summary>The raw nextKey value received from the client.</summary>
        public string NextKey { get; }

        /// <summary>The column whose value failed type parsing.</summary>
        public string NomeColonna { get; }

        /// <summary>The raw string value that could not be parsed.</summary>
        public string ValoreNonValido { get; }

        /// <summary>The expected data type name (e.g., "integer").</summary>
        public string TipoAtteso { get; }

        /// <param name="nextKey">The raw cursor value received from the client.</param>
        /// <param name="nomeColonna">The column whose value failed type parsing.</param>
        /// <param name="valoreNonValido">The raw string value that could not be parsed.</param>
        /// <param name="tipoAtteso">The expected data type name.</param>
        public InvalidNextKeyValuesException(string nextKey, string nomeColonna, string valoreNonValido, string tipoAtteso)
            : base($"Il valore '{valoreNonValido}' per la colonna '{nomeColonna}' nel cursore nextKey '{nextKey}' non è del tipo atteso '{tipoAtteso}'.")
        {
            NextKey = nextKey;
            NomeColonna = nomeColonna;
            ValoreNonValido = valoreNonValido;
            TipoAtteso = tipoAtteso;
        }
    }
}
