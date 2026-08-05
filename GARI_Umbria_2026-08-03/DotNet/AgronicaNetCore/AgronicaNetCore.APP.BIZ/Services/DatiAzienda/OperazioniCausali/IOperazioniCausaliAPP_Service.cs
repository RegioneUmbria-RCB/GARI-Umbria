using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.OperazioniCausali
{
    public interface IOperazioniCausaliAPP_Service
    {
        Task<List<OperazioneCausaleEntity>> LeggiAsync(
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
