using AgronicaNetCore.Base.Models;
using System.Data;
using AgronicaNetCore.Widgets.DAL.DataLayer.WidgetsDocumentale.WidgetsDocumentale;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;
using Microsoft.Extensions.DependencyInjection;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Widgets.BIZ.Resources;

namespace AgronicaNetCore.Widgets.BIZ.Services.WidgetStatistiche
{
    public class WidgetDocumentaleService : BaseServiceWidgetsBIZ, IWidgetDocumentaleService
    {
        private readonly IUtentiVisibilitaAppoggio _utentiVisibilitaAppoggio;
        private readonly IUtentiImpostazioni _utentiImpostazioni;
        private readonly IWidgetsDocumentale _widgetsDocumentale;

        public WidgetDocumentaleService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _utentiVisibilitaAppoggio = _serviceProvider.GetRequiredService<IUtentiVisibilitaAppoggio>();
            _utentiImpostazioni = _serviceProvider.GetRequiredService<IUtentiImpostazioni>();
            _widgetsDocumentale = _serviceProvider.GetRequiredService<IWidgetsDocumentale>();
        }

        public async Task<DataTable?> GetDocumentRecapAsync(DateTime timeStart, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            DataTable? res = null;
            try
            {
                if (_widgetsDocumentale == null || _utentiVisibilitaAppoggio == null)
                    throw new Exception("Riferimento a servizi DAL per widget. impossibile proseguire");

                bool userVisibility = (await _utentiVisibilitaAppoggio.ReadAsync((int)Enum_TipoEntita.Impresa, objParametriServer)).Rows.Count > 0;

                DataTable dt = (await _utentiImpostazioni.ReadAsync((int)Enum_Impostazioni_Utenti.SUPERUSER_Documentale_GestioneWorkFlow, 2, objParametriUtenti, objParametriServer))!;
                bool useWorkflow = dt.Rows.Count > 0;

                res = await _widgetsDocumentale.GetDocumentRecapAsync(timeStart, useWorkflow, userVisibility, objParametriServer, objParametriUtenti);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return res;
        }
    }
}
