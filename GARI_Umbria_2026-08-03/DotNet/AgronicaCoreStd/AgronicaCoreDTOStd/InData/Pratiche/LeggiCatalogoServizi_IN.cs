namespace AgronicaCoreDTOStd.InData.Pratiche
{
    /// <summary>
    /// Parametri di input per il recupero del catalogo pratiche/servizi.
    /// DS10 – API: Endpoint Recupero Catalogo Pratiche.
    /// La validazione dei valori è delegata al controller, che restituisce RispostaStandard in caso di errore.
    /// </summary>
    public class LeggiCatalogoServizi_IN
    {
        /// <summary>Filtro testo su descrizione pratica (case-insensitive, substring match).</summary>
        public string Filter { get; set; }

        /// <summary>Ordinamento per codice servizio. Valori ammessi: ASC, DESC. Default: ASC.</summary>
        public string Sort { get; set; } = "ASC";

        /// <summary>Se true include anche le pratiche scadute (dataValiditaFine &lt; oggi). Default: false.</summary>
        public bool IncludeExpired { get; set; } = false;

        /// <summary>Numero di pagina (1-based). Default: 1.</summary>
        public int Page { get; set; } = 1;

        /// <summary>Numero di record per pagina. Default: 100. Massimo: 1000.</summary>
        public int PageSize { get; set; } = 100;
    }
}
