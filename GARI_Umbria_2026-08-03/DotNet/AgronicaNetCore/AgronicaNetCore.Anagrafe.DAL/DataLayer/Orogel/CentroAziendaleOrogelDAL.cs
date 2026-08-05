using System.Data;
using System.Text;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Orogel
{
    /// <summary>
    /// DS06-BL §Persistenze / §Query base: Concrete implementation of <see cref="ICentroAziendaleOrogelDAL"/>.
    /// Executes a paginated SELECT against <c>Centri_Aziendali</c> with geographical LEFT JOINs,
    /// applying temporal validity, optional company code and hardcoded 2-key keyset pagination filters.
    /// </summary>
    public class CentroAziendaleOrogelDAL : BaseDALAnagrafe, ICentroAziendaleOrogelDAL
    {
        /// <summary>
        /// DS06-BL: Initializes <see cref="CentroAziendaleOrogelDAL"/>.
        /// </summary>
        public CentroAziendaleOrogelDAL(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer) { }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiCentriAziendaliPaginatiAsync(
            int pageSize,
            IReadOnlyDictionary<string, object> parSqlPaginazione,
            IReadOnlyList<string> codiciAzienda,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();
            var stbQuery = new StringBuilder();

            // DS06-BL §Query base: SELECT with LEFT JOINs towards geographical tables
            stbQuery.AppendLine("SELECT TOP(@pageSize)");
            stbQuery.AppendLine("    ca.PIVA                                 AS codice_azienda,");
            stbQuery.AppendLine("    ca.sa_cod                               AS codice_centro,");
            stbQuery.AppendLine("    ca.Validita_Inizio                      AS data_inizio,");
            stbQuery.AppendLine("    ca.Validita_Fine                        AS data_fine,");
            stbQuery.AppendLine("    ISNULL(ind.ind_des, '')                 AS indirizzo,");
            stbQuery.AppendLine("    ISNULL(ind.frz_des, '')                 AS frazione,");
            stbQuery.AppendLine("    ISNULL(ind.CAP, '')                     AS cap,");
            stbQuery.AppendLine("    ISNULL(ist.LOCALITA, '')                AS localita,");
            stbQuery.AppendLine("    ISNULL(ind.pro_cod, '')                 AS provincia,");
            stbQuery.AppendLine("    CONCAT(ist.PROV, ist.COM)               AS codice_istat");
            stbQuery.AppendLine("FROM Centri_Aziendali ca");

            // DS06-BL §Recupero Indirizzo Sede Operativa: Tipo_Indirizzo 1 = enum_IndirizzoTipo.SedeOperativa
            stbQuery.AppendLine("LEFT JOIN CentrixIndirizzi cxi");
            stbQuery.AppendLine("    ON ca.PIVA = cxi.PIVA");
            stbQuery.AppendLine("    AND ca.sa_cod = cxi.sa_cod");
            stbQuery.AppendLine("    AND cxi.Tipo_Indirizzo = 1");
            stbQuery.AppendLine("LEFT JOIN Indirizzi ind");
            stbQuery.AppendLine("    ON cxi.cod_indirizzo = ind.cod_indirizzo");
            stbQuery.AppendLine("LEFT JOIN ISTAT ist");
            stbQuery.AppendLine("    ON ind.pro_cod_istat = ist.PROV");
            stbQuery.AppendLine("    AND ind.com_cod_istat = ist.COM");

            // DS06-BL §Filtri Obbligatori: Inviato >= 0
            stbQuery.AppendLine("WHERE ca.Inviato >= 0");

            parSql.Add("@pageSize", pageSize);

            // DS06-BL §Filtro Opzionale Azienda
            if (codiciAzienda.Count > 0)
            {
                stbQuery.AppendLine("    AND ca.PIVA IN (@filtroPiva)");
                parSqlIn.Add("@filtroPiva", FormatClauseIn(codiciAzienda.ToList()));
            }

            // DS06-BL §Filtro Paginazione Keyset: hardcoded 2-key fragment with table alias ca.
            // to resolve PIVA/sa_cod ambiguity across Centri_Aziendali, Centri_Aziendali_Codici, CentrixIndirizzi.
            if (parSqlPaginazione.Count > 0)
            {
                stbQuery.AppendLine(
                    "    AND (ca.PIVA > @LastPiva OR (ca.PIVA = @LastPiva AND ca.sa_cod > @LastSaCod))"
                );
                foreach (KeyValuePair<string, object> kv in parSqlPaginazione)
                    parSql[kv.Key] = kv.Value;
            }

            // DS06-BL §Ordine Statico Obbligatorio
            stbQuery.AppendLine("ORDER BY ca.PIVA ASC, ca.sa_cod ASC");

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
