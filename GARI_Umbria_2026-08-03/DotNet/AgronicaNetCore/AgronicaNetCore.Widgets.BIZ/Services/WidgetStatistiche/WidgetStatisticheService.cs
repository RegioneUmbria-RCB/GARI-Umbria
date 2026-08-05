using AgronicaNetCore.Base.Models;
using System.Data;
using AgronicaCoreDTOStd.InData.Widgets;
using AgronicaNetCore.Widgets.DAL.DataLayer.WidgetsStatistiche.WidgetsStatistiche;
using Microsoft.Extensions.DependencyInjection;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Widgets.BIZ.Resources;

namespace AgronicaNetCore.Widgets.BIZ.Services.WidgetStatistiche
{
    public class WidgetStatisticheService : BaseServiceWidgetsBIZ, IWidgetStatisticheService
    {
        private readonly IUtentiImpostazioni _utentiImpostazioni;
        private readonly IWidgetsStatistiche _widgetsStatistiche;

        public WidgetStatisticheService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _utentiImpostazioni = _serviceProvider.GetRequiredService<IUtentiImpostazioni>();
            _widgetsStatistiche = _serviceProvider.GetRequiredService<IWidgetsStatistiche>();
        }


        public async Task<DataTable?> GetCountriesAsync(int year, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable? res = null;
            try
            {
                var widgetDAL = _serviceProvider.GetRequiredService<IWidgetsStatistiche>();
                res = await widgetDAL.GetCountriesAsync(year, objParametriServer);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }
            return res;
        }

        public async Task<DataTable?> GetGeneralStatisticsAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable? res = null;
            try
            {
                var widgetDAL = _serviceProvider.GetRequiredService<IWidgetsStatistiche>();
                res = await widgetDAL.GetGeneralStatisticsAsync(input.Year, input.Country, objParametriServer);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }
            return res;
        }
        public async Task<DataTable?> GetMappedFarmersAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable? res = null;
            try
            {
                var widgetDAL = _serviceProvider.GetRequiredService<IWidgetsStatistiche>();
                res = await widgetDAL.GetMappedFarmersAsync(input.Year, input.Country, objParametriServer);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }
            return res;
        }
        public async Task<DataTable?> GetMovedMappedFarmersPriorWeekAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable? res = null;
            try
            {
                var widgetDAL = _serviceProvider.GetRequiredService<IWidgetsStatistiche>();
                res = await widgetDAL.GetMovedMappedFarmersPriorWeekAsync(input.Year, input.Country, objParametriServer);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }
            return res;
        }
        public async Task<DataTable?> GetMovedMappedFarmersCampaignBeginAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            DataTable? res = null;
            try
            {
                var dataInizioEFine = await _utentiImpostazioni.CropYearAsync(DateTime.Now, objParametriUtenti, objParametriServer);

                res = await _widgetsStatistiche.GetMovedMappedFarmersCampaignBeginAsync(input.Year, input.Country, dataInizioEFine.DataInizio, dataInizioEFine.DataFine, objParametriServer);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }
            return res;
        }
        public async Task<DataTable?> GetCropMapAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable? res = null;
            try
            {
                var widgetDAL = _serviceProvider.GetRequiredService<IWidgetsStatistiche>();
                res = await widgetDAL.GetCropMapAsync(input.Year, input.Country, objParametriServer);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }
            return res;
        }
        public async Task<DataTable?> GetFarmersHarvestSowingDataAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable? res = null;
            try
            {
                var widgetDAL = _serviceProvider.GetRequiredService<IWidgetsStatistiche>();
                res = await widgetDAL.GetFarmersHarvestSowingDataAsync(input.Year, input.Country, objParametriServer);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }
            return res;
        }
        public async Task<DataTable?> GetPlotsHarvestSowingDataAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable? res = null;
            try
            {
                var widgetDAL = _serviceProvider.GetRequiredService<IWidgetsStatistiche>();
                res = await widgetDAL.GetPlotsHarvestSowingDataAsync(input.Year, input.Country, objParametriServer);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }
            return res;
        }
        public async Task<DataTable?> GetTargetHAAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable? res = null;
            try
            {
                var widgetDAL = _serviceProvider.GetRequiredService<IWidgetsStatistiche>();
                res = await widgetDAL.GetTargetHAAsync(input.Year, input.Country, objParametriServer);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }
            return res;
        }
        public async Task<DataTable?> GetFarmerxRegionxRangeAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable? res = null;
            try
            {
                var widgetDAL = _serviceProvider.GetRequiredService<IWidgetsStatistiche>();
                res = await widgetDAL.GetFarmerxRegionxRangeAsync(input.Year, input.Country, objParametriServer);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }
            return res;
        }
    }
}
