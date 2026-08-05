using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Widgets.DAL.DataLayer.WidgetsDocumentale.WidgetsDocumentale
{
    public interface IWidgetsDocumentale
    {
        Task<DataTable?> GetDocumentRecapAsync(DateTime timeStart, bool useWorkflow, bool userVisibility, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
