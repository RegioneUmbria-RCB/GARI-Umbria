using AgronicaNetCore.Agenda.DAL.DataLayer.Agenda;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.RecuperoResaProduttiva
{
    /// <summary>
    /// Implementazione della business logic per il recupero della resa produttiva (DS04-BL).
    /// <para>
    /// Utilizza <c>IAgenda.LeggiImpiantiAsync</c> con <see cref="Common.OperazioniRaccolta"/>
    /// per ottenere le operazioni di raccolta filtrate per anno ed esercizi in stato "Chiuso".
    /// Recupera le quantità prodotte tramite <c>IAgenda.LeggiProdottiAsync</c> sui soli
    /// Id_Agenda filtrati.
    /// </para>
    /// <para>
    /// La resa è aggregata in due proiezioni: per esercizio (Progetto_Cod, modalità "Per Colture")
    /// e per azienda (PIVA, modalità "Aziendale").
    /// </para>
    /// Riferimento spec: DS04-BL RecuperoResaProduttiva.
    /// </summary>
    public class RecuperoResaProduttivaService
        : BaseServiceSostenibilitaH2OBiz, IRecuperoResaProduttivaService
    {
        ///// <summary>
        ///// Nome della colonna restituita da <c>LeggiProdottiAsync</c> contenente la
        ///// quantità del prodotto raccolto.
        ///// TODO: verificare se Qta_Dett è espressa in KG o in altra unità di misura.
        ///// </summary>
        //private const string ColQtaDett = "Qta_Dett";

        public RecuperoResaProduttivaService(
            IServiceProvider provider,
            IStringLocalizer<Resources.Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <inheritdoc/>
        public async Task<ResaProduttivaResult> GetResaAsync(
            List<EsercizioH2O> esercizi,
            int anno,
            AgronicaCoreParametriServer objParametriServer)
        {
            ArgumentNullException.ThrowIfNull(esercizi);
            if (esercizi.Count == 0)
                throw new ArgumentException("La lista degli esercizi non può essere vuota.", nameof(esercizi));
            if (anno <= 0)
                throw new ArgumentOutOfRangeException(nameof(anno), "L'anno di riferimento deve essere maggiore di zero.");

            var agenda        = _serviceProvider.GetRequiredService<IAgenda>();
            var idEsercizi    = esercizi.Select(e => e.IdEsercizio).ToList();
            var lavCodRaccolta = new List<int>(Common.OperazioniRaccolta);

            LogInformation(
                "RecuperoResaProduttiva avviato — Anno: {Anno}, Esercizi: {Count}",
                objParametriServer, null, anno, idEsercizi.Count);

            // ── Step 1: recupera impianti per le operazioni di raccolta ─────────
            DataTable dtImpianti;
            try
            {
                dtImpianti = await agenda.LeggiImpiantiAsync(
                    Id_Agenda: new List<int>(),
                    objParametriServer: objParametriServer,
                    createTempEsercizi: false,
                    leggiFlagEsercizioChiuso: true,
                    leggiIndirizzoAppezzamento: false,
                    leggiGIS: false,
                    Id_Esercizi: idEsercizi,
                    Lav_cod: lavCodRaccolta);
            }
            catch (Exception ex) when (ex is not ArgumentException and not ArgumentOutOfRangeException)
            {
                throw new DatabaseQueryException(
                    $"Errore durante il recupero degli impianti per la resa produttiva — Anno: {anno}.", ex);
            }

            // ── Step 2: filtra per anno e validazione esercizio chiuso ──────────
            // TODO: valutare se applicare anche un filtro su Tipo_Destinazione
            //       per limitare alle sole destinazioni di tipo impianto.
            var righeRaccolta = dtImpianti.AsEnumerable()
                .Where(r =>
                    r["Data_Movimento"] != DBNull.Value &&
                    ((DateTime)r["Data_Movimento"]).Year == anno &&
                    string.Equals(r["Flag_Esercizio_Chiuso"]?.ToString(), "1", StringComparison.Ordinal))
                .ToList();

            if (righeRaccolta.Count == 0)
                throw new NoHarvestDataException(
                    $"Nessuna operazione di raccolta trovata per l'anno {anno} su esercizi chiusi.");

            // Id_Agenda filtrati → usati come ingresso per LeggiProdottiAsync
            var idAgendaFiltrati = righeRaccolta
                .Select(r => Convert.ToInt32(r["Id_Agenda"]))
                .Distinct()
                .ToList();

            // Lookup Id_Agenda → (ProgettoCtod, Piva) per aggregazione successiva
            var impiantiLookup = righeRaccolta
                .GroupBy(r => Convert.ToInt32(r["Id_Agenda"]))
                .ToDictionary(
                    g => g.Key,
                    g => (
                        ProgettoCtod: Convert.ToInt32(g.First()["progetto_cod"]),
                        Piva: g.First()["Piva"]?.ToString() ?? string.Empty));

            // Veg_Cod e Cul_Cod per esercizio (da LeggiImpiantiAsync — sempre presenti)
            var vegCodPerEsercizio = righeRaccolta
                .GroupBy(r => Convert.ToInt32(r["progetto_cod"]))
                .ToDictionary(g => g.Key, g => Convert.ToInt32(g.First()["Veg_Cod"]));

            var culCodPerEsercizio = righeRaccolta
                .GroupBy(r => Convert.ToInt32(r["progetto_cod"]))
                .ToDictionary(g => g.Key, g => Convert.ToInt32(g.First()["Cul_Cod"]));

            // Dati anagrafici impianto per esercizio (da LeggiImpiantiAsync — sempre presenti)
            var datiImpiantoPerEsercizio = righeRaccolta
                .GroupBy(r => Convert.ToInt32(r["progetto_cod"]))
                .ToDictionary(g => g.Key, g =>
                {
                    var r = g.First();
                    return (
                        Piva:    r["Piva"]?.ToString() ?? string.Empty,
                        SaCod: r["Sa_Cod"] != DBNull.Value ? Convert.ToInt32(r["Sa_Cod"]) : 0,
                        Appezza: r["Appezza"] != DBNull.Value ? Convert.ToInt32(r["Appezza"]) : 0,
                        IdReg:   r["Id_Destinazione"] != DBNull.Value ? Convert.ToInt32(r["Id_Destinazione"]) : 0,
                        CulCod:  r["Cul_Cod"] != DBNull.Value ? Convert.ToInt32(r["Cul_Cod"]) : 0,
                        SupApp:  r["SUP_APP"] != DBNull.Value ? Convert.ToDecimal(r["SUP_APP"]) : 0m,
                        VegCod:  r["Veg_Cod"] != DBNull.Value ? Convert.ToInt32(r["Veg_Cod"]) : 0
                    );
                });

            // ── Step 3: recupera prodotti raccolta per gli Id_Agenda filtrati ───
            DataTable dtProdotti;
            try
            {
                dtProdotti = await agenda.LeggiProdottiAsync(
                    Id_Agenda: idAgendaFiltrati,
                    leggiMateriePrime: true,
                    leggiEsercizi: false,
                    objParametriServer: objParametriServer,
                    estraiSpecieECultivar: false,
                    leggiFlagEsercizioChiuso: false,
                    Id_Esercizi: null,
                    Lav_cod: null);
            }
            catch (Exception ex) when (ex is not ArgumentException and not ArgumentOutOfRangeException)
            {
                throw new DatabaseQueryException(
                    $"Errore durante il recupero dei prodotti raccolta — Anno: {anno}.", ex);
            }

            // ── Step 4: aggrega la resa in tonnellate ────────────────────────────
            var resaPerEsercizio       = new Dictionary<int, decimal>();
            var resaPerAzienda         = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var lottoRaccoltaPerEsercizio = new Dictionary<int, string?>();

            foreach (DataRow row in dtProdotti.Rows)
            {
                if (row["Qta_Dett"] == DBNull.Value)
                    continue;

                var qtaKg = Convert.ToDecimal(row["Qta_Dett"]);
                if (qtaKg <= 0)
                    continue;

                //se non sono Kg sono numeri e non posso convertirli
                var udmCod = Convert.ToInt32(row["Udm_Cod"]);
                if (udmCod != (int)Enum_UnitaMisura.KG)
                    continue;
               
                var idAgenda = Convert.ToInt32(row["Id_Agenda"]);
                if (!impiantiLookup.TryGetValue(idAgenda, out var impianto))
                    continue;

                // unità di misura Qta_Dett in kg —-> t = KG ÷ 1000
                var resaT = qtaKg / 1000;

                resaPerEsercizio[impianto.ProgettoCtod] =
                    resaPerEsercizio.TryGetValue(impianto.ProgettoCtod, out var prevE)
                        ? prevE + resaT
                        : resaT;

                if (!string.IsNullOrEmpty(impianto.Piva))
                {
                    resaPerAzienda[impianto.Piva] =
                        resaPerAzienda.TryGetValue(impianto.Piva, out var prevA)
                            ? prevA + resaT
                            : resaT;
                }

                // Primo lotto raccolta per esercizio (Movimenti_dettagli.Lotto)
                if (!lottoRaccoltaPerEsercizio.ContainsKey(impianto.ProgettoCtod))
                {
                    var lotto = row["Lotto"] != DBNull.Value ? row["Lotto"]?.ToString() : null;
                    lottoRaccoltaPerEsercizio[impianto.ProgettoCtod] = lotto;
                }
            }

            // Precisione minima 2 decimali (requisito spec DS04-BL)
            var resaEserciziRounded = resaPerEsercizio
                .ToDictionary(kv => kv.Key, kv => Math.Round(kv.Value, 2));
            var resaAziendaRounded  = resaPerAzienda
                .ToDictionary(kv => kv.Key, kv => Math.Round(kv.Value, 2), StringComparer.OrdinalIgnoreCase);

            LogInformation(
                "RecuperoResaProduttiva completato — Esercizi con resa: {CountE}, Aziende con resa: {CountA}",
                objParametriServer, null, resaEserciziRounded.Count, resaAziendaRounded.Count);

            return new ResaProduttivaResult(resaEserciziRounded, resaAziendaRounded, vegCodPerEsercizio, culCodPerEsercizio, datiImpiantoPerEsercizio, lottoRaccoltaPerEsercizio);
        }
    }
}
