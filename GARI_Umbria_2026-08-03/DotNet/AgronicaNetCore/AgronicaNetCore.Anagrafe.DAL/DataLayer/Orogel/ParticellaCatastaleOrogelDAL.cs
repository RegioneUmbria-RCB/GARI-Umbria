using System.Data;
using System.Text;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Orogel
{
    /// <summary>
    /// DS07-BL §Persistenze / §Query base: Concrete implementation of <see cref="IParticellaCatastaleOrogelDAL"/>.
    /// Executes a paginated SELECT against <c>ImpreseXParticelle INNER JOIN ParticelleCatastali</c>
    /// applying temporal validity, optional company code and single-key keyset pagination filters.
    /// </summary>
    public class ParticellaCatastaleOrogelDAL : BaseDALAnagrafe, IParticellaCatastaleOrogelDAL
    {
        /// <summary>
        /// DS07-BL: Initializes <see cref="ParticellaCatastaleOrogelDAL"/>.
        /// </summary>
        public ParticellaCatastaleOrogelDAL(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiParticelleCatastaliPaginateAsync(
            int pageSize,
            IReadOnlyDictionary<string, object> parSqlPaginazione,
            IReadOnlyList<string> codiciAzienda,
            AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();
            var stbQuery = new StringBuilder();

            // DS07-BL §Query base: SELECT with INNER JOIN to ParticelleCatastali
            stbQuery.AppendLine("SELECT TOP(@pageSize)");
            stbQuery.AppendLine("    ixp.ID                                                              AS ID,");
            stbQuery.AppendLine("    ixp.PIVA                                                            AS codice_azienda,");
            stbQuery.AppendLine("    ixp.Sa_Cod                                                          AS codice_centro,");
            stbQuery.AppendLine("    CONCAT(pc.PROV, pc.COM)                                             AS codice_istat,");
            stbQuery.AppendLine("    pc.FOGLIO                                                           AS foglio,");
            stbQuery.AppendLine("    pc.SEZIONE                                                          AS sezione,");
            stbQuery.AppendLine("    pc.NUMERO                                                           AS particella,");
            stbQuery.AppendLine("    pc.SUBALTERNO                                                       AS subalterno,");

            // DS07-BL §Conversione superficie catastale: ETTARI + ARE/100 + CENTIARE/10000
            stbQuery.AppendLine("    (pc.ETTARI + pc.ARE / 100.0 + pc.CENTIARE / 10000.0)               AS superficie_catastale_ha,");
            stbQuery.AppendLine("    ixp.Sup_Condotta                                                    AS superficie_condotta_ha,");
            stbQuery.AppendLine("    ixp.Validita_Inizio                                                 AS data_inizio_conduzione,");
            stbQuery.AppendLine("    ixp.Validita_Fine                                                   AS data_fine_conduzione,");
            stbQuery.AppendLine("    ixp.TitoloPossesso                                                  AS codice_conduzione,");

            // DS07-BL §Mapping enum codice_conduzione: inline CASE for descrizione_conduzione
            stbQuery.AppendLine("    CASE ixp.TitoloPossesso");
            stbQuery.AppendLine("        WHEN 1 THEN 'Proprietà'");
            stbQuery.AppendLine("        WHEN 2 THEN 'Comodato d''uso'");
            stbQuery.AppendLine("        WHEN 3 THEN 'Affitto con contratto'");
            stbQuery.AppendLine("        WHEN 4 THEN 'Affitto senza contratto'");
            stbQuery.AppendLine("        WHEN 5 THEN 'In conto terzi'");
            stbQuery.AppendLine("        ELSE 'Altro'");
            stbQuery.AppendLine("    END                                                                  AS descrizione_conduzione");

            stbQuery.AppendLine("FROM ImpreseXParticelle ixp");

            // DS07-BL §Persistenze: INNER JOIN on all cadastral key columns
            stbQuery.AppendLine("INNER JOIN ParticelleCatastali pc");
            stbQuery.AppendLine("    ON ixp.PROV      = pc.PROV");
            stbQuery.AppendLine("    AND ixp.COM      = pc.COM");
            stbQuery.AppendLine("    AND ixp.SEZIONE  = pc.SEZIONE");
            stbQuery.AppendLine("    AND ixp.FOGLIO   = pc.FOGLIO");
            stbQuery.AppendLine("    AND ixp.NUMERO   = pc.NUMERO");
            stbQuery.AppendLine("    AND ixp.SUBALTERNO = pc.SUBALTERNO");

            // DS07-BL §Filtri Obbligatori: Inviato >= 0
            stbQuery.AppendLine("WHERE ixp.Inviato >= 0");

            parSql.Add("@pageSize", pageSize);

            // DS07-BL §Filtro Opzionale Azienda
            if (codiciAzienda.Count > 0)
            {
                stbQuery.AppendLine("    AND ixp.PIVA IN (@filtroPiva)");
                parSqlIn.Add("@filtroPiva", FormatClauseIn(codiciAzienda.ToList()));
            }

            // DS07-BL §Filtro Paginazione Keyset: hardcoded with ixp. alias to prevent ambiguity
            // with any ID column that ParticelleCatastali might expose.
            if (parSqlPaginazione.Count > 0)
            {
                stbQuery.AppendLine("    AND ixp.ID > @LastId");
                foreach (KeyValuePair<string, object> kv in parSqlPaginazione)
                    parSql[kv.Key] = kv.Value;
            }

            // DS07-BL §Ordine Statico Obbligatorio
            stbQuery.AppendLine("ORDER BY ixp.ID ASC");

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
