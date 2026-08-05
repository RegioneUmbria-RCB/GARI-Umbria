using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Orogel
{
    /// <summary>
    /// DS07-BL §Persistenze: Data access contract for paginated extraction of particella catastale
    /// data from <c>ImpreseXParticelle</c> INNER JOIN <c>ParticelleCatastali</c>.
    /// </summary>
    public interface IParticellaCatastaleOrogelDAL
    {
        /// <summary>
        /// DS07-BL §Query base / Filtri / Paginazione: Executes
        /// <c>SELECT TOP(@pageSize) … FROM ImpreseXParticelle ixp INNER JOIN ParticelleCatastali pc … ORDER BY ixp.ID ASC</c>
        /// applying temporal validity filters, an optional company code filter and a single-key
        /// keyset pagination filter (<c>ixp.ID &gt; @LastId</c>).
        /// The keyset WHERE is hardcoded with the <c>ixp.</c> table alias to prevent
        /// column ambiguity with the INNER JOIN.
        /// </summary>
        /// <param name="pageSize">Number of rows to fetch (<c>TOP</c> argument).</param>
        /// <param name="parSqlPaginazione">
        /// Keyset SQL parameter from DS04-BL: <c>{ "@LastId": int }</c>.
        /// The WHERE fragment (<c>ixp.ID &gt; @LastId</c>) is built internally.
        /// </param>
        /// <param name="codiciAzienda">Optional PIVA filter list. Empty list → no filter.</param>
        /// <param name="objParametriServer">Connection parameters for the target archive database.</param>
        /// <returns>
        /// A <see cref="DataTable"/> with columns: <c>ID</c>, <c>codice_azienda</c>,
        /// <c>codice_centro</c>, <c>codice_istat</c>, <c>foglio</c>, <c>sezione</c>,
        /// <c>particella</c>, <c>subalterno</c>, <c>superficie_catastale_ha</c>,
        /// <c>superficie_condotta_ha</c>, <c>data_inizio_conduzione</c>,
        /// <c>data_fine_conduzione</c>, <c>codice_conduzione</c>, <c>descrizione_conduzione</c>.
        /// </returns>
        Task<DataTable> LeggiParticelleCatastaliPaginateAsync(
            int pageSize,
            IReadOnlyDictionary<string, object> parSqlPaginazione,
            IReadOnlyList<string> codiciAzienda,
            AgronicaCoreParametriServer objParametriServer);
    }
}
