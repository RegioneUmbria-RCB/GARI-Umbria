using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;
using AgronicaNetCore.APP.BIZ.Common;
using AgronicaNetCore.APP.BIZ.Services.ImpostazioniApp;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Prodotti
{
    public interface IProdotti_APPService
    {
        Task<List<ProdottoEntity>> LeggiProdottiAPPAsync(
            string piva,
            int elemCod,
            string specieProdotti,
            string statoCod,
            bool limitazioniPianoColturale,
            bool leggiNonMovimentati,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer,
            ImpostazioniAppModel? impostazioniApp = null,
            bool modalitaDemetra = false
        );
    }
}
