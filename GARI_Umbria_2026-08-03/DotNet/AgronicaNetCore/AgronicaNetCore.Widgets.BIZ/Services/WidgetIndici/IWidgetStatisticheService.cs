using AgronicaCoreDTOStd.InData.Widgets;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Widgets.BIZ.Services.WidgetIndici;

public interface IWidgetIndiciService
{
    Task<List<WidgetKPI>> GetWidgetKpiAsync(uint year, string piva, AgronicaCoreParametriServer objParametriServer);

    Task<WidgetIndiciProduttivitaGlobal> GetWidgetProduttivitaAsync(WidgetRequestIndiciProduttivita request,
        AgronicaCoreParametriServer objParametriServer);

    Task<List<int>> GetAvailableYearsIndiciProduttivitaAsync(string piva, AgronicaCoreParametriServer objParametriServer);
}