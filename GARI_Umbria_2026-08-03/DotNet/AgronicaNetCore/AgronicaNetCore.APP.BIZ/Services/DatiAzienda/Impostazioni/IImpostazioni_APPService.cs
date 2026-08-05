using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Impostazioni
{
    public interface IImpostazioni_APPService
    {
        Task<List<ImpostazioneEntity>> LeggiImpostazioniAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
