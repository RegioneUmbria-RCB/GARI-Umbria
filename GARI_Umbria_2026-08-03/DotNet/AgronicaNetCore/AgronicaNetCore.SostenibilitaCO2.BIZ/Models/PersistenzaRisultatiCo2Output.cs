namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Output della business logic <c>PersistenzaRisultatiM4Service</c> (DS08-BL).
    /// Descrive l'esito della persistenza su tabelle di lookup e le righe inserite.
    /// Riferimento spec: DS08-BL PersistenzaRisultatiM4LookupTable — Output.
    /// </summary>
    public class PersistenzaRisultatiCo2Output
    {
        /// <summary>Identificativo univoco dell'invocazione generato e persistito.</summary>
        public string IdInvocazione { get; set; }

        /// <summary><c>true</c> se la persistenza è avvenuta con successo.</summary>
        public bool PersistenzaEsito { get; set; }

        /// <summary>Nome della tabella chiavi popolata.</summary>
        public string TabellaChiavi { get; set; } = string.Empty;

        /// <summary>Nome della tabella payload popolata.</summary>
        public string TabellaPayload { get; set; } = string.Empty;

        /// <summary>Numero di righe inserite nella tabella chiavi.</summary>
        public int RigheInserite { get; set; }
    }
}
