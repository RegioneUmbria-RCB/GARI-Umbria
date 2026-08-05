using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Magazzini
{
    public interface IMagazzini_APPService
    {
        Task<List<FabbricatoEntity>> LeggiMagazziniAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
