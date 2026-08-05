using AgronicaCoreDTOStd.InData.FiltroRicerca;
using AgronicaCoreModelsSTD.Models.ConfiguratoreFiltroRicerca;
using AgronicaNetCore.Base.Models;
using AgronicaCoreDTOStd.OutData.FiltroRicerca;

namespace AgronicaNetCore.FiltroRicerca.DAL.DataLayer.FiltroRicerca
{
    public interface IFiltroRicerca
    {
        Task<CriteriRicerca_OUT> GetResultAsync(CriteriRicercaExtended criteriRicerca, ConfiguratoreFiltroRicerca configuratore, string utenti_DB_name, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
