using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.DettagliMateriePrime
{
    public interface IDettagliMateriePrimeService
    {
        /// <summary>
        /// Recupera i dettagli di una materia prima e li mappa in un oggetto DTO.
        /// </summary>
        Task<MateriePrime?> GetMateriaPrimaByCodAsync(int matCod, AgronicaCoreParametriServer objParametriServer);
    }
}
