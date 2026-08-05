using System.Data;
using System.Text;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Orogel
{
    /// <summary>
    /// DS09-BL §Persistenze / §Query base: Concrete implementation of
    /// <see cref="IMappingAppezzamentiParticelleOrogelDAL"/>.
    /// Executes a paginated SELECT against
    /// <c>AppezzamentixParticelle INNER JOIN Imprese_Progetti INNER JOIN ParticelleCatastali</c>
    /// applying temporal validity, optional company code and a hardcoded 3-key
    /// cascading keyset pagination WHERE fragment.
    /// </summary>
    public class MappingAppezzamentiParticelleOrogelDAL
        : BaseDALAnagrafe, IMappingAppezzamentiParticelleOrogelDAL
    {
        /// <summary>
        /// DS09-BL: Initializes <see cref="MappingAppezzamentiParticelleOrogelDAL"/>.
        /// </summary>
        public MappingAppezzamentiParticelleOrogelDAL(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiMappingAppezzamentiParticellePaginatoAsync(
            int pageSize,
            IReadOnlyDictionary<string, object> parSqlPaginazione,
            IReadOnlyList<string> codiciAzienda,
            AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();
            var stbQuery = new StringBuilder();

            // DS09-BL §Query base: SELECT with 2 INNER JOINs (WITH NOLOCK)
            stbQuery.AppendLine("SELECT TOP(@pageSize)");
            stbQuery.AppendLine("    i.Progetto_Cod          AS codice_esercizio,");
            stbQuery.AppendLine("    p.PART_COD              AS particellaCodice,");
            stbQuery.AppendLine("    a.Validita_inizio        AS validitaInizioRelazione,");
            stbQuery.AppendLine("    a.PIVA                  AS codice_azienda,");
            stbQuery.AppendLine("    a.sa_cod                AS codice_centro,");
            stbQuery.AppendLine("    a.APPEZZA               AS codice_appezzamento,");
            stbQuery.AppendLine("    i.Id_Reg                AS codice_impianto,");
            stbQuery.AppendLine("    a.PROV                  AS provincia,");
            stbQuery.AppendLine("    a.com                   AS comune,");
            stbQuery.AppendLine("    a.SEZIONE               AS sezione,");
            stbQuery.AppendLine("    a.FOGLIO                AS foglio,");
            stbQuery.AppendLine("    a.NUMERO                AS particella,");
            stbQuery.AppendLine("    a.SUBALTERNO            AS subalterno,");
            stbQuery.AppendLine("    CONCAT(a.PROV, a.com)   AS codice_istat,");
            stbQuery.AppendLine("    a.Validita_Fine         AS validitaFineRelazione,");
            stbQuery.AppendLine("    a.AREA                  AS superficie_attribuita_ha");

            stbQuery.AppendLine("FROM AppezzamentixParticelle a");

            // DS09-BL §Persistenze: INNER JOIN Imprese_Progetti to retrieve Progetto_Cod and Id_Reg
            stbQuery.AppendLine("INNER JOIN Imprese_Progetti i");
            stbQuery.AppendLine("    ON i.Piva     = a.PIVA");
            stbQuery.AppendLine("    AND i.Sa_Cod  = a.SA_COD");
            stbQuery.AppendLine("    AND i.Appezza = a.APPEZZA");

            // DS09-BL §Persistenze: INNER JOIN ParticelleCatastali to retrieve PART_COD
            stbQuery.AppendLine("INNER JOIN ParticelleCatastali p");
            stbQuery.AppendLine("    ON p.PROV      = a.PROV");
            stbQuery.AppendLine("    AND p.com      = a.com");
            stbQuery.AppendLine("    AND p.SEZIONE  = a.SEZIONE");
            stbQuery.AppendLine("    AND p.FOGLIO   = a.FOGLIO");
            stbQuery.AppendLine("    AND p.NUMERO   = a.NUMERO");
            stbQuery.AppendLine("    AND p.SUBALTERNO = a.SUBALTERNO");

            // DS09-BL §Filtri Obbligatori: Inviato >= 0
            stbQuery.AppendLine("WHERE a.Inviato >= 0");

            parSql.Add("@pageSize", pageSize);

            // DS09-BL §Filtro Opzionale Azienda
            if (codiciAzienda.Count > 0)
            {
                stbQuery.AppendLine("    AND a.PIVA IN (@filtroPiva)");
                parSqlIn.Add("@filtroPiva", FormatClauseIn(codiciAzienda.ToList()));
            }

            // DS09-BL §Filtro paginazione 3-chiave cascata: hardcoded with table aliases.
            // Uses i.Progetto_Cod, p.PART_COD, a.Validita_inizio to prevent column ambiguity.
            if (parSqlPaginazione.Count > 0)
            {
                stbQuery.AppendLine("    AND (");
                stbQuery.AppendLine("        i.Progetto_Cod > @LastProgettoCod");
                stbQuery.AppendLine("        OR (i.Progetto_Cod = @LastProgettoCod AND p.PART_COD > @LastPartCod)");
                stbQuery.AppendLine("        OR (i.Progetto_Cod = @LastProgettoCod AND p.PART_COD = @LastPartCod AND a.Validita_inizio > @LastValiditaInizio)");
                stbQuery.AppendLine("    )");
                foreach (KeyValuePair<string, object> kv in parSqlPaginazione)
                    parSql[kv.Key] = kv.Value;
            }

            // DS09-BL §Ordine Statico Obbligatorio (3-column composite)
            stbQuery.AppendLine("ORDER BY i.Progetto_Cod ASC, p.PART_COD ASC, a.Validita_inizio ASC");

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
