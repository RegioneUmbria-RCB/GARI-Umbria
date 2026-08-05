using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.MisureIndiciMaturitaAnagrafiche
{
    public interface IMisureIndiciMaturitaAnagrafiche_APPService
    {
        Task<List<MisuraIndiciMaturitaAnagraficheEntity>> LeggiMisureIndiciMaturitaAnagraficheAsync(AgronicaCoreParametriServer objParametriServer);
    }
}