namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Rappresenta un singolo appezzamento nell'array <c>appezzamenti[]</c> del payload M4.
    /// Contiene geometria (centroide WKT), superficie organica opzionale e lista impianti.
    /// Riferimento spec: DS04-BL AssemblyPayloadM4AppezzamentoImpianto — Regole 1-3, Output <c>appezzamenti[]</c>.
    /// </summary>
    public class AppezzamentoPayload
    {
        /// <summary>
        /// Identificativo univoco appezzamento: <c>PIVA|Sa_Cod|Appezza</c>.
        /// Carattere separatore <c>|</c> non escaped.
        /// Riferimento spec: DS04-BL Regola 1.
        /// </summary>
        public string id_appezzamento { get; set; } = string.Empty;

        /// <summary>
        /// Centroide dell'appezzamento in formato WKT EPSG:4326.
        /// Calcolato come media coordinate vertici del poligono GIS.
        /// Esempio: <c>"POINT(12.3456 41.2345)"</c>.
        /// Riferimento spec: DS04-BL Regola 2.
        /// </summary>
        public string centroide { get; set; } = string.Empty;

        /// <summary>
        /// Percentuale di sostanza organica nel suolo (parametro <c>SO</c> da tabella <c>Analisi_Terreno</c>).
        /// Campo opzionale: <c>null</c> se non disponibile → omesso dal payload serializzato.
        /// Riferimento spec: DS04-BL Regola 3.
        /// </summary>
        public decimal? perc_sostanza_organica { get; set; }

        /// <summary>
        /// Anno di impianto (anno di campagna selezionato dall'utente).
        /// Riferimento spec: DS04-BL mapping <c>anno_impianto</c> — Sorgente: Perimetro.
        /// </summary>
        public int anno_campagna { get; set; }

        /// <summary>
        /// Superficie dell'appezzamento in ettari.
        /// Campo opzionale: <c>null</c> se non disponibile → omesso dal payload serializzato.
        /// Riferimento spec: DS04-BL Regola 4.
        /// </summary>
        public decimal? area_ha { get; set; }

        /// <summary>
        /// Lista degli impianti/esercizi associati all'appezzamento.
        /// Deve contenere almeno un elemento (Regola 9).
        /// </summary>
        public List<ImpiantoPayload> impianti { get; set; } = new List<ImpiantoPayload>();
    }
}
