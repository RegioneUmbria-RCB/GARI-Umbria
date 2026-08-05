using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Orogel
{
    /// <summary>
    /// DS09-BL §Persistenze: Data access contract for paginated extraction of the
    /// appezzamento–particella mapping from <c>AppezzamentixParticelle</c>
    /// INNER JOIN <c>Imprese_Progetti</c> and <c>ParticelleCatastali</c>.
    /// </summary>
    public interface IMappingAppezzamentiParticelleOrogelDAL
    {
        /// <summary>
        /// DS09-BL §Query base / Filtri / Paginazione: Executes
        /// <c>SELECT TOP(@pageSize) … FROM AppezzamentixParticelle a … ORDER BY i.Progetto_Cod ASC, p.PART_COD ASC, a.Validita_inizio ASC</c>
        /// applying temporal validity, optional company code and a hardcoded
        /// 3-key cascading keyset WHERE fragment.
        /// The WHERE fragment is built internally with table-qualified column references
        /// (<c>i.Progetto_Cod</c>, <c>p.PART_COD</c>, <c>a.Validita_inizio</c>) to
        /// avoid ambiguity across the three-table JOIN.
        /// </summary>
        /// <param name="pageSize">Number of rows to fetch (<c>TOP</c> argument).</param>
        /// <param name="parSqlPaginazione">
        /// Decoded 3-key cursor parameters built by the BIZ service:
        /// <c>{ "@LastProgettoCod": int, "@LastPartCod": string, "@LastValiditaInizio": DateTime }</c>.
        /// The WHERE fragment is constructed internally.
        /// </param>
        /// <param name="codiciAzienda">Optional PIVA filter list. Empty list → no filter.</param>
        /// <param name="objParametriServer">Connection parameters for the target archive database.</param>
        /// <returns>
        /// A <see cref="DataTable"/> with columns:
        /// <c>codice_esercizio</c>, <c>particellaCodice</c>, <c>validitaInizioRelazione</c>,
        /// <c>codice_azienda</c>, <c>codice_centro</c>, <c>codice_appezzamento</c>,
        /// <c>codice_impianto</c>, <c>provincia</c>, <c>comune</c>, <c>sezione</c>,
        /// <c>foglio</c>, <c>particella</c>, <c>subalterno</c>, <c>codice_istat</c>,
        /// <c>validitaFineRelazione</c>, <c>superficie_attribuita_ha</c>.
        /// </returns>
        Task<DataTable> LeggiMappingAppezzamentiParticellePaginatoAsync(
            int pageSize,
            IReadOnlyDictionary<string, object> parSqlPaginazione,
            IReadOnlyList<string> codiciAzienda,
            AgronicaCoreParametriServer objParametriServer);
    }
}
