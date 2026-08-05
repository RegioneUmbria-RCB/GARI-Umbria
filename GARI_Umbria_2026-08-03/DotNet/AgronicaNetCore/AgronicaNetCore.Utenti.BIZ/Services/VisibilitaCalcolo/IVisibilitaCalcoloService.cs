using AgronicaCoreDTOStd.InData.Pratiche;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Utenti.BIZ.Services.VisibilitaCalcolo
{
    public interface IVisibilitaCalcoloService
    {
        Task<VisibilitaCombinataResult> CalcolaPreviewAsync(
            string username,
            string descrizione1,
            string descrizione2,
            bool filtroPraticheAttivo,
            string operatoreFiltri,
            List<PraticaFiltrata_IN> pratiche,
            AgronicaCoreParametriDouble objParametriDouble,
            CancellationToken ct = default);

        Task<VisibilitaCombinataResult> CalcolaEPersistiAsync(
            string username,
            string descrizione1,
            string descrizione2,
            bool filtroPraticheAttivo,
            string operatoreFiltri,
            List<PraticaFiltrata_IN> pratiche,
            AgronicaCoreParametriDouble objParametriDouble,
            CancellationToken ct = default);
    }
}
