using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.CodificaProdotti
{
    public interface ICodificaProdotti_APPService
    {
        Task<List<CodificaProdottoEntity>> LeggiCodificaProdottiAsync(
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
