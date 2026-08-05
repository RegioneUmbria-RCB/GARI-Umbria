using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.MeteoSuite.DAL.DataLayer.Stazioni
{
    public interface ITbRerQuadrantiDAL
    {
        /// <summary>
        /// Legge elenco quadranti metereo RER (Regione Emilia-Romagna).
        /// </summary>
        Task<DataTable?> LeggiElencoQuadrantiAsync(AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Legge singolo quadrante metereo RER (Regione Emilia-Romagna).
        /// </summary>
        Task<DataTable?> LeggiQuadrantePerIdAsync(int id, AgronicaCoreParametriServer objParametriServer);
    }
}
