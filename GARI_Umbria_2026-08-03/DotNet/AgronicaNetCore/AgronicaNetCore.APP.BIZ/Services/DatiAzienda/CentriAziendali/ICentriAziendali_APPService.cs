using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.CentriAziendali
{
    public interface ICentriAziendali_APPService
    {
        Task<List<CentroAziendaleEntity>> LeggiCentriAziendaliAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
