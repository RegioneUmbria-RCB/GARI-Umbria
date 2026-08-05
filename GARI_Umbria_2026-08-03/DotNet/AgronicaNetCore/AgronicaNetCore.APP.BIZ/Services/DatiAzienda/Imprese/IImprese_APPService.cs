using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Imprese
{
    public interface IImprese_APPService
    {
        Task<ImpresaEntity?> LeggiImpresaAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
