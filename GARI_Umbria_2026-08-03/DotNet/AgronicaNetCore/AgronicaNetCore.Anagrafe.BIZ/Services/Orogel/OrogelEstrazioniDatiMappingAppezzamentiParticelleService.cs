using System.Data;
using System.Globalization;
using AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Orogel;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    /// <summary>
    /// DS09-BL: Orchestrates the extraction of the paginated appezzamento–particella mapping (FS005).
    /// Decodes and encodes the 3-key cursor (<c>Progetto_Cod§PART_COD§Validita_inizio</c>)
    /// directly, because <see cref="KeysetTipoDato"/> does not support datetime keys.
    /// Delegates to <see cref="IMappingAppezzamentiParticelleOrogelDAL"/> for the
    /// three-table INNER JOIN query execution.
    /// </summary>
    public class OrogelEstrazioniDatiMappingAppezzamentiParticelleService
        : BaseServiceAnagrafeBIZ,
            IOrogelEstrazioniDatiMappingAppezzamentiParticelleService
    {
        private const int MaxPageSize = 1000;
        private const char KeySeparator = '§';
        private const int ExpectedKeyParts = 3;

        /// <summary>
        /// DS09-BL §Paginazione null fallback: SQL Server minimum safe datetime
        /// used as the first-page value for the <c>@LastValiditaInizio</c> parameter
        /// when the cursor is absent or the third part is empty.
        /// </summary>
        private static readonly DateTime SqlMinDateTime = new DateTime(1753, 1, 1);

        /// <summary>
        /// DS09-BL: Initializes <see cref="OrogelEstrazioniDatiMappingAppezzamentiParticelleService"/>.
        /// </summary>
        private readonly IMappingAppezzamentiParticelleOrogelDAL _mappingOrogelDAL;

        public OrogelEstrazioniDatiMappingAppezzamentiParticelleService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer,
            IMappingAppezzamentiParticelleOrogelDAL mappingOrogelDAL
        )
            : base(provider, localizer)
        {
            _mappingOrogelDAL = mappingOrogelDAL;
        }

        /// <inheritdoc/>
        public async Task<EstrazioniDatiMappingAppezzamentiParticelleResult> EstraiMappingAppezzamentiParticelleAsync(
            IReadOnlyList<string> codiciAzienda,
            int pageSize,
            string? nextKey,
            AgronicaCoreParametriTriple objParametriTriple
        )
        {
            // DS09-BL §Limite massimo pageSize (hardcoded, non bypassabile da client)
            if (pageSize > MaxPageSize)
                throw new PaginationSizeExceededException(pageSize);

            // DS09-BL §Paginazione: decode the 3-key cursor manually.
            // DS04-BL is NOT used because it does not support DateTime keys.
            (int lastProgettoCod, string lastPartCod, DateTime lastValiditaInizio) = DecodeCursor(
                nextKey
            );

            var parSqlPaginazione = new Dictionary<string, object>
            {
                ["@LastProgettoCod"] = lastProgettoCod,
                ["@LastPartCod"] = lastPartCod,
                ["@LastValiditaInizio"] = lastValiditaInizio,
            };

            DataTable dt;
            try
            {
                dt = await _mappingOrogelDAL.LeggiMappingAppezzamentiParticellePaginatoAsync(
                    pageSize,
                    parSqlPaginazione,
                    codiciAzienda,
                    objParametriTriple.ObjParametriServer
                );
            }
            catch (Exception ex) when (OrogelBizHelper.IsConnectionError(ex))
            {
                throw new DatabaseConnectionException(
                    "Impossibile connettersi al database archivio per l'estrazione mapping appezzamenti-particelle.",
                    ex
                );
            }
            catch (Exception ex)
            {
                throw new QueryExecutionException(
                    "Errore durante l'esecuzione della query di estrazione mapping appezzamenti-particelle.",
                    ex
                );
            }

            if (dt.Rows.Count == 0)
                return new EstrazioniDatiMappingAppezzamentiParticelleResult(
                    new KeysetMetadatiPaginazione(pageSize, null),
                    Array.Empty<MappingAppezzamentiParticelleItem>()
                );

            List<MappingAppezzamentiParticelleItem> data = MapToItems(dt);

            // DS09-BL §Costruzione Metadati: 3-key cursor from last record.
            KeysetMetadatiPaginazione metadata = BuildMetadata(
                pageSize,
                dt.Rows.Count,
                dt.Rows[dt.Rows.Count - 1]
            );

            return new EstrazioniDatiMappingAppezzamentiParticelleResult(metadata, data);
        }

        /// <remarks>
        /// DS09-BL §Paginazione: Decodes the cursor string <c>LastProgettoCod§LastPartCod§LastValiditaInizio</c>.
        /// Returns first-page defaults when <paramref name="nextKey"/> is null or empty:
        /// <c>-1</c> for integer, <c>""</c> for string, <see cref="SqlMinDateTime"/> for datetime.
        /// </remarks>
        private static (
            int lastProgettoCod,
            string lastPartCod,
            DateTime lastValiditaInizio
        ) DecodeCursor(string? nextKey)
        {
            if (string.IsNullOrEmpty(nextKey))
                return (-1, string.Empty, SqlMinDateTime);

            string[] parts = nextKey.Split(KeySeparator);
            if (parts.Length != ExpectedKeyParts)
                throw new InvalidNextKeyException(nextKey, ExpectedKeyParts, parts.Length);

            if (!int.TryParse(parts[0], out int progettoCod))
                throw new InvalidNextKeyValuesException(
                    nextKey,
                    "Progetto_Cod",
                    parts[0],
                    "integer"
                );

            string partCod = parts[1]; // string, no parsing needed

            DateTime validitaInizio;
            if (string.IsNullOrEmpty(parts[2]))
                validitaInizio = SqlMinDateTime;
            else if (
                !DateTime.TryParse(
                    parts[2],
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out validitaInizio
                )
            )
                throw new InvalidNextKeyValuesException(
                    nextKey,
                    "Validita_inizio",
                    parts[2],
                    "datetime"
                );

            return (progettoCod, partCod, validitaInizio);
        }

        /// <remarks>
        /// DS09-BL §Costruzione Metadati: Builds the next 3-key cursor from the last DataRow.
        /// Returns <c>nextKey = null</c> when <paramref name="count"/> &lt; <paramref name="pageSize"/>.
        /// <c>Validita_inizio</c> is serialized using ISO 8601 round-trip format (<c>"O"</c>)
        /// so that <see cref="DecodeCursor"/> can round-trip it without loss.
        /// </remarks>
        private static KeysetMetadatiPaginazione BuildMetadata(
            int pageSize,
            int count,
            DataRow lastRow
        )
        {
            if (count < pageSize)
                return new KeysetMetadatiPaginazione(pageSize, null);

            string progettoCod = lastRow["codice_esercizio"]?.ToString() ?? "-1";
            string partCod = lastRow["particellaCodice"]?.ToString() ?? string.Empty;
            string validitaInizio = Convert
                .ToDateTime(lastRow["validitaInizioRelazione"])
                .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            return new KeysetMetadatiPaginazione(
                pageSize,
                $"{progettoCod}{KeySeparator}{partCod}{KeySeparator}{validitaInizio}"
            );
        }

        /// <remarks>DS09-BL §Output data[]: Maps DataTable rows to <see cref="MappingAppezzamentiParticelleItem"/> records.</remarks>
        private static List<MappingAppezzamentiParticelleItem> MapToItems(DataTable dt)
        {
            var items = new List<MappingAppezzamentiParticelleItem>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                items.Add(
                    new MappingAppezzamentiParticelleItem(
                        CodiceEsercizio: row["codice_esercizio"]?.ToString() ?? string.Empty,
                        ParticellaCodice: row["particellaCodice"]?.ToString() ?? string.Empty,
                        ValiditaInizioRelazione: Convert.ToDateTime(row["validitaInizioRelazione"]),
                        CodiceAzienda: row["codice_azienda"]?.ToString() ?? string.Empty,
                        CodiceCentro: row["codice_centro"]?.ToString() ?? string.Empty,
                        CodiceAppezzamento: row["codice_appezzamento"]?.ToString() ?? string.Empty,
                        CodiceImpianto: row["codice_impianto"]?.ToString() ?? string.Empty,
                        Provincia: row["provincia"]?.ToString() ?? string.Empty,
                        Comune: row["comune"]?.ToString() ?? string.Empty,
                        Sezione: OrogelBizHelper.NullIfEmpty(row["sezione"]?.ToString()),
                        Foglio: row["foglio"]?.ToString() ?? string.Empty,
                        Particella: row["particella"]?.ToString() ?? string.Empty,
                        Subalterno: OrogelBizHelper.NullIfEmpty(row["subalterno"]?.ToString()),
                        CodiceIstat: row["codice_istat"]?.ToString() ?? string.Empty,
                        ValiditaFineRelazione: row["validitaFineRelazione"] is DBNull
                            ? null
                            : Convert.ToDateTime(row["validitaFineRelazione"]),
                        SuperficieAttribuitaHa: row["superficie_attribuita_ha"] is DBNull
                            ? 0m
                            : Convert.ToDecimal(row["superficie_attribuita_ha"])
                    )
                );
            }
            return items;
        }
    }
}
