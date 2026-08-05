using System.Data;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaNetCore.Base.Models;
using OutData.Zoo;

namespace AgronicaNetCore.Widgets.BIZ.Services.WidgetZoo
{
    public interface IWidgetZoo
    {
        Task<DataTable?> GetInvalidAnimals(string piva, int saCod, int staNum, DateTime timeStart, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti);

        Task<DataTable?> GetTreatmentsToDo(string piva, int saCod, int staNum, DateTime timeStart, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti);
        
        Task<DataTable?> GetTreatmentsToSend(string piva, int saCod, int staNum, DateTime timeStart, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti);
        
        Task<DataTable?> GetExpiringDrugs(string piva, DateTime timeStart, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti);
    }
}
