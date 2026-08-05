namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Singolo record di consumo carburante immesso dall'utente (output DS02-BL).
    /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale — Output <c>consumi_validati.carburanti[]</c>.
    /// </summary>
    public class CarburanteConsumo
    {
        /// <summary>Partita IVA dell'azienda a cui appartiene il consumo.</summary>
        public string Azienda { get; set; } = string.Empty;

        /// <summary>Tipo di carburante (es. Diesel, Benzina, GPL, Metano, Altro).</summary>
        public string TipoCarburante { get; set; } = string.Empty;

        /// <summary>Quantità consumata (deve essere &gt; 0).</summary>
        public decimal Quantita { get; set; }

        /// <summary>Unità di misura (litri, Kg, m³).</summary>
        public string UnitaMisura { get; set; } = "litri";
    }
}
