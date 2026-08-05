using AgronicaNetCore.Agenda.DAL.DataLayer.Agenda;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.RecuperoConsumoIdricoEffettivo
{
    /// <summary>
    /// Implementazione della business logic di recupero del consumo idrico effettivo.
    /// <para>
    /// Recupera le operazioni di irrigazione e fertirrigazione tramite <see cref="IAgenda.LeggiImpiantiAsync"/>
    /// con <c>Lav_cod = [1, 26]</c>, filtra in-memory per <c>Cau_Mov = 2300</c> e anno di riferimento,
    /// poi interroga <c>Mov_Dettaglio_Tecnico</c> per l'unità di misura (<c>Dett_Cod</c>)
    /// e converte i volumi in m³ aggregandoli per esercizio e per azienda.
    /// Per la fertirrigazione viene applicato anche il filtro <c>Elem_Cod = 200</c> e <c>Mat_Cod = -1</c>
    /// per isolare la sola componente acqua.
    /// </para>
    /// Riferimento spec: DS03-BL RecuperoConsumoIdricoEffettivo.
    /// </summary>
    public class RecuperoConsumoIdricoEffettivoService
        : BaseServiceSostenibilitaH2OBiz, IRecuperoConsumoIdricoEffettivoService
    {
        public RecuperoConsumoIdricoEffettivoService(
            IServiceProvider provider,
            IStringLocalizer<Resources.Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <inheritdoc/>
        public async Task<VolumiIrrigazioneResult> GetVolumiAsync(
            List<EsercizioH2O> esercizi,
            int anno,
            AgronicaCoreParametriServer objParametriServer)
        {
            ArgumentNullException.ThrowIfNull(esercizi);
            if (esercizi.Count == 0)
                throw new ArgumentException("La lista degli esercizi non può essere vuota.", nameof(esercizi));
            if (anno <= 0)
                throw new ArgumentOutOfRangeException(nameof(anno), "L'anno di riferimento deve essere maggiore di zero.");

            var agenda     = _serviceProvider.GetRequiredService<IAgenda>();
            var idEsercizi = esercizi.Select(e => e.IdEsercizio).ToList();

            LogInformation(
                "RecuperoConsumoIdricoEffettivo avviato — Anno: {Anno}, Esercizi: {Count}",
                objParametriServer, null, anno, idEsercizi.Count);

            // ── Step 1: recupera irrigazione e fertirrigazione in un'unica chiamata ────
            // LeggiImpiantiAsync con Lav_cod=[1,26] restituisce entrambe le operazioni.
            // Le colonne Elem_Cod e Mat_Cod (aggiunte al DAL via LEFT OUTER JOIN su
            // Movimenti_Dettagli WHERE Elem_Cod=200/Mat_Cod=-1) consentono di distinguere
            // in-memory le righe ferti che contengono la componente acqua.
            DataTable dtImpianti;
            try
            {
                dtImpianti = await agenda.LeggiImpiantiAsync(
                    Id_Agenda: new List<int>(),
                    objParametriServer: objParametriServer,
                    Id_Esercizi: idEsercizi,
                    Lav_cod: new List<int> { LAV_COD.LAVCOD_IRRIGAZIONE, LAV_COD.LAVCOD_FERTIRRIGAZIONE });
            }
            catch (Exception ex) when (ex is not ArgumentException and not ArgumentOutOfRangeException)
            {
                LogError(
                    "RecuperoConsumoIdricoEffettivo: errore durante la query su GIAS — Anno: {Anno}", objParametriServer, ex, anno);
                throw new DatabaseQueryException(
                    $"Errore durante il recupero del consumo idrico per l'anno {anno}.", ex);
            }

            // ── Step 2: filtra irrigazione per anno, Cau_Mov e Qta > 0 ───────────────
            var righeIrrigazione = dtImpianti.AsEnumerable()
                .Where(row =>
                    row["Data_Movimento"] != DBNull.Value &&
                    Convert.ToDateTime(row["Data_Movimento"]).Year == anno &&
                    row["Cau_Mov"] != DBNull.Value &&
                    row["Cau_Mov"].ToString() == CAU_MOV.CAU_LAVORAZIONE &&
                    row["Lav_Cod"] != DBNull.Value &&
                    Convert.ToInt32(row["Lav_Cod"]) == LAV_COD.LAVCOD_IRRIGAZIONE &&
                    row["Qta"] != DBNull.Value &&
                    Convert.ToDecimal(row["Qta"]) > 0m)
                .ToList();

            // ── Step 2b: filtra fertirrigazione — solo righe con la componente acqua ────
            // Elem_Cod=200 e Mat_Cod=-1 sono esposti da LeggiImpiantiAsync tramite
            // LEFT OUTER JOIN su Movimenti_Dettagli (riga acqua). Per le righe di
            // irrigazione questi campi valgono 0 (ISNULL default), quindi non intersecano.
            var righeFertirrigazione = dtImpianti.AsEnumerable()
                .Where(row =>
                    row["Data_Movimento"] != DBNull.Value &&
                    Convert.ToDateTime(row["Data_Movimento"]).Year == anno &&
                    row["Cau_Mov"] != DBNull.Value &&
                    row["Cau_Mov"].ToString() == CAU_MOV.CAU_LAVORAZIONE &&
                    row["Lav_Cod"] != DBNull.Value &&
                    Convert.ToInt32(row["Lav_Cod"]) == LAV_COD.LAVCOD_FERTIRRIGAZIONE &&
                    row["Elem_Cod"] != DBNull.Value &&
                    Convert.ToInt32(row["Elem_Cod"]) == ELEM_COD.ALTRE_MATERIE &&
                    row["Mat_Cod"] != DBNull.Value &&
                    Convert.ToInt32(row["Mat_Cod"]) == MAT_COD.MAT_COD_ACQUA_IRRIGAZIONE &&
                    row["Qta"] != DBNull.Value &&
                    Convert.ToDecimal(row["Qta"]) > 0m)
                .ToList();

            if (righeIrrigazione.Count == 0 && righeFertirrigazione.Count == 0)
            {
                LogWarning(
                    "RecuperoConsumoIdricoEffettivo: nessun movimento trovato — Anno: {Anno}", objParametriServer, null, anno);
                return new VolumiIrrigazioneResult(
                    new Dictionary<int, decimal>(),
                    new Dictionary<string, decimal>());
            }

            // ── Step 3: recupera Dett_Cod da Mov_Dettaglio_Tecnico per entrambe le liste ──
            var idAgendas = righeIrrigazione.Concat(righeFertirrigazione)
                .Select(row => Convert.ToInt32(row["Id_Agenda"]))
                .Distinct()
                .ToList();

            DataTable dtDettCod;
            try
            {
                dtDettCod = await agenda.LeggiDettCodAsync(idAgendas, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(
                    "RecuperoConsumoIdricoEffettivo: errore nel recupero Dett_Cod — Anno: {Anno}", objParametriServer, ex, anno);
                throw new DatabaseQueryException(
                    $"Errore durante il recupero dell'unità di misura per l'anno {anno}.", ex);
            }

            // Mapping in-memory: costruisce il lookup (Piva, SaCod, IdAgenda, IdMov) → Dett_Cod
            var dettCodLookup = dtDettCod.AsEnumerable()
                .ToDictionary(
                    row => new MovimentoKeyH2O(
                        row["Piva"]?.ToString() ?? string.Empty,
                        Convert.ToInt32(row["Sa_Cod"]),
                        Convert.ToInt32(row["Id_Agenda"]),
                        Convert.ToInt32(row["Id_Mov"])),
                    row => row["Dett_Cod"] is DBNull ? 0 : Convert.ToInt32(row["Dett_Cod"]));

            // ── Step 4: aggrega irrigazione per esercizio e per azienda ──────────
            var volumiPerEsercizio = AggregaPerEsercizio(righeIrrigazione, dettCodLookup);
            var volumiPerAzienda   = AggregaPerAzienda(righeIrrigazione, dettCodLookup);

            // ── Step 5: aggrega fertirrigazione e somma all'irrigazione ───────────
            var fertiPerEsercizio  = AggregaPerEsercizio(righeFertirrigazione, dettCodLookup);
            var fertiPerAzienda    = AggregaPerAzienda(righeFertirrigazione, dettCodLookup);

            var totalePerEsercizio = MergeVolumi(volumiPerEsercizio, fertiPerEsercizio);
            var totalePerAzienda   = MergeVolumi(volumiPerAzienda,   fertiPerAzienda);

            LogInformation(
                "RecuperoConsumoIdricoEffettivo completato — Esercizi con dati: {NE}, Aziende con dati: {NA}",
                objParametriServer, null, totalePerEsercizio.Count, totalePerAzienda.Count);

            return new VolumiIrrigazioneResult(totalePerEsercizio, totalePerAzienda);
        }

        // ─── Aggregazione ────────────────────────────────────────────────────────

        private static IReadOnlyDictionary<int, decimal> AggregaPerEsercizio(
            IReadOnlyList<DataRow> righe,
            IReadOnlyDictionary<MovimentoKeyH2O, int> dettCodLookup)
        {
            var result = new Dictionary<int, decimal>();

            foreach (var row in righe)
            {
                var progettoCod = Convert.ToInt32(row["progetto_cod"]);
                var volumeM3    = CalcolaVolumeM3(row, dettCodLookup);

                if (volumeM3 <= 0m)
                    continue;

                result[progettoCod] = result.TryGetValue(progettoCod, out var existing)
                    ? existing + volumeM3
                    : volumeM3;
            }

            return result;
        }

        private static IReadOnlyDictionary<string, decimal> AggregaPerAzienda(
            IReadOnlyList<DataRow> righe,
            IReadOnlyDictionary<MovimentoKeyH2O, int> dettCodLookup)
        {
            var result = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in righe)
            {
                var piva     = row["Piva"]?.ToString() ?? string.Empty;
                var volumeM3 = CalcolaVolumeM3(row, dettCodLookup);

                if (string.IsNullOrWhiteSpace(piva) || volumeM3 <= 0m)
                    continue;

                result[piva] = result.TryGetValue(piva, out var existing)
                    ? existing + volumeM3
                    : volumeM3;
            }

            return result;
        }

        /// <summary>
        /// Somma due dizionari con la stessa chiave, aggregando i valori per chiavi uguali.
        /// </summary>
        private static Dictionary<TKey, decimal> MergeVolumi<TKey>(
            IReadOnlyDictionary<TKey, decimal> primo,
            IReadOnlyDictionary<TKey, decimal> secondo)
            where TKey : notnull
        {
            if (secondo.Count == 0) return primo.ToDictionary(kvp => kvp.Key, kvp => Math.Round(kvp.Value, 2));
            if (primo.Count == 0)   return secondo.ToDictionary(kvp => kvp.Key, kvp => Math.Round(kvp.Value, 2));

            var result = new Dictionary<TKey, decimal>(primo);
            foreach (var (key, value) in secondo)
                result[key] = result.TryGetValue(key, out var existing)
                    ? Math.Round(existing + value, 2)
                    : Math.Round(value, 2);

            return result;
        }

        // ─── Conversione unità di misura ─────────────────────────────────────────

        /// <summary>
        /// Converte il volume della singola riga in m³ in base a <c>Dett_Cod</c>
        /// recuperato dal lookup su <c>Mov_Dettaglio_Tecnico</c>.
        /// <list type="bullet">
        ///   <item><c>Dett_Cod = 1</c> (mm): volume_m³ = Qta × sup_imp × 10</item>
        ///   <item><c>Dett_Cod = 20</c> (m³): volume_m³ = Qta</item>
        ///   <item>Altro / assente: assunto m³ (TODO: verificare con team GIAS).</item>
        /// </list>
        /// Riferimento spec: DS03-BL — "l'acqua ha anche una unità di misura mm oppure m³".
        /// </summary>
        private static decimal CalcolaVolumeM3(
            DataRow row,
            IReadOnlyDictionary<MovimentoKeyH2O, int> dettCodLookup)
        {
            if (row["Qta"] is DBNull || row["Qta"] is null)
                return 0m;

            var qta = Convert.ToDecimal(row["Qta"]);
            if (qta <= 0m)
                return 0m;

            var key = new MovimentoKeyH2O(
                row["Piva"]?.ToString() ?? string.Empty,
                Convert.ToInt32(row["Sa_Cod"]),
                Convert.ToInt32(row["Id_Agenda"]),
                Convert.ToInt32(row["Id_Mov"]));

            var dettCod = dettCodLookup.TryGetValue(key, out var dc) ? dc : 0;
            var supImp  = row["sup_imp"] is DBNull ? 0m : Convert.ToDecimal(row["sup_imp"]);

            return dettCod switch
            {
                (int)Enum_UnitaMisura.Millimetri =>
                    // mm → m³: (mm / 1000) × (ha × 10000 m²) = mm × ha × 10
                    qta * supImp * 10m,

                (int)Enum_UnitaMisura.METRI3__HA =>
                    // TODO: verificare il valore esatto di DettCodMetriCubi con il team GIAS
                    qta,

                _ =>
                    // Unità sconosciuta o Dett_Cod assente: assumo m³ come fallback
                    qta
            };
        }

        // ─── Tipi privati ─────────────────────────────────────────────────────────

        /// <summary>Chiave composita per il lookup di Dett_Cod da Mov_Dettaglio_Tecnico.</summary>
        private record MovimentoKeyH2O(string Piva, int SaCod, int IdAgenda, int IdMov);
    }
}
