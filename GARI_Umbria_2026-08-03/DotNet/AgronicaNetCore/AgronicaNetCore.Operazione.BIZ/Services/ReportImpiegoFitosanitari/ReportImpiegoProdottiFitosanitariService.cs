using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Operazione.BIZ.Resources;
using AgronicaNetCore.Operazione.DAL.DataLayer.ReportImpiegoProdottiFitosanitari;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using InData.QuadernoDiCampagna;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OutData.Kendo;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Operazione.BIZ.Services.ReportImpiegoFitosanitari
{
    public class ReportImpiegoProdottiFitosanitariService : BaseServiceOperazioneBIZ, IReportImpiegoProdottiFitosanitariService
    {
        private readonly IReportImpiegoProdottiFitosanitari _reportImpiegoProdottiFitosanitariDAL;
        private readonly IUtentiVisibilitaAppoggio _utentiVisibilitaAppoggio;
        public ReportImpiegoProdottiFitosanitariService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _reportImpiegoProdottiFitosanitariDAL = provider.GetRequiredService<IReportImpiegoProdottiFitosanitari>();
            _utentiVisibilitaAppoggio = provider.GetRequiredService<IUtentiVisibilitaAppoggio>();
        }

        public async Task<ResultAndKendoColumns> GetReportImpiegoFitosanitariAsync(ReportImpiegoProdottiFitosanitari_IN reportImpiegoProdottiFitosanitariIn, AgronicaCoreParametriServer objParametriServer)
        {
            ResultAndKendoColumns result;
            try
            {
                var dtImpreseVisibili = await _utentiVisibilitaAppoggio.ReadAsync((int)Enum_TipoEntita.Impresa, objParametriServer);
                var visibilitaLimitataImprese = dtImpreseVisibili is not null && dtImpreseVisibili.Rows.Count > 0;

                result = await _reportImpiegoProdottiFitosanitariDAL.GetReportImpiegoFitosanitariAsync(reportImpiegoProdottiFitosanitariIn, visibilitaLimitataImprese, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return result;
        }
    }
}
