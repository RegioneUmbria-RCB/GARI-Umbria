using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.MisureAvversitaAnagrafiche
{
    public interface IMisureAvversitaAnagrafiche_APPService
    {
        Task<List<MisuraAvversitaAnagraficheEntity>> LeggiMisureAvversitaAnagraficheAsync(AgronicaCoreParametriServer objParametriServer);
    }
}