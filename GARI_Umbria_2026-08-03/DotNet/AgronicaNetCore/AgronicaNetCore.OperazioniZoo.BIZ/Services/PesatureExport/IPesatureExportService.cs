using AgronicaCoreDTOStd.InData.Zoo;
using AgronicaNetCore.Base.Models;
using OutData.Zoo;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesatureExport
{
    /// <summary>
    /// Servizio BIZ per la generazione del file di export delle pesature (curva di accrescimento).
    /// Implementa DS07-API: GET /api/v1/animals/weighing-curves/export.
    /// Supporta i formati CSV e XLS in modalità detailed e aggregated.
    /// Vedere DS07-API, sezione "Specifiche Tecniche" e "Roadmap Implementazione".
    /// </summary>
    public interface IPesatureExportService
    {
        /// <summary>
        /// Genera il file di export in memoria nel formato richiesto.
        /// Restituisce i byte del file, il content-type e il nome file proposto.
        /// Vedere DS07-API, sezione "Risposte / 200".
        /// </summary>
        Task<PesatureExportFileResult> ExportAsync(
            PesatureExportQueryParams queryParams,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
