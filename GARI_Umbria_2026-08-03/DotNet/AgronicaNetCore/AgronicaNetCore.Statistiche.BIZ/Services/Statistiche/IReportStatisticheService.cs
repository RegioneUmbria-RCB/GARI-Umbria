using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Base.Models;
using OutData.Kendo;
using OutData.Varie;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Statistiche.BIZ.Services.Statistiche
{
    public interface IReportStatisticheService
    {
        Task<string?> ImportReportElencoSinteticoAsync(int jobId, DateTime? avvio, int filtroPerDataCompetenzaOrDataRegistrazione, 
            IntervalloTemporale intervalloOperazioniDiCampagna, IntervalloTemporale intervalloPratiche,
            string? username, AgronicaCoreParametriUtenti objParametriUtenti, 
            AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer);

        Task<objAllegato?> ExportReportElencoSinteticoAsync(int filtroPerDataCompetenzaOrDataRegistrazione,
            IntervalloTemporale intervalloOperazioniDiCampagna, IntervalloTemporale intervalloPratiche,
            bool dettagliQdC, string? username, string? piva, AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer);
    }
}
