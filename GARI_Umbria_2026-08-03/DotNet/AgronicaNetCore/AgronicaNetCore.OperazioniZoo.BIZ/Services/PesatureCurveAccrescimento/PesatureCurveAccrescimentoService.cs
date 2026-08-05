using AgronicaCoreDTOStd.InData.Zoo;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.OperazioniZoo.BIZ.Resources;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesatureCurveAccrescimento;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiProfili;
using OutData.Zoo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesatureCurveAccrescimento
{
    /// <summary>
    /// Servizio BIZ per il calcolo e il recupero delle metriche della curva di accrescimento bovini.
    /// Orchestrate la validazione degli input, la verifica dei profili utente e la chiamata al DAL.
    /// Vedere DS06-API: Endpoint Query Metriche Curva Accrescimento.
    /// </summary>
    public class PesatureCurveAccrescimentoService : BaseServiceOperazioniZooBIZ, IPesatureCurveAccrescimentoService
    {
        private readonly IPesatureCurveAccrescimentoDAL _dal;
        private readonly IUtentiProfili _utentiProfili;

        public PesatureCurveAccrescimentoService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _dal          = provider.GetRequiredService<IPesatureCurveAccrescimentoDAL>();
            _utentiProfili = provider.GetRequiredService<IUtentiProfili>();
        }

        /// <inheritdoc/>
        public async Task<OutData.Zoo.PesatureCurveAccrescimentoResult> LeggiPesatureCurveAccrescimentoAsync(
            PesatureCurveAccrescimentoQueryParams queryParams,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            try
            {
                bool visibilitaTotale = await _utentiProfili.VisibilitaTotaleGiasOnline(
                    objParametriUtenti, objParametriServer);

                string pivaJwt     = queryParams.Piva;
                string usernameJwt  = objParametriServer.UtenteUsername;

                (IReadOnlyList<OutData.Zoo.PesaturaCurvaAccrescimentoRow> rows, int totalCount) =
                    await _dal.LeggiPesatureCurveAccrescimentoAsync(
                        queryParams, pivaJwt, usernameJwt, visibilitaTotale, objParametriServer);

                int totalPages = queryParams.Limit > 0
                    ? (int)Math.Ceiling(totalCount / (double)queryParams.Limit)
                    : 0;

                var stallaKeysFiltro = ParseFilterKeys(queryParams.StallaPKeys);
                var razzaKeysFiltro  = ParseFilterKeys(queryParams.RazzaKeys);

                int distinctAnimals = rows.Select(r => r.Lid).Distinct().Count();

                return new OutData.Zoo.PesatureCurveAccrescimentoResult
                {
                    Metadata = new OutData.Zoo.PesatureCurveAccrescimentoMetadata
                    {
                        QueryTimestamp    = DateTime.UtcNow,
                        StallaKeyFiltro   = stallaKeysFiltro,
                        RazzaKeyFiltro    = razzaKeysFiltro,
                        DateRange = new OutData.Zoo.PesatureDateRange
                        {
                            From = queryParams.DateFrom,
                            To   = queryParams.DateTo
                        },
                        KgPerDay          = queryParams.KgPerDay,
                        AlertThresholdPct = queryParams.AlertThresholdPct,
                        TotalAnimals      = distinctAnimals,
                        TotalWeighings    = totalCount,
                        Pagination = new OutData.Zoo.PesaturePagination
                        {
                            Page       = queryParams.Page,
                            Limit      = queryParams.Limit,
                            TotalPages = totalPages,
                            HasNext    = queryParams.Page < totalPages
                        }
                    },
                    Data = rows
                };
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        /// <summary>
        /// Converte una stringa di chiavi separate da virgola in una lista di stringhe.
        /// </summary>
        private static IReadOnlyList<string> ParseFilterKeys(string? keys)
        {
            if (string.IsNullOrWhiteSpace(keys))
                return Array.Empty<string>();

            return keys
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }

        /// <inheritdoc/>
        public async Task<OutData.Zoo.PesatureCurveAccrescimentoAggregatedResult> LeggiPesatureCurveAccrescimentoAggregatedAsync(
            PesatureCurveAccrescimentoQueryParams queryParams,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            try
            {
                bool visibilitaTotale = await _utentiProfili.VisibilitaTotaleGiasOnline(
                    objParametriUtenti, objParametriServer);

                string pivaJwt    = queryParams.Piva;
                string usernameJwt = objParametriServer.UtenteUsername;

                // Fetch all rows (no pagination) — aggregation happens in-memory.
                var fetchParams = new PesatureCurveAccrescimentoQueryParams
                {
                    Piva              = queryParams.Piva,
                    StallaPKeys       = queryParams.StallaPKeys,
                    RazzaKeys         = queryParams.RazzaKeys,
                    DateFrom          = queryParams.DateFrom,
                    DateTo            = queryParams.DateTo,
                    KgPerDay          = queryParams.KgPerDay,
                    AlertThresholdPct = queryParams.AlertThresholdPct,
                    Page              = 1,
                    Limit             = 10000,
                    SortBy            = "lid",
                    SortOrder         = "ASC"
                };

                (IReadOnlyList<OutData.Zoo.PesaturaCurvaAccrescimentoRow> rows, _) =
                    await _dal.LeggiPesatureCurveAccrescimentoAsync(
                        fetchParams, pivaJwt, usernameJwt, visibilitaTotale, objParametriServer);

                IReadOnlyList<OutData.Zoo.PesaturaCurvaAccrescimentoAggregatedRow> aggregatedRows =
                    AggregateRows(rows, queryParams.AggregationLevel);

                var stallaKeysFiltro = ParseFilterKeys(queryParams.StallaPKeys);
                var razzaKeysFiltro  = ParseFilterKeys(queryParams.RazzaKeys);

                return new OutData.Zoo.PesatureCurveAccrescimentoAggregatedResult
                {
                    Metadata = new OutData.Zoo.PesatureCurveAccrescimentoMetadata
                    {
                        QueryTimestamp    = DateTime.UtcNow,
                        StallaKeyFiltro   = stallaKeysFiltro,
                        RazzaKeyFiltro    = razzaKeysFiltro,
                        DateRange = new OutData.Zoo.PesatureDateRange
                        {
                            From = queryParams.DateFrom,
                            To   = queryParams.DateTo
                        },
                        KgPerDay          = queryParams.KgPerDay,
                        AlertThresholdPct = queryParams.AlertThresholdPct,
                        TotalAnimals      = rows.Select(r => r.Lid).Distinct().Count(),
                        TotalWeighings    = rows.Count,
                        Pagination = new OutData.Zoo.PesaturePagination
                        {
                            Page       = 1,
                            Limit      = aggregatedRows.Count,
                            TotalPages = 1,
                            HasNext    = false
                        }
                    },
                    Data = aggregatedRows
                };
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        /// <summary>
        /// Aggrega le righe di pesatura in memoria per livello di aggregazione specificato.
        /// </summary>
        private static IReadOnlyList<OutData.Zoo.PesaturaCurvaAccrescimentoAggregatedRow> AggregateRows(
            IReadOnlyList<OutData.Zoo.PesaturaCurvaAccrescimentoRow> rows,
            string aggregationLevel)
        {
            IEnumerable<IGrouping<string, OutData.Zoo.PesaturaCurvaAccrescimentoRow>> groups =
                aggregationLevel.ToLowerInvariant() switch
                {
                    "razza_key"  => rows.GroupBy(r => r.RazzaKey),
                    "stalla_key" => rows.GroupBy(r => r.StallaKey),
                    _            => rows.GroupBy(r => r.Lid)   // default: animal
                };

            return groups.Select(g =>
            {
                string gruppoDes = aggregationLevel.ToLowerInvariant() switch
                {
                    "razza_key"  => g.First().RazzaDes,
                    "stalla_key" => g.First().StaDes,
                    _            => g.Key  // animal: use LID as description
                };

                int distinctAnimali = aggregationLevel.ToLowerInvariant() == "animal"
                    ? 1
                    : g.Select(r => r.Lid).Distinct().Count();

                return new OutData.Zoo.PesaturaCurvaAccrescimentoAggregatedRow
                {
                    GruppoKey            = g.Key,
                    GruppoDes            = gruppoDes,
                    NumeroAnimali        = distinctAnimali,
                    NumeroPesate         = g.Count(),
                    ScostamentoMedioPct  = Math.Round(g.Average(r => r.ScostamentoPct), 2),
                    ScostamentoMaxPct    = Math.Round(g.Max(r => r.ScostamentoPct), 2),
                    ScostamentoMinPct    = Math.Round(g.Min(r => r.ScostamentoPct), 2),
                    AlertCount           = g.Count(r => r.AlertFlag)
                };
            }).ToList();
        }
    }
}