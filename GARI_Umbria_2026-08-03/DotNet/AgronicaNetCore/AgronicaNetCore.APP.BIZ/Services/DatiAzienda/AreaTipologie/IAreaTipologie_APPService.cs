using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.AreaTipologie
{
    public interface IAreaTipologie_APPService
    {
        Task<(
            List<AreaTipologieDocumentoEntity> aree,
            List<TipologiaDocumentoEntity> tipologie
        )> LeggiTipologieAsync(AgronicaCoreParametriServer objParametriServer);
    }
}
