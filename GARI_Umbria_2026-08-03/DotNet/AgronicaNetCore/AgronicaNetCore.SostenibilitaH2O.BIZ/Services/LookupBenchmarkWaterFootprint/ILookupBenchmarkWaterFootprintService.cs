using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.LookupBenchmarkWaterFootprint
{
    /// <summary>
    /// Contratto per il recupero del valore Green-Blue Water Footprint (m³/t)
    /// dalla tabella <c>waterfootprint</c>.
    /// <para>
    /// Il servizio esegue la trascodifica dal codice varietà interno GIAS
    /// (<c>veg_cod_gias</c>) al codice FAOSTAT (<c>veg_cod_coltiva</c>) tramite
    /// la tabella <c>CAC_Codifica_Veg_Cod</c>, quindi interroga
    /// <c>waterfootprint</c> con filtri su paese (ISO Alpha-3)
    /// e <c>wf_type = 'Green+Blue'</c>.
    /// </para>
    /// <para>
    /// La ricerca usa prima la regione fornita; se non trovata (o regione assente)
    /// usa il fallback <c>region_code = 'CNTRY-average'</c>.
    /// L'assenza di risultato anche col fallback è bloccante.
    /// </para>
    /// Riferimento spec: DS06-BL LookupBenchmarkWaterFootprint.
    /// </summary>
    public interface ILookupBenchmarkWaterFootprintService
    {
        /// <summary>
        /// Recupera il valore Green-Blue Water Footprint (m³/t) per la combinazione
        /// (vegCodGias, paeseIsoAlpha3).
        /// </summary>
        /// <param name="vegCodGias">
        /// Codice varietà interno GIAS (Profitosan). Viene trascodificato in codice FAOSTAT prima
        /// della query su <c>waterfootprint</c>.
        /// </param>
        /// <param name="paeseIsoAlpha3">
        /// Codice paese ISO 3166-1 Alpha-3 (es. <c>ITA</c>). Deve essere esattamente 3 caratteri.
        /// </param>
        /// <param name="regionCode">
        /// Codice regione opzionale. Se <see langword="null"/>, o se non trovato, viene usato
        /// il fallback <c>CNTRY-average</c>.
        /// </param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>Valore WF in m³/t con precisione minima di 2 decimali.</returns>
        /// <exception cref="InvalidCountryCodeException">
        /// <paramref name="paeseIsoAlpha3"/> non è un codice ISO Alpha-3 valido (!=3 caratteri alphabetici).
        /// </exception>
        /// <exception cref="MalformedVarietaException">
        /// Nessuna riga trovata in <c>CAC_Codifica_Veg_Cod</c> per il <paramref name="vegCodGias"/> fornito.
        /// </exception>
        /// <exception cref="BenchmarkNotFoundException">
        /// Nessuna riga trovata in <c>waterfootprint</c> per la combinazione
        /// (paese, codice FAOSTAT, wf_type='Green+Blue') anche col fallback CNTRY-average.
        /// </exception>
        /// <exception cref="DatabaseQueryException">
        /// Errore durante l'interrogazione del database GIAS.
        /// </exception>
        Task<decimal> GetWaterFootprintAsync(
            int vegCodGias,
            string paeseIsoAlpha3,
            string? regionCode,
            AgronicaCoreParametriServer objParametriServer);
    }
}
