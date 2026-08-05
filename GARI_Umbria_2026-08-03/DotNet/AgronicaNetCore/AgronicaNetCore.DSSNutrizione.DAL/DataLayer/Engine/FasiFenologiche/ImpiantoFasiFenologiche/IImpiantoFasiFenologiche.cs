using AgronicaCoreModelsSTD.Engine;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.Models;
using InData.Engine.FasiFenologiche;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.ImpiantoFasiFenologiche
{
    /// <summary>
    /// Contratto DAL per la persistenza delle fasi fenologiche nella tabella Impianto_Fasi_Fenologiche_Engine.
    /// </summary>
    /// <remarks>
    /// Design Specification: PersistenzaFasiFenologiche - Persistenze Coinvolte.
    /// </remarks>
    public interface IImpiantoFasiFenologiche
    {
        /// <summary>
        /// Inserisce le fasi una alla volta (usato quando il numero di fasi è ≤ 50).
        /// </summary>
        Task InserisciFasiAsync(
            ImpiantoInput impianto,
            IReadOnlyList<FaseFenologicaEngine> fasi,
            MetadataAcquisizione metadata,
            AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Esegue un bulk insert ottimizzato (usato quando il numero di fasi è > 50).
        /// </summary>
        Task EseguiBulkInsertAsync(
            ImpiantoInput impianto,
            IReadOnlyList<FaseFenologicaEngine> fasi,
            MetadataAcquisizione metadata,
            AgronicaCoreParametriServer objParametriServer);
    }
}
