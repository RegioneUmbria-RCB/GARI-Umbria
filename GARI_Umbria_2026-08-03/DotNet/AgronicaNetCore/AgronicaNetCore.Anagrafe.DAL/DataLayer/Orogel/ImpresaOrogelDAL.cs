using System.Data;
using System.Text;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Orogel
{
    /// <summary>
    /// DS05-BL §Persistenze / §Query base: Concrete implementation of <see cref="IImpresaOrogelDAL"/>.
    /// Executes a paginated SELECT against <c>Imprese</c> applying temporal validity,
    /// optional company code and keyset pagination filters.
    /// </summary>
    public class ImpresaOrogelDAL : BaseDALAnagrafe, IImpresaOrogelDAL
    {
        /// <summary>
        /// DS05-BL: Initializes <see cref="ImpresaOrogelDAL"/>.
        /// </summary>
        public ImpresaOrogelDAL(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiImpresePaginateAsync(
            int pageSize,
            string filtroWherePaginazione,
            IReadOnlyDictionary<string, object> parSqlPaginazione,
            IReadOnlyList<string> codiciAzienda,
            AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();
            var stbQuery = new StringBuilder();

            // DS05-BL §Query base
            stbQuery.AppendLine("SELECT TOP(@pageSize)");
            stbQuery.AppendLine("    PIVA                AS codice_azienda,");
            stbQuery.AppendLine("    rag_soc             AS ragione_sociale,");
            stbQuery.AppendLine("    Validita_Inizio     AS data_inizio,");
            stbQuery.AppendLine("    Validita_Fine       AS data_fine");
            stbQuery.AppendLine("FROM Imprese");

            // DS05-BL §Filtri Obbligatori: Inviato >= 0
            stbQuery.AppendLine("WHERE Inviato >= 0");

            parSql.Add("@pageSize", pageSize);

            // DS05-BL §Filtro Opzionale Azienda
            if (codiciAzienda.Count > 0)
            {
                stbQuery.AppendLine("    AND PIVA IN (@filtroPiva)");
                parSqlIn.Add("@filtroPiva", FormatClauseIn(codiciAzienda.ToList()));
            }

            // DS05-BL §Filtro Paginazione Keyset: append WHERE fragment from DS04-BL
            if (parSqlPaginazione.Count > 0)
            {
                stbQuery.AppendLine($"    AND ({filtroWherePaginazione})");
                foreach (KeyValuePair<string, object> kv in parSqlPaginazione)
                    parSql[kv.Key] = kv.Value;
            }

            // DS05-BL §Ordine statico Obbligatorio
            stbQuery.AppendLine("ORDER BY PIVA ASC");

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
