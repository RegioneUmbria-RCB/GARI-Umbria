using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.DettagliMateriePrime
{
    public interface IDettagliMateriePrime
    {
        /// <summary>
        /// Recupera i dettagli completi di una singola materia prima.
        /// </summary>
        Task<DataTable> GetDettagliMateriaPrimaAsync(int matCod, AgronicaCoreParametriServer objParametriServer);
    }
}
