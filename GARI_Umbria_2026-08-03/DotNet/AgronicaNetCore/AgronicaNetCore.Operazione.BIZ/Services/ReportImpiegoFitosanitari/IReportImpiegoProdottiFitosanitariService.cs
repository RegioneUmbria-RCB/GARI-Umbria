using AgronicaNetCore.Base.Models;
using InData.QuadernoDiCampagna;
using OutData.Kendo;

namespace AgronicaNetCore.Operazione.BIZ.Services.ReportImpiegoFitosanitari
{
    public interface IReportImpiegoProdottiFitosanitariService
    {
        Task<ResultAndKendoColumns> GetReportImpiegoFitosanitariAsync(ReportImpiegoProdottiFitosanitari_IN reportImpiegoProdottiFitosanitariIn, AgronicaCoreParametriServer objParametriServer);
    }
}
