namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Output della persistenza atomica chiavi+payload per la modalità "Per Azienda".
    /// Contiene gli identificativi delle righe inserite nelle due tabelle correlate.
    /// Riferimento spec: DS10-BL PersistenzaChiaviPayloadPerAzienda — Output.
    /// </summary>
    public class PersistenzaChiaviPayloadPerAziendaOutput
    {
        /// <summary>
        /// Identificativo surrogato della riga inserita in <c>Lookup_Sost_H20_Aziendale_Chiavi</c>.
        /// Riferimento spec: DS10-BL — "id_calcolo: bigint".
        /// </summary>
        public int IdCalcolo { get; set; }

        /// <summary>
        /// Identificativo surrogato della riga inserita in <c>Lookup_Sost_H20_Aziendale_Payload</c>.
        /// Riferimento spec: DS10-BL — "id_payload: bigint".
        /// </summary>
        public int IdPayload { get; set; }
    }
}
