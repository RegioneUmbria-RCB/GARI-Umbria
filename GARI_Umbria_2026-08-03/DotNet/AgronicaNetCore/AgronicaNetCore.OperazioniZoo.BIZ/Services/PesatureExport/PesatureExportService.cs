using AgronicaCoreDTOStd.InData.Zoo;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.OperazioniZoo.BIZ.Resources;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesatureCurveAccrescimento;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiProfili;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OutData.Zoo;
using System.Text;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesatureExport
{
    /// <summary>
    /// Implementazione del servizio di export pesate curva di accrescimento.
    /// Riutilizza <see cref="IPesatureCurveAccrescimentoDAL"/> per il recupero dati senza paginazione,
    /// genera il file in memoria (CSV o XLS) e scrive l'audit log.
    /// Vedere DS07-API: Endpoint Export Pesate Curva Accrescimento.
    /// </summary>
    public class PesatureExportService : BaseServiceOperazioniZooBIZ, IPesatureExportService
    {
        /// <summary>
        /// Numero massimo di righe esportabili in un singolo file.
        /// Vedere DS07-API, sezione "Sicurezza / File Size Limits".
        /// </summary>
        private const int MaxExportRows = 100_000;

        /// <summary>Limite dimensione file in byte: 50 MB.</summary>
        private const long MaxFileSizeBytes = 50L * 1024 * 1024;

        private readonly IPesatureCurveAccrescimentoDAL _dal;
        private readonly IUtentiProfili _utentiProfili;
        private readonly ILogger<PesatureExportService> _logger;

        public PesatureExportService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _dal           = provider.GetRequiredService<IPesatureCurveAccrescimentoDAL>();
            _utentiProfili = provider.GetRequiredService<IUtentiProfili>();
            _logger        = provider.GetRequiredService<ILogger<PesatureExportService>>();
        }

        /// <inheritdoc/>
        public async Task<PesatureExportFileResult> ExportAsync(
            PesatureExportQueryParams queryParams,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            string pivaJwt    = queryParams.Piva;
            string usernameJwt = objParametriServer.UtenteUsername;

            // ── Visibilità utente ──────────────────────────────────────────────────────────────────
            bool visibilitaTotale = await _utentiProfili.VisibilitaTotaleGiasOnline(objParametriUtenti, objParametriServer);

            // ── Recupero dati (nessuna paginazione — tutte le righe fino al limite di sicurezza) ──
            var dalParams = new PesatureCurveAccrescimentoQueryParams
            {
                Piva              = queryParams.Piva,
                StallaPKeys       = queryParams.StallaPKeys,
                RazzaKeys         = queryParams.RazzaKeys,
                DateFrom          = queryParams.DateFrom,
                DateTo            = queryParams.DateTo,
                KgPerDay          = queryParams.KgPerDay,
                AlertThresholdPct = queryParams.AlertThresholdPct,
                SortBy            = queryParams.SortBy,
                SortOrder         = queryParams.SortOrder,
                Page              = 1,
                Limit             = MaxExportRows
            };

            (IReadOnlyList<PesaturaCurvaAccrescimentoRow> rows, _) =
                await _dal.LeggiPesatureCurveAccrescimentoAsync(
                    dalParams, pivaJwt, usernameJwt, visibilitaTotale, objParametriServer);

            // ── Generazione file ───────────────────────────────────────────────────────────────────
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            byte[] content;
            string contentType;
            string fileName;

            bool isAggregated = queryParams.ViewMode.Equals("aggregated", StringComparison.OrdinalIgnoreCase);

            if (queryParams.Format.Equals("XLS", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fileName    = $"pesate_accrescimento_{queryParams.ViewMode}_{timestamp}.xlsx";
                content     = isAggregated
                    ? BuildXlsAggregated(rows, queryParams)
                    : BuildXlsDetailed(rows, queryParams, pivaJwt, usernameJwt);
            }
            else
            {
                contentType = "text/csv";
                fileName    = $"pesate_accrescimento_{queryParams.ViewMode}_{timestamp}.csv";
                content     = isAggregated
                    ? BuildCsvAggregated(rows, queryParams, pivaJwt, usernameJwt)
                    : BuildCsvDetailed(rows, queryParams, pivaJwt, usernameJwt);
            }

            long fileSizeBytes = content.LongLength;

            // ── Audit log ──────────────────────────────────────────────────────────────────────────
            // Vedere DS07-API, sezione "Logging e Monitoring / Audit Log".
            string logLevel = fileSizeBytes > MaxFileSizeBytes ? "ERROR"
                            : fileSizeBytes > 10L * 1024 * 1024 ? "WARN"
                            : rows.Count == 0 ? "WARN"
                            : "INFO";

            _logger.LogInformation(
                "AUDIT EXPORT_PESATE | status={Status} | username={Username} | stalla={StallaKey} | razza={RazzaKey} | " +
                "date_from={DateFrom:yyyy-MM-dd} | date_to={DateTo:yyyy-MM-dd} | record_count={RecordCount} | " +
                "view_mode={ViewMode} | aggregation_level={AggregationLevel} | format={Format} | " +
                "file_size_bytes={FileSizeBytes} | log_level={LogLevel}",
                fileSizeBytes > MaxFileSizeBytes ? "failure" : "success",
                usernameJwt,
                queryParams.StallaPKeys ?? "*",
                queryParams.RazzaKeys   ?? "*",
                queryParams.DateFrom,
                queryParams.DateTo,
                rows.Count,
                queryParams.ViewMode,
                queryParams.AggregationLevel ?? "-",
                queryParams.Format,
                fileSizeBytes,
                logLevel);

            // ── Limite dimensione file ─────────────────────────────────────────────────────────────
            if (fileSizeBytes > MaxFileSizeBytes)
                throw new InvalidOperationException(
                    $"Il file generato ({fileSizeBytes / 1024 / 1024} MB) supera il limite di 50 MB. " +
                    "Applicare filtri più restrittivi.");

            return new PesatureExportFileResult
            {
                Content       = content,
                ContentType   = contentType,
                FileName      = fileName,
                RecordCount   = rows.Count,
                FileSizeBytes = fileSizeBytes
            };
        }

        // ── CSV Detailed ───────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Genera il CSV in modalità detailed: una riga per pesatura, con header e footer metadata.
        /// Encoding UTF-8 BOM, separatore virgola, CRLF.
        /// Vedere DS07-API, sezione "Formati Supportati - CSV" e "Risposte / 200 - Detailed".
        /// </summary>
        private static byte[] BuildCsvDetailed(
            IReadOnlyList<PesaturaCurvaAccrescimentoRow> rows,
            PesatureExportQueryParams queryParams,
            string pivaJwt,
            string usernameJwt)
        {
            var sb = new StringBuilder();
            sb.Append("LID,Data_Pesata,Peso_kg,Eta_Giorni,Peso_Teorico_kg,Scostamento_%,Alert,Razza_Key,Razza_Des,Stalla_Key,Sta_Des\r\n");

            foreach (var r in rows)
            {
                sb.Append(EscapeCsv(r.Lid)).Append(',');
                sb.Append(r.DataPesata.ToString("yyyy-MM-dd")).Append(',');
                sb.Append(r.PesoKg.ToString("F2")).Append(',');
                sb.Append(r.EtaGiorni).Append(',');
                sb.Append(r.PesoTeorico.ToString("F2")).Append(',');
                sb.Append(r.ScostamentoPct.ToString("F2")).Append(',');
                sb.Append(r.AlertFlag ? "Sì" : "No").Append(',');
                sb.Append(EscapeCsv(r.RazzaKey)).Append(',');
                sb.Append(EscapeCsv(r.RazzaDes)).Append(',');
                sb.Append(EscapeCsv(r.StallaKey)).Append(',');
                sb.Append(EscapeCsv(r.StaDes)).Append("\r\n");
            }

            if (queryParams.IncludeMetadata)
                AppendCsvMetadataDetailed(sb, queryParams, usernameJwt, rows.Count);

            // UTF-8 BOM + body
            byte[] bom  = new byte[] { 0xEF, 0xBB, 0xBF };
            byte[] body = Encoding.UTF8.GetBytes(sb.ToString());
            return bom.Concat(body).ToArray();
        }

        // ── CSV Aggregated ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Genera il CSV in modalità aggregated. L'aggregazione avviene in memoria sui dati detailed.
        /// Vedere DS07-API, sezione "Risposte / 200 - Aggregated".
        /// </summary>
        private static byte[] BuildCsvAggregated(
            IReadOnlyList<PesaturaCurvaAccrescimentoRow> rows,
            PesatureExportQueryParams queryParams,
            string pivaJwt,
            string usernameJwt)
        {
            var aggregated = AggregateRows(rows, queryParams.AggregationLevel ?? "animal");

            var sb = new StringBuilder();
            sb.Append("Gruppo,Gruppo_Des,Numero_Animali,Numero_Pesate,Scostamento_Medio_%,Scostamento_Max_%,Scostamento_Min_%,Alert_Count,Ultimo_Aggiornamento\r\n");

            foreach (var a in aggregated)
            {
                sb.Append(EscapeCsv(a.Gruppo)).Append(',');
                sb.Append(EscapeCsv(a.GruppoDes)).Append(',');
                sb.Append(a.NumeroAnimali).Append(',');
                sb.Append(a.NumeroPesate).Append(',');
                sb.Append(a.ScostamentoMedioPct.ToString("F2")).Append(',');
                sb.Append(a.ScostamentoMaxPct.ToString("F2")).Append(',');
                sb.Append(a.ScostamentoMinPct.ToString("F2")).Append(',');
                sb.Append(a.AlertCount).Append(',');
                sb.Append(a.UltimoAggiornamento.ToString("yyyy-MM-ddTHH:mm:ssZ")).Append("\r\n");
            }

            if (queryParams.IncludeMetadata)
                AppendCsvMetadataAggregated(sb, queryParams, usernameJwt, aggregated.Count);

            byte[] bom  = new byte[] { 0xEF, 0xBB, 0xBF };
            byte[] body = Encoding.UTF8.GetBytes(sb.ToString());
            return bom.Concat(body).ToArray();
        }

        // ── XLS Detailed ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Genera il file Excel (.xlsx) in modalità detailed usando ClosedXML.
        /// Header in grassetto con sfondo colorato, colonne auto-dimensionate.
        /// Vedere DS07-API, sezione "Formati Supportati - XLS" e "Roadmap / Fase 1".
        /// </summary>
        private static byte[] BuildXlsDetailed(
            IReadOnlyList<PesaturaCurvaAccrescimentoRow> rows,
            PesatureExportQueryParams queryParams,
            string pivaJwt,
            string usernameJwt)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.AddWorksheet("Dati");

            // Header
            string[] headers = new string[] { "LID", "Data Pesata", "Peso (kg)", "Età (giorni)", "Peso Teorico (kg)", "Scostamento (%)", "Alert", "Razza Key", "Razza Des", "Stalla Key", "Stalla Des" };
            for (int c = 0; c < headers.Length; c++)
            {
                var cell = ws.Cell(1, c + 1);
                cell.Value = headers[c];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                cell.Style.Font.FontColor = XLColor.White;
            }

            // Data rows
            int row = 2;
            foreach (var r in rows)
            {
                ws.Cell(row, 1).Value  = r.Lid;
                ws.Cell(row, 2).Value  = r.DataPesata;
                ws.Cell(row, 2).Style.NumberFormat.Format = "yyyy-MM-dd";
                ws.Cell(row, 3).Value  = r.PesoKg;
                ws.Cell(row, 4).Value  = r.EtaGiorni;
                ws.Cell(row, 5).Value  = r.PesoTeorico;
                ws.Cell(row, 6).Value  = r.ScostamentoPct;
                ws.Cell(row, 7).Value  = r.AlertFlag ? "Sì" : "No";
                ws.Cell(row, 8).Value  = r.RazzaKey;
                ws.Cell(row, 9).Value  = r.RazzaDes;
                ws.Cell(row, 10).Value = r.StallaKey;
                ws.Cell(row, 11).Value = r.StaDes;
                row++;
            }

            ws.Columns().AdjustToContents();

            if (queryParams.IncludeMetadata)
                AddXlsMetadataDetailed(ws, row + 1, queryParams, usernameJwt, rows.Count);

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        // ── XLS Aggregated ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Genera il file Excel (.xlsx) in modalità aggregated usando ClosedXML.
        /// Vedere DS07-API, sezione "Risposte / 200 - Aggregated" e "Roadmap / Fase 2".
        /// </summary>
        private static byte[] BuildXlsAggregated(
            IReadOnlyList<PesaturaCurvaAccrescimentoRow> rows,
            PesatureExportQueryParams queryParams)
        {
            var aggregated = AggregateRows(rows, queryParams.AggregationLevel ?? "animal");

            using var workbook = new XLWorkbook();
            var ws = workbook.AddWorksheet("Dati");

            string[] headers = new string[] { "Gruppo", "Gruppo Des", "N. Animali", "N. Pesate", "Scostamento Medio (%)", "Scostamento Max (%)", "Scostamento Min (%)", "Alert Count", "Ultimo Aggiornamento" };
            for (int c = 0; c < headers.Length; c++)
            {
                var cell = ws.Cell(1, c + 1);
                cell.Value = headers[c];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                cell.Style.Font.FontColor = XLColor.White;
            }

            int row = 2;
            foreach (var a in aggregated)
            {
                ws.Cell(row, 1).Value = a.Gruppo;
                ws.Cell(row, 2).Value = a.GruppoDes;
                ws.Cell(row, 3).Value = a.NumeroAnimali;
                ws.Cell(row, 4).Value = a.NumeroPesate;
                ws.Cell(row, 5).Value = a.ScostamentoMedioPct;
                ws.Cell(row, 6).Value = a.ScostamentoMaxPct;
                ws.Cell(row, 7).Value = a.ScostamentoMinPct;
                ws.Cell(row, 8).Value = a.AlertCount;
                ws.Cell(row, 9).Value = a.UltimoAggiornamento;
                ws.Cell(row, 9).Style.NumberFormat.Format = "yyyy-MM-dd HH:mm:ss";
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        // ── Aggregation logic ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Aggrega le righe detailed per il livello richiesto (animal, razza_key, stalla_key).
        /// Vedere DS07-API, sezione "Risposte / 200 - Aggregated".
        /// </summary>
        private static List<PesatureExportRigaAggregata> AggregateRows(
            IReadOnlyList<PesaturaCurvaAccrescimentoRow> rows,
            string aggregationLevel)
        {
            IEnumerable<IGrouping<string, PesaturaCurvaAccrescimentoRow>> groups =
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
                    _            => string.Empty
                };

                int distinctAnimali = aggregationLevel.ToLowerInvariant() == "animal"
                    ? 1
                    : g.Select(r => r.Lid).Distinct().Count();

                return new PesatureExportRigaAggregata
                {
                    Gruppo               = g.Key,
                    GruppoDes            = gruppoDes,
                    NumeroAnimali        = distinctAnimali,
                    NumeroPesate         = g.Count(),
                    ScostamentoMedioPct  = Math.Round(g.Average(r => r.ScostamentoPct), 2),
                    ScostamentoMaxPct    = Math.Round(g.Max(r => r.ScostamentoPct), 2),
                    ScostamentoMinPct    = Math.Round(g.Min(r => r.ScostamentoPct), 2),
                    AlertCount           = g.Count(r => r.AlertFlag),
                    UltimoAggiornamento  = g.Max(r => r.DataPesata)
                };
            }).ToList();
        }

        // ── Metadata helpers ──────────────────────────────────────────────────────────────────────

        private static void AppendCsvMetadataDetailed(
            StringBuilder sb,
            PesatureExportQueryParams qp,
            string usernameJwt,
            int recordCount)
        {
            sb.Append("---METADATA---\r\n");
            sb.Append($"Data_Export,{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}\r\n");
            sb.Append("View_Mode,detailed\r\n");
            sb.Append($"Filtri_Applicati,\"{BuildFiltriApplicati(qp)}\"\r\n");
            sb.Append($"Username_Creazione,{usernameJwt}\r\n");
            sb.Append($"Numero_Record,{recordCount}\r\n");
        }

        private static void AppendCsvMetadataAggregated(
            StringBuilder sb,
            PesatureExportQueryParams qp,
            string usernameJwt,
            int numGruppi)
        {
            sb.Append("---METADATA---\r\n");
            sb.Append($"Data_Export,{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}\r\n");
            sb.Append("View_Mode,aggregated\r\n");
            sb.Append($"Aggregation_Level,{qp.AggregationLevel ?? "animal"}\r\n");
            sb.Append($"Filtri_Applicati,\"{BuildFiltriApplicati(qp)}\"\r\n");
            sb.Append($"Username_Creazione,{usernameJwt}\r\n");
            sb.Append($"Numero_Gruppi,{numGruppi}\r\n");
        }

        private static void AddXlsMetadataDetailed(
            IXLWorksheet ws,
            int startRow,
            PesatureExportQueryParams qp,
            string usernameJwt,
            int recordCount)
        {
            ws.Cell(startRow, 1).Value = "---METADATA---";
            ws.Cell(startRow, 1).Style.Font.Bold = true;
            ws.Cell(startRow + 1, 1).Value = "Data_Export";
            ws.Cell(startRow + 1, 2).Value = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            ws.Cell(startRow + 2, 1).Value = "View_Mode";
            ws.Cell(startRow + 2, 2).Value = "detailed";
            ws.Cell(startRow + 3, 1).Value = "Filtri_Applicati";
            ws.Cell(startRow + 3, 2).Value = BuildFiltriApplicati(qp);
            ws.Cell(startRow + 4, 1).Value = "Username_Creazione";
            ws.Cell(startRow + 4, 2).Value = usernameJwt;
            ws.Cell(startRow + 5, 1).Value = "Numero_Record";
            ws.Cell(startRow + 5, 2).Value = recordCount;
        }

        private static string BuildFiltriApplicati(PesatureExportQueryParams qp)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(qp.StallaPKeys))
                parts.Add($"stalla_key: {qp.StallaPKeys}");
            if (!string.IsNullOrWhiteSpace(qp.RazzaKeys))
                parts.Add($"razza_key: {qp.RazzaKeys}");
            parts.Add($"Range: {qp.DateFrom:yyyy-MM-dd} to {qp.DateTo:yyyy-MM-dd}");
            return string.Join(", ", parts);
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }
}
