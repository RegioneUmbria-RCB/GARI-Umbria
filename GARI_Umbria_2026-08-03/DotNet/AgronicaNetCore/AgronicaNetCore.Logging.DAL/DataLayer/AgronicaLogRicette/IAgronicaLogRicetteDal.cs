using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogRicette
{
    public interface IAgronicaLogRicetteDal
    {
        Task<bool> EsistonoModificheDopoLaDataPerBrogliacciAppAsync(string piva, DateTime dataLavorazioneMin, DateTime dataModificheMin, AgronicaCoreParametriServer parametriServer);
    }
}
