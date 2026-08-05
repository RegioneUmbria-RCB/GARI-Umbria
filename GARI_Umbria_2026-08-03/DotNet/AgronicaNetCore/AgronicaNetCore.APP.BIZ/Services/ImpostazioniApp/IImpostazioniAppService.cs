using AgronicaCoreModelsSTD.App;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.UtentiImpostazioni;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.ImpostazioniApp;

public interface IImpostazioniAppService
{
    Task<PermessiUtenteSincronizzazioneEntity> LeggiPermessiAppAsync(
        AgronicaCoreParametriUtenti objParametriUtenti,
        AgronicaCoreParametriServer objParametriServer
    );

    Task<ImpostazioniAPP?> LeggiImpostazioniAppAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
}
