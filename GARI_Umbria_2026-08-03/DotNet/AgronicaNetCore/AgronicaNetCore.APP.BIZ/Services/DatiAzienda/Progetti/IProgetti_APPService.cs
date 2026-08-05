using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Progetti
{
    public interface IProgetti_APPService
    {
        Task<List<ProgettoEntity>> LeggiProgettiAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
