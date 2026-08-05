using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.MeteoSuite.DAL.DataLayer.Stazioni
{
    public interface ITbRerStazioniDAL
    {
        /// <summary>
        /// Legge elenco stazioni metereologiche per quadranti RER (Regione Emilia-Romagna).
        /// </summary>
        Task<DataTable?> LeggiElencoStazioniAsync(AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Legge singola stazione metereologica per quadrante RER (Regione Emilia-Romagna).
        /// </summary>
        Task<DataTable?> LeggiStazionePerIdAsync(int id, AgronicaCoreParametriServer objParametriServer);
    }
}
