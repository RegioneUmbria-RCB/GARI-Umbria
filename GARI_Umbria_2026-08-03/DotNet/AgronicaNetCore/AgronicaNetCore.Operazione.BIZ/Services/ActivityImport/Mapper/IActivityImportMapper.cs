using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InData.ActivityImport;
using AgronicaCoreModelsSTD.attivita;
using AgronicaNetCore.Base.Models;
using AgronicaCoreDTOStd.InData.ActivityImport;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Operazione.BIZ.Services.ActivityImport.Mapper
{
    public interface IActivityImportMapper<T> where T : IActivityImportData
    {
        /// <summary>
        /// Set the base for the current object.
        /// This method must be called before executing the MapActivity method.
        /// </summary>
        /// <param name="baseObject">Base object to be mapped.</param>
        void SetBase(T baseObject);
        /// <summary>
        /// Maps the base of the current object into a new activity.
        /// Should be used when the activity is not grouped (single activity operation).
        /// </summary>
        /// <returns>A new activity ready to be saved.</returns>
        Task<Attivita> MapActivity(AgronicaCoreParametriServer objParametriServer);
        /// <summary>
        /// Maps the base of the current object into a new activity.
        /// </summary>
        /// /// <param name="raccoglitore">The code that groups multi-activity
        /// operations. If the operation formed by a single activity pass 0.</param>
        /// <returns>A new activity ready to be saved.</returns>
        Task<Attivita> MapActivity(string piva, int raccoglitore, AgronicaCoreParametriServer objParametriServer);
    }
}
