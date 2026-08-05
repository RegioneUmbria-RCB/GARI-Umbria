using System.Data;
using AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.CodiciAnagrafe;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Orogel;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    /// <summary>
    /// DS06-BL: Orchestrates the extraction of paginated centro aziendale data for FS002.
    /// Coordinates DS04-BL (keyset pagination cursor decoding/encoding) and
    /// <see cref="ICentroAziendaleOrogelDAL"/> (join query execution).
    /// </summary>
    public class OrogelEstrazioniDatiCentriAziendaliService
        : BaseServiceAnagrafeBIZ,
            IOrogelEstrazioniDatiCentriAziendaliService
    {
        private readonly IOrogelPaginazioneKeysetService _paginazioneKeysetService;
        private readonly ICentroAziendaleOrogelDAL _centroAziendaleOrogelDAL;
        private readonly ICodiciAnagrafe _codiciAnagrafe;

        /// <summary>
        /// DS06-BL §Ordine statico: 2-key keyset ordering columns for FS002 (PIVA string, sa_cod integer).
        /// Column names match the <c>ultimoRecord</c> dictionary keys used in <see cref="GeneraMetadati"/>.
        /// </summary>
        private static readonly IReadOnlyList<KeysetColonnaDefinizione> ColonneOrdinamento = new[]
        {
            new KeysetColonnaDefinizione("PIVA", KeysetTipoDato.String),
            new KeysetColonnaDefinizione("sa_cod", KeysetTipoDato.Integer),
        };

        /// <summary>
        /// DS06-BL: Initializes <see cref="OrogelEstrazioniDatiCentriAziendaliService"/>.
        /// </summary>
        public OrogelEstrazioniDatiCentriAziendaliService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer,
            IOrogelPaginazioneKeysetService paginazioneKeysetService,
            ICentroAziendaleOrogelDAL centroAziendaleOrogelDAL,
            ICodiciAnagrafe codiciAnagrafe
        )
            : base(provider, localizer)
        {
            _paginazioneKeysetService = paginazioneKeysetService;
            _centroAziendaleOrogelDAL = centroAziendaleOrogelDAL;
            _codiciAnagrafe = codiciAnagrafe;
        }

        /// <inheritdoc/>
        public async Task<EstrazioniDatiCentriAziendaliResult> EstraiCentriAziendaliAsync(
            IReadOnlyList<string> codiciAzienda,
            int pageSize,
            string? nextKey,
            AgronicaCoreParametriTriple objParametriTriple
        )
        {
            // DS06-BL §Paginazione: decode cursor via DS04-BL.
            // FiltroWherePaginazione is NOT forwarded — the DAL builds the table-qualified WHERE internally
            // to resolve PIVA/sa_cod ambiguity across the multi-table LEFT JOIN.
            KeysetFiltroResult filtro = _paginazioneKeysetService.DecodificaNextKey(
                nextKey,
                pageSize,
                ColonneOrdinamento
            );

            DataTable dt;
            try
            {
                await OpenConnectionAsync(
                    objParametriTriple.ObjParametriServer,
                    OpenTransaction: false,
                    IsolationLevel.ReadUncommitted
                );
                try
                {
                    dt = await _centroAziendaleOrogelDAL.LeggiCentriAziendaliPaginatiAsync(
                        pageSize,
                        filtro.ParSqlPaginazione,
                        codiciAzienda,
                        objParametriTriple.ObjParametriServer
                    );
                }
                catch (Exception ex) when (OrogelBizHelper.IsConnectionError(ex))
                {
                    throw new DatabaseConnectionException(
                        "Impossibile connettersi al database archivio per l'estrazione centri aziendali.",
                        ex
                    );
                }
                catch (Exception ex)
                {
                    throw new QueryExecutionException(
                        "Errore durante l'esecuzione della query di estrazione centri aziendali.",
                        ex
                    );
                }

                // DS06-BL §Se nessun record trovato: return empty data with null nextKey
                if (dt.Rows.Count == 0)
                    return new EstrazioniDatiCentriAziendaliResult(
                        new KeysetMetadatiPaginazione(pageSize, null),
                        Array.Empty<CentroAziendaleItem>()
                    );

                DataTable dtDestinazioni = await _codiciAnagrafe.LeggiCodiciCentroAsync(
                    dt.Rows.Cast<DataRow>()
                        .Select(r =>
                            (
                                r["codice_azienda"]?.ToString() ?? string.Empty,
                                Convert.ToInt32(r["codice_centro"])
                            )
                        )
                        .Distinct()
                        .ToList(),
                    new List<int> { (int)Enum_CodiciAnagrafe.Codice_Destinazione },
                    objParametriTriple.ObjParametriServer,
                    escludiCodiciCliente: true
                );

                var destinazioni = new Dictionary<(string, int), string>();
                foreach (DataRow codRow in dtDestinazioni.Rows)
                {
                    var key = (
                        codRow["PIVA"]?.ToString() ?? string.Empty,
                        Convert.ToInt32(codRow["sa_cod"])
                    );
                    destinazioni[key] = codRow["val_cod"]?.ToString() ?? string.Empty;
                }

                List<CentroAziendaleItem> data = MapToItems(dt, destinazioni);

                // DS06-BL §Costruzione Metadati: last record (PIVA, sa_cod) for next cursor.
                // Keys match ColonneOrdinamento NomeColonna so DS04-BL can extract values correctly.
                DataRow ultimaRiga = dt.Rows[dt.Rows.Count - 1];
                var ultimoRecord = new Dictionary<string, object?>
                {
                    ["PIVA"] = ultimaRiga["codice_azienda"],
                    ["sa_cod"] = ultimaRiga["codice_centro"],
                };

                KeysetMetadatiPaginazione metadata = _paginazioneKeysetService.GeneraMetadati(
                    pageSize,
                    dt.Rows.Count,
                    ColonneOrdinamento,
                    ultimoRecord
                );

                return new EstrazioniDatiCentriAziendaliResult(metadata, data);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriTriple.ObjParametriServer, ex);
                CloseTransaction(objParametriTriple.ObjParametriServer, Rollback: true);
                throw;
            }
            finally
            {
                CloseConnection(objParametriTriple.ObjParametriServer);
            }
        }

        /// <remarks>
        /// DS06-BL §Output data[]: Maps DataTable rows to <see cref="CentroAziendaleItem"/> records.
        /// Nullable string fields are set to null when ISNULL fallback returns empty string or DBNull.
        /// </remarks>
        private static List<CentroAziendaleItem> MapToItems(
            DataTable dt,
            Dictionary<(string, int), string> destinazioni
        )
        {
            var items = new List<CentroAziendaleItem>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                DateTime? dataFine =
                    row["data_fine"] is DBNull ? null : Convert.ToDateTime(row["data_fine"]);

                string piva = row["codice_azienda"]?.ToString() ?? string.Empty;
                int saCod = Convert.ToInt32(row["codice_centro"]);
                destinazioni.TryGetValue((piva, saCod), out string? destinazione);
                if (string.IsNullOrEmpty(destinazione))
                    destinazione = null;

                string? frazione = row["frazione"]?.ToString();
                if (string.IsNullOrEmpty(frazione))
                    frazione = null;

                string? localita = row["localita"]?.ToString();
                if (string.IsNullOrEmpty(localita))
                    localita = null;

                items.Add(
                    new CentroAziendaleItem(
                        CodiceAzienda: row["codice_azienda"]?.ToString() ?? string.Empty,
                        CodiceCentro: row["codice_centro"]?.ToString() ?? string.Empty,
                        DataInizio: Convert.ToDateTime(row["data_inizio"]),
                        DataFine: dataFine,
                        DestinazioneProdotto: destinazione,
                        Indirizzo: row["indirizzo"]?.ToString() ?? string.Empty,
                        Frazione: frazione,
                        Cap: row["cap"]?.ToString() ?? string.Empty,
                        Localita: localita,
                        Provincia: row["provincia"]?.ToString() ?? string.Empty,
                        CodiceIstat: row["codice_istat"]?.ToString() ?? string.Empty
                    )
                );
            }
            return items;
        }
    }
}
