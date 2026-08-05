using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.Models;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.FasiFenologiche.Persistenza
{
    /// <summary>
    /// Servizio di persistenza delle fasi fenologiche ricevute dall'engine nella tabella Impianto_Fasi_Fenologiche_Engine.
    /// </summary>
    /// <remarks>
    /// Design Specification: PersistenzaFasiFenologiche - Scopo.
    /// </remarks>
    public interface IPersistenzaFasiFenologicheService
    {
        /// <summary>
        /// Salva atomicamente le fasi fenologiche nel database GIAS, applicando le regole di business
        /// su colonne standard, bulk insert e validazione referenziale.
        /// </summary>
        /// <param name="input">Dati di input: appezzamento, fasi e metadati di acquisizione.</param>
        /// <param name="objParametriServer">Parametri server GIAS.</param>
        /// <returns>Esito della persistenza con numero di fasi salvate e ID inseriti.</returns>
        /// <remarks>
        /// Design Specification: PersistenzaFasiFenologiche - Regole di Business.
        /// </remarks>
        Task<bool> SalvaAsync(
            PersistenzaFasiFenologicheInput input,
            AgronicaCoreParametriServer objParametriServer);
    }
}
