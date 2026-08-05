using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Widgets.DAL.DataLayer.WidgetsStatistiche.WidgetsStatistiche
{
    public interface IWidgetsStatistiche
    {
        Task<DataTable?> GetCountriesAsync(int year, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetGeneralStatisticsAsync(int year, string country, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetMappedFarmersAsync(int year, string country, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetMovedMappedFarmersPriorWeekAsync(int year, string country, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetMovedMappedFarmersCampaignBeginAsync(int year, string country, DateTime campaignBegin, DateTime campaignEnd, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetCropMapAsync(int year, string country, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetFarmersHarvestSowingDataAsync(int year, string country, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetPlotsHarvestSowingDataAsync(int year, string country, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetTargetHAAsync(int year, string country, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetFarmerxRegionxRangeAsync(int year, string country, AgronicaCoreParametriServer objParametriServer);
    }
}