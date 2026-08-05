using AgronicaNetCore.Base.Models;
using InData.QuadernoDiCampagna;
using OutData.Kendo;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.ReportImpiegoProdottiFitosanitari
{
    public interface IReportImpiegoProdottiFitosanitari
    {
        Task<ResultAndKendoColumns> GetReportImpiegoFitosanitariAsync(ReportImpiegoProdottiFitosanitari_IN reportImpiegoProdottiFitosanitariIn, bool visibilitaLimitataImprese, AgronicaCoreParametriServer objParametriServer);
    }
}
