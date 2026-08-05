using AgronicaCoreDTOStd.InData.ConfrontoCatasto;
using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgronicaCoreDTOStd.InData.Widgets;

namespace AgronicaNetCore.Widgets.BIZ.Services.WidgetStatistiche
{
    public interface IWidgetStatisticheService
    {
        Task<DataTable?> GetCountriesAsync(int year, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable?> GetGeneralStatisticsAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetMappedFarmersAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetMovedMappedFarmersPriorWeekAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetMovedMappedFarmersCampaignBeginAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti);
        Task<DataTable?> GetCropMapAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetFarmersHarvestSowingDataAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetPlotsHarvestSowingDataAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetTargetHAAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetFarmerxRegionxRangeAsync(Widget_Statistics_IN input, AgronicaCoreParametriServer objParametriServer);
    }
}
