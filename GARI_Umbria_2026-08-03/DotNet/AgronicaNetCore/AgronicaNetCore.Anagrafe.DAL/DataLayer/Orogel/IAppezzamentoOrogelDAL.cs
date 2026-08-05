using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Orogel
{
    /// <summary>
    /// DS08-BL §Persistenze: Data access contract for paginated extraction of appezzamento
    /// data from <c>Impresa_Progetti</c> INNER JOIN <c>Reg_Impianti</c> with lookup LEFT JOINs.
    /// </summary>
    public interface IAppezzamentoOrogelDAL
    {
        /// <summary>
        /// DS08-BL §Query base / Filtri / Paginazione: Executes
        /// <c>SELECT TOP(@pageSize) … FROM Impresa_Progetti ese INNER JOIN Reg_Impianti imp …</c>
        /// applying temporal validity, optional company code and single-key keyset pagination
        /// (<c>ese.Progetto_Cod &gt; @LastProgettoCod</c>).
        /// The keyset WHERE is hardcoded with the <c>ese.</c> table alias to prevent
        /// column ambiguity with the INNER JOIN.
        /// </summary>
        /// <param name="pageSize">Number of rows to fetch (<c>TOP</c> argument).</param>
        /// <param name="parSqlPaginazione">
        /// Keyset SQL parameter from DS04-BL: <c>{ "@LastProgettoCod": int }</c>.
        /// The WHERE fragment (<c>ese.Progetto_Cod &gt; @LastProgettoCod</c>) is built internally.
        /// </param>
        /// <param name="codiciAzienda">Optional PIVA filter list. Empty list → no filter.</param>
        /// <param name="objParametriServer">Connection parameters for the target archive database.</param>
        /// <returns>
        /// A <see cref="DataTable"/> whose column aliases match the DS08-BL output fields
        /// (<c>partitaIva</c>, <c>saCod</c>, <c>appezzamento</c>, <c>impianto</c>, <c>esercizio</c>, …).
        /// </returns>
        Task<DataTable> LeggiAppezzamentiPaginatiAsync(
            int pageSize,
            IReadOnlyDictionary<string, object> parSqlPaginazione,
            IReadOnlyList<string> codiciAzienda,
            AgronicaCoreParametriServer objParametriServer);
    }
}
