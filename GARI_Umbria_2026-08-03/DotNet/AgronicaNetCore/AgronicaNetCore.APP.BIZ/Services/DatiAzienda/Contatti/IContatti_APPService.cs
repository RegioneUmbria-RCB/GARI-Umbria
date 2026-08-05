using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Contatti
{
    public interface IContatti_APPService
    {
        Task<Contatti_APPResult> LeggiAsync(
            Contatti_APPRequest request,
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
