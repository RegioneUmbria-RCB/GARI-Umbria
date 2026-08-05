using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.ParcoMacchine
{
    public interface IParcoMacchine_APPService
    {
        Task<List<ParcoMacchineEntity>> LeggiParcoMacchineAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        );
        Task<List<MacchinaEntity>> LeggiMacchineAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
