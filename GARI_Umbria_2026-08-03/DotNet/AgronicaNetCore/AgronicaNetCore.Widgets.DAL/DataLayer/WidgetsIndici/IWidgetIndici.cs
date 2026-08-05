using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Widgets.DAL.DataLayer.WidgetsIndici;

public interface IWidgetIndici
{
    Task<DataTable?> GetWidgetKpiAsync(uint year, string piva, AgronicaCoreParametriServer objParametriServer);
    Task<DataTable> ReadDatiImpresaIndiceProduttivita(string piva, AgronicaCoreParametriServer objParametriServer);

    Task<DataTable> ReadSpecificIndiciProduttivitaAsync(string piva, int year, int? vegCod,
        AgronicaCoreParametriServer objParametriServer);

    Task<DataTable> ReadGeneralIndiciProduttivitaAsync(string regione, int year, AgronicaCoreParametriServer objParametriServer);
    Task<DataTable> ReadAvailableYearsAsync(string piva, AgronicaCoreParametriServer objParametriServer);
}