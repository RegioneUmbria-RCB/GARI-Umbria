using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.AttivitaCDG
{
    public interface IAttivitaCDG_APPService
    {
        Task<List<AttivitaCDGEntity>> LeggiAttivitaCDGAsync(
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
