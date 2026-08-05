namespace AgronicaNetCore.RischiMeteo.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando un campo obbligatorio (es. <c>codiceSpecie</c>)
    /// non è disponibile per un Esercizio. Il payload per l'Esercizio viene saltato.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Tolleranza dati mancanti,
    /// MissingMandatoryFieldException.
    /// </summary>
    public class MissingMandatoryFieldException : Exception
    {
        /// <summary>Nome del campo obbligatorio mancante.</summary>
        public string Campo { get; }

        /// <summary>Identificativo dell'Esercizio per cui il campo è mancante.</summary>
        public string IdEsercizio { get; }

        public MissingMandatoryFieldException(string campo, string idEsercizio)
            : base($"Campo obbligatorio '{campo}' non disponibile per l'Esercizio '{idEsercizio}'. Payload saltato.")
        {
            Campo = campo;
            IdEsercizio = idEsercizio;
        }
    }
}
