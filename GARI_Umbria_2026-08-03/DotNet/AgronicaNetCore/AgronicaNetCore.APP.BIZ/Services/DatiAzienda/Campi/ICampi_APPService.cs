using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Campi
{
    public interface ICampi_APPService
    {
        Task<List<CampoEntity>> LeggiCampiAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
