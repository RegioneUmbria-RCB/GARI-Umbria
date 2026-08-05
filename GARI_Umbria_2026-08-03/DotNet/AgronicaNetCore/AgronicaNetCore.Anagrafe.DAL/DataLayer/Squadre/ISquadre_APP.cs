using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Squadre
{
    /// <summary>
    /// Ref: DS08-BL sections 3.1.3 and 7.1.
    /// Legge le squadre aziendali dalla sorgente SquadreXAttivita.
    /// </summary>
    public interface ISquadre_APP
    {
        Task<List<SquadreEntity>> LeggiAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti);
    }
}