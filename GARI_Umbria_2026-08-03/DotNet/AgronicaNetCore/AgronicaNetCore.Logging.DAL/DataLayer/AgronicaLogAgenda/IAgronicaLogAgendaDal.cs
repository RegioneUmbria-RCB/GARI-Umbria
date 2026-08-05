using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogAgenda
{
    public interface IAgronicaLogAgendaDal
    {
        Task<bool> EsistonoModificheDopoLaDataPerAttivitaAppAsync(string piva, DateTime dataLavorazioneMin, DateTime dataModificheMin, AgronicaCoreParametriServer parametriServer);
    }
}
