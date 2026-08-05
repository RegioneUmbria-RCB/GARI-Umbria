using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.AttivitaxCentriAziendali
{
    public interface IAttivitaxCentriAziendali_APPService
    {
        Task<List<CentroAziendaleAttivitaCDGEntity>> LeggiAttivitaxCentriAziendaliAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
