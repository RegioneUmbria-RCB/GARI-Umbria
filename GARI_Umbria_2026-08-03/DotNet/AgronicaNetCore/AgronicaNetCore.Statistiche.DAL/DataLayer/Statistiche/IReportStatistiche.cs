using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System.Data;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Statistiche.DAL.DataLayer.Statistiche
{
    public interface IReportStatistiche
    {
        Task<bool> IsSemaforoVerde(string? pivaSuperuser, int? jobId, AgronicaCoreParametri objParametriInterscambio);

        Task<bool> ImpostaBloccoJobPerSemaforo(string? pivaSuperUser, int? jobId, DateTime? avvio, 
            AgronicaCoreParametri objParametriInterscambio);

        Task<bool> ImpostaSbloccoJobPerSemaforo(string? pivaSuperUser, int? jobId, AgronicaCoreParametri agronicaCoreParametri);

        Task<DataTable?> LeggiDettagliAsync(enum_FiltroDateStatisticheUtilizzo filtroPerDataCompetenzaOrDataRegistrazione,
            DateTime dataCompetenzaInizio, DateTime dataCompetenzaFine, string? filtroUsername, string? filtroPiva,
            AgronicaCoreParametriServer objParametriServer);

        Task<bool?> BulkInsertAsync(string tabella, DataTable? dettagliOperazioni, AgronicaCoreParametri objParametriInterscambio);
        Task<bool?> CopyFromSwapTableAsync(object swapTable, object targetTable, AgronicaCoreParametri objParametriInterscambio);

        Task<bool> ClearSwapTableAsync(object swapTable, AgronicaCoreParametri objParametriInterscambio);
    }
}
