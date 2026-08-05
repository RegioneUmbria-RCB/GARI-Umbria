using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Squadre
{
    /// <summary>
    /// Ref: DS08-BL sections 3.2.3 and 7.2.
    /// Orchestrates the acquisition of company teams for DatiAzienda synchronisation.
    /// </summary>
    public interface ISquadre_APPService
    {
        /// <summary>
        /// Ref: DS08-BL sections 3.2.3 and 7.2.
        /// Legge le squadre aziendali valide per la partita IVA richiesta.
        /// </summary>
        Task<List<SquadreEntity>> LeggiAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti);
    }
}