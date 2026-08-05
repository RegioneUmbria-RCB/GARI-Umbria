using System.Data;
using AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Orogel;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    /// <summary>
    /// DS05-BL: Orchestrates the extraction of paginated azienda anagrafica data for FS001.
    /// Coordinates DS04-BL (keyset pagination) and <see cref="IImpresaOrogelDAL"/> (query execution).
    /// </summary>
    public class OrogelEstrazioniDatiAziendeService
        : BaseServiceAnagrafeBIZ,
            IOrogelEstrazioniDatiAziendeService
    {
        private readonly IOrogelPaginazioneKeysetService _paginazioneKeysetService;
        private readonly IImpresaOrogelDAL _impresaOrogelDAL;

        /// <summary>DS05-BL §Ordine statico: Keyset ordering column definition for FS001 (single key: PIVA string).</summary>
        private static readonly IReadOnlyList<KeysetColonnaDefinizione> ColonneOrdinamento = new[]
        {
            new KeysetColonnaDefinizione("PIVA", KeysetTipoDato.String),
        };

        /// <summary>
        /// DS05-BL: Initializes <see cref="OrogelEstrazioniDatiAziendeService"/>.
        /// </summary>
        public OrogelEstrazioniDatiAziendeService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer,
            IOrogelPaginazioneKeysetService paginazioneKeysetService,
            IImpresaOrogelDAL impresaOrogelDAL
        )
            : base(provider, localizer)
        {
            _paginazioneKeysetService = paginazioneKeysetService;
            _impresaOrogelDAL = impresaOrogelDAL;
        }

        /// <inheritdoc/>
        public async Task<EstrazioniDatiAziendeResult> EstraiAziendeAsync(
            IReadOnlyList<string> codiciAzienda,
            int pageSize,
            string? nextKey,
            AgronicaCoreParametriTriple objParametriTriple
        )
        {
            // DS05-BL §Paginazione: decode cursor via DS04-BL
            KeysetFiltroResult filtro = _paginazioneKeysetService.DecodificaNextKey(
                nextKey,
                pageSize,
                ColonneOrdinamento
            );

            DataTable dt;
            try
            {
                dt = await _impresaOrogelDAL.LeggiImpresePaginateAsync(
                    pageSize,
                    filtro.FiltroWherePaginazione,
                    filtro.ParSqlPaginazione,
                    codiciAzienda,
                    objParametriTriple.ObjParametriServer
                );
            }
            catch (Exception ex) when (OrogelBizHelper.IsConnectionError(ex))
            {
                throw new DatabaseConnectionException(
                    "Impossibile connettersi al database archivio per l'estrazione aziende.",
                    ex
                );
            }
            catch (Exception ex)
            {
                throw new QueryExecutionException(
                    "Errore durante l'esecuzione della query di estrazione aziende.",
                    ex
                );
            }

            // DS05-BL §Se nessun record trovato: return empty data with null nextKey
            if (dt.Rows.Count == 0)
                return new EstrazioniDatiAziendeResult(
                    new KeysetMetadatiPaginazione(pageSize, null),
                    Array.Empty<AziendeItem>()
                );

            List<AziendeItem> data = MapToItems(dt);

            // DS05-BL §Costruzione Metadati: last record PIVA for next cursor
            DataRow ultimaRiga = dt.Rows[dt.Rows.Count - 1];
            var ultimoRecord = new Dictionary<string, object?>
            {
                ["PIVA"] = ultimaRiga["codice_azienda"],
            };

            KeysetMetadatiPaginazione metadata = _paginazioneKeysetService.GeneraMetadati(
                pageSize,
                dt.Rows.Count,
                ColonneOrdinamento,
                ultimoRecord
            );

            return new EstrazioniDatiAziendeResult(metadata, data);
        }

        /// <remarks>DS05-BL §Output data[]: Maps DataTable rows to <see cref="AziendeItem"/> records.</remarks>
        private static List<AziendeItem> MapToItems(DataTable dt)
        {
            var items = new List<AziendeItem>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                DateTime? dataFine =
                    row["data_fine"] is DBNull ? null : Convert.ToDateTime(row["data_fine"]);
                items.Add(
                    new AziendeItem(
                        CodiceAzienda: row["codice_azienda"]?.ToString() ?? string.Empty,
                        RagioneSociale: row["ragione_sociale"]?.ToString() ?? string.Empty,
                        DataInizio: Convert.ToDateTime(row["data_inizio"]),
                        DataFine: dataFine
                    )
                );
            }
            return items;
        }
    }
}
