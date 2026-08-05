namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Singolo record di consumo energetico immesso dall'utente (output DS02-BL).
    /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale — Output <c>consumi_validati.energia[]</c>.
    /// </summary>
    public class EnergiaConsumo
    {
        /// <summary>Partita IVA dell'azienda a cui appartiene il consumo.</summary>
        public string Azienda { get; set; } = string.Empty;

        /// <summary>Data di inizio del periodo di consumo (ISO 8601).</summary>
        public DateTime DataInizio { get; set; }

        /// <summary>Data di fine del periodo di consumo (ISO 8601). Deve essere &gt;= <see cref="DataInizio"/>.</summary>
        public DateTime DataFine { get; set; }

        /// <summary>Consumo energetico in kWh (deve essere &gt; 0).</summary>
        public decimal ConsumoKwh { get; set; }

        /// <summary>Percentuale di energia da fonti rinnovabili (range 0–100).</summary>
        public decimal PercentualeRinnovabili { get; set; }
    }
}
