using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Contatti
{
    public interface IContatti_APP
    {
        Task<DataTable> ReadAsync(
            string? piva,
            bool includeContatti,
            bool includeFornitoriMeteo,
            bool includeFornitoriNormali,
            AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> ContattiFornitoriMovimentati_APPAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> ContattiLavoratoriMovimentati_APPAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer);
    }
}
