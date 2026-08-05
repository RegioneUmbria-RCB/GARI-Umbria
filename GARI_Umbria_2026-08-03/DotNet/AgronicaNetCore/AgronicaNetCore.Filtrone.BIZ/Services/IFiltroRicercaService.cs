using AgronicaCoreDTOStd.InData.FiltroRicerca;
using AgronicaNetCore.Base.Models;
using AgronicaCoreDTOStd.OutData.FiltroRicerca;

namespace AgronicaNetCore.FiltroRicerca.BIZ.Services
{
    public interface IFiltroRicercaService
    {
        /// <param name="capSelectTopRows">
        /// Optional hard ceiling on the number of rows returned, applied before the query runs.
        /// When set, overrides <c>NumeroMassimoRigheEstraibiliFiltroRicerca</c> if that config
        /// value is 0 (unlimited) or greater than this cap. Callers that do not pass a value
        /// keep the existing config-driven behaviour.
        /// </param>
        Task<CriteriRicerca_OUT> GetResultAsync(CriteriRicerca_IN criteriRicerca_IN, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriSuperServer objParametriSuperServer, int? capSelectTopRows = null);
        Task<string> GetPivaRealeAsync(string piva, AgronicaCoreParametriServer objParametriServer);
    }
}