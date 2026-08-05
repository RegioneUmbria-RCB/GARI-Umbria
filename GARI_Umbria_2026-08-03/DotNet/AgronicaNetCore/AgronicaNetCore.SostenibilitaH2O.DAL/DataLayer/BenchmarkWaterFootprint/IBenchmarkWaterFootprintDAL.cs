using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.BenchmarkWaterFootprint
{
    /// <summary>
    /// Contratto per il data access della tabella <c>waterfootprint</c>
    /// e per la trascodifica varietà interna GIAS → codice FAOSTAT tramite <c>CAC_Codifica_Veg_Cod</c>.
    /// Riferimento spec: DS06-BL LookupBenchmarkWaterFootprint.
    /// </summary>
    public interface IBenchmarkWaterFootprintDAL
    {
        /// <summary>
        /// Converte un codice paese ISO Alpha-2 (2 caratteri) nel corrispondente codice
        /// ISO Alpha-3 (3 caratteri) tramite la tabella
        /// <c>ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166</c>.
        /// </summary>
        /// <param name="alpha2">Codice paese ISO Alpha-2 (es. <c>IT</c>).</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// Codice ISO Alpha-3 (es. <c>ITA</c>), oppure <see langword="null"/> se non trovato.
        /// </returns>
        Task<string?> GetIsoAlpha3FromAlpha2Async(
            string alpha2,
            AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Recupera il codice FAOSTAT (<c>veg_cod_coltiva</c>) corrispondente al codice
        /// varietà interno GIAS (<c>veg_cod_gias</c>) dalla tabella <c>CAC_Codifica_Veg_Cod</c>.
        /// </summary>
        /// <param name="vegCodGias">Codice varietà interno GIAS (Profitosan).</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// Codice FAOSTAT come stringa, oppure <see langword="null"/> se non trovato.
        /// </returns>
        Task<string?> GetProductCodeFaostatAsync(
            int vegCodGias,
            AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Recupera il valore Green-Blue Water Footprint (m³/t) dalla tabella
        /// <c>waterfootprint</c> per la combinazione (productCodeFaostat,
        /// paeseIsoAlpha3, wf_type='Green+Blue').
        /// <para>
        /// La ricerca usa prima la regione fornita; se non trova risultato (o la regione
        /// è <see langword="null"/>), ritenta con <c>region_code = 'CNTRY-average'</c>.
        /// </para>
        /// </summary>
        /// <param name="productCodeFaostat">Codice FAOSTAT della coltura.</param>
        /// <param name="paeseIsoAlpha3">Codice paese ISO Alpha-3 (es. <c>ITA</c>).</param>
        /// <param name="regionCode">
        /// Codice regione opzionale. Se <see langword="null"/> viene usato direttamente
        /// <c>CNTRY-average</c>.
        /// </param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// Valore WF in m³/t, oppure <see langword="null"/> se nessuna riga trovata
        /// neanche con CNTRY-average.
        /// </returns>
        Task<decimal?> GetWaterFootprintAsync(
            string productCodeFaostat,
            string paeseIsoAlpha3,
            string? regionCode,
            AgronicaCoreParametriServer objParametriServer);
    }
}
