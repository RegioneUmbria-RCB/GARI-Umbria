using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.APP.BIZ.Services.ImpostazioniApp;
using AgronicaNetCore.Base.Models;
using PianoColturaleEntity = AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda.DatiAziendaEntity.PianoColturaleEntity;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.PianoColturale
{
    public interface IPianoColturale_APPService
    {
        Task<PianoColturaleEntity> LeggiPianoColturaleAsync(
            string piva,
            DateTime data,
            bool leggiCompleto,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            ImpostazioniAppModel? impostazioniApp = null,
            bool modalitaDemetra = false
        );
    }
}
