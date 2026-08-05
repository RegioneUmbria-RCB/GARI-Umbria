using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.LavorazioniAttivitaCDG
{
    public interface ILavorazioniAttivitaCDG_APPService
    {
        Task<List<LavorazioneAttivitaCDGEntity>> LeggiLavorazioniAttivitaCDGAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
