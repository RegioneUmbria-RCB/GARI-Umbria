namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.RecuperoPayloadM4Token
{
    /// <summary>
    /// Singola riga della tabella "Token Generabili" (DS09-BL output).
    /// Espone i metadati di un'invocazione M4 in modalità "Aziendale" disponibile
    /// per la predisposizione del Token di Sostenibilità CO₂.
    /// </summary>
    /// <remarks>
    /// Riferimento spec: DS09-BL RecuperoPayloadM4DaLookupToken —
    /// endpoint <c>GET v1/sostenibilita/token-generabili</c>.
    /// </remarks>
    public class TokenGenerabileItem
    {
        /// <summary>UUID univoco dell'invocazione M4.</summary>
        public string IdInvocazione { get; set; } = string.Empty;

        /// <summary>Timestamp dell'invocazione M4 (ISO 8601 UTC).</summary>
        public DateTime DataInvocazione { get; set; }

        /// <summary>P.IVA dell'azienda.</summary>
        public string PivaAzienda { get; set; } = string.Empty;

        /// <summary>Ragione sociale dell'azienda.</summary>
        public string? RagSocAzienda { get; set; }

        /// <summary>
        /// Variazione di carbonio organico nel suolo con imputazione biogenica (Kg CO₂ eq.).
        /// Estratto da <c>json_risposta.aziende[id_azienda].var_soc_soil_biogenic_carbon</c>.
        /// </summary>
        public decimal? VarSocSoilBiogenicCarbon { get; set; }

        /// <summary>
        /// Numero di appezzamenti dell'azienda inclusi nel calcolo.
        /// Estratto come <c>count(json_risposta.aziende[id_azienda].appezzamenti[])</c>.
        /// </summary>
        public int NumeroAppezzamenti { get; set; }

        /// <summary>
        /// Payload JSON grezzo della risposta M4 così come persistito in
        /// <c>Lookup_Sost_CO2_Aziendale_Payload.json_risposta</c>.
        /// </summary>
        public string? JsonRisposta { get; set; }
    }
}
