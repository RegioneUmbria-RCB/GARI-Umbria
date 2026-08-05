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
    /// DS07-BL: Orchestrates the extraction of paginated particella catastale data for FS003.
    /// Coordinates DS04-BL (single-key keyset cursor) and
    /// <see cref="IParticellaCatastaleOrogelDAL"/> (INNER JOIN query execution).
    /// </summary>
    public class OrogelEstrazioniDatiParticelleCatastaliService
        : BaseServiceAnagrafeBIZ,
            IOrogelEstrazioniDatiParticelleCatastaliService
    {
        private readonly IOrogelPaginazioneKeysetService _paginazioneKeysetService;
        private readonly IParticellaCatastaleOrogelDAL _particellaCatastaleOrogelDAL;

        /// <summary>
        /// DS07-BL §Ordine statico: Single integer key <c>ID</c> maps to cursor parameter <c>@LastId</c>.
        /// The fallback value for null/first page is -1 (DS04-BL integer fallback).
        /// </summary>
        private static readonly IReadOnlyList<KeysetColonnaDefinizione> ColonneOrdinamento = new[]
        {
            new KeysetColonnaDefinizione("ID", KeysetTipoDato.Integer),
        };

        /// <summary>
        /// DS07-BL: Initializes <see cref="OrogelEstrazioniDatiParticelleCatastaliService"/>.
        /// </summary>
        public OrogelEstrazioniDatiParticelleCatastaliService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer,
            IOrogelPaginazioneKeysetService paginazioneKeysetService,
            IParticellaCatastaleOrogelDAL particellaCatastaleOrogelDAL
        )
            : base(provider, localizer)
        {
            _paginazioneKeysetService = paginazioneKeysetService;
            _particellaCatastaleOrogelDAL = particellaCatastaleOrogelDAL;
        }

        /// <inheritdoc/>
        public async Task<EstrazioniDatiParticelleCatastaliResult> EstraiParticelleCatastaliAsync(
            IReadOnlyList<string> codiciAzienda,
            int pageSize,
            string? nextKey,
            AgronicaCoreParametriTriple objParametriTriple
        )
        {
            // DS07-BL §Paginazione: decode single-key cursor via DS04-BL.
            // FiltroWherePaginazione is not forwarded — DAL hardcodes ixp.ID > @LastId.
            KeysetFiltroResult filtro = _paginazioneKeysetService.DecodificaNextKey(
                nextKey,
                pageSize,
                ColonneOrdinamento
            );

            DataTable dt;
            try
            {
                dt = await _particellaCatastaleOrogelDAL.LeggiParticelleCatastaliPaginateAsync(
                    pageSize,
                    filtro.ParSqlPaginazione,
                    codiciAzienda,
                    objParametriTriple.ObjParametriServer
                );
            }
            catch (Exception ex) when (OrogelBizHelper.IsConnectionError(ex))
            {
                throw new DatabaseConnectionException(
                    "Impossibile connettersi al database archivio per l'estrazione particelle catastali.",
                    ex
                );
            }
            catch (Exception ex)
            {
                throw new QueryExecutionException(
                    "Errore durante l'esecuzione della query di estrazione particelle catastali.",
                    ex
                );
            }

            // DS07-BL §Se nessun record trovato: return empty data with null nextKey
            if (dt.Rows.Count == 0)
                return new EstrazioniDatiParticelleCatastaliResult(
                    new KeysetMetadatiPaginazione(pageSize, null),
                    Array.Empty<ParticellaCatastaleItem>()
                );

            List<ParticellaCatastaleItem> data = MapToItems(dt);

            // DS07-BL §Costruzione Metadati: last record ID as the next cursor value.
            DataRow ultimaRiga = dt.Rows[dt.Rows.Count - 1];
            var ultimoRecord = new Dictionary<string, object?> { ["ID"] = ultimaRiga["ID"] };

            KeysetMetadatiPaginazione metadata = _paginazioneKeysetService.GeneraMetadati(
                pageSize,
                dt.Rows.Count,
                ColonneOrdinamento,
                ultimoRecord
            );

            return new EstrazioniDatiParticelleCatastaliResult(metadata, data);
        }

        /// <remarks>
        /// DS07-BL §Output data[]: Maps DataTable rows to <see cref="ParticellaCatastaleItem"/> records.
        /// Each row in <c>ImpreseXParticelle</c> represents one conduzione period, so a physical
        /// parcel may produce multiple items.
        /// </remarks>
        private static List<ParticellaCatastaleItem> MapToItems(DataTable dt)
        {
            var items = new List<ParticellaCatastaleItem>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                DateTime? dataFine =
                    row["data_fine_conduzione"] is DBNull
                        ? null
                        : Convert.ToDateTime(row["data_fine_conduzione"]);

                decimal? superficieCondotta =
                    row["superficie_condotta_ha"] is DBNull
                        ? null
                        : Convert.ToDecimal(row["superficie_condotta_ha"]);

                string? sezione = row["sezione"]?.ToString();
                if (string.IsNullOrEmpty(sezione))
                    sezione = null;

                string? subalterno = row["subalterno"]?.ToString();
                if (string.IsNullOrEmpty(subalterno))
                    subalterno = null;

                items.Add(
                    new ParticellaCatastaleItem(
                        CodiceAzienda: row["codice_azienda"]?.ToString() ?? string.Empty,
                        CodiceCentro: row["codice_centro"]?.ToString() ?? string.Empty,
                        CodiceIstat: row["codice_istat"]?.ToString() ?? string.Empty,
                        Foglio: row["foglio"]?.ToString() ?? string.Empty,
                        Sezione: sezione,
                        Particella: row["particella"]?.ToString() ?? string.Empty,
                        Subalterno: subalterno,
                        SuperficieCatastaleHa: Convert.ToDecimal(row["superficie_catastale_ha"]),
                        SuperficieCondottaHa: superficieCondotta,
                        DataInizioConduzione: Convert.ToDateTime(row["data_inizio_conduzione"]),
                        DataFineConduzione: dataFine,
                        CodiceConduzione: Convert.ToInt32(row["codice_conduzione"]),
                        DescrizioneConduzione: row["descrizione_conduzione"]?.ToString()
                            ?? string.Empty
                    )
                );
            }
            return items;
        }
    }
}
