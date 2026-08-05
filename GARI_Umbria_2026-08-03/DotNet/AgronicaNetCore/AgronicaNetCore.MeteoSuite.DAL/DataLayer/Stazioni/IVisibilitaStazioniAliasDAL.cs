using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.MeteoSuite.DAL.DataLayer.Stazioni
{
    public interface IVisibilitaStazioniAliasDAL
    {
        /// <summary>
        /// Legge elenco alias stazioni.
        /// </summary>
        Task<DataTable?> LeggiElencoAsync(string piva, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Legge singolo alias associato a stazione.
        /// </summary>
        Task<DataTable?> LeggiPerStazioneAsync(string piva, int idStazione, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Aggiorna od inserisce alias per stazione.
        /// </summary>
        Task<DataTable?> UpsertAsync(string piva, int idStazione, string strAlias, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Cancella alias per stazione
        /// </summary>
        Task<bool> DeleteAsync(string piva, int idStazione, AgronicaCoreParametriServer objParametriServer);
    }
}
