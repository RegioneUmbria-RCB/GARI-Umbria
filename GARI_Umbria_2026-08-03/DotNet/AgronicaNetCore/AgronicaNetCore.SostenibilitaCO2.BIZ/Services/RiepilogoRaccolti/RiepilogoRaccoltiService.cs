using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Agenda.DAL.DataLayer.Agenda;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Resources;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.GerarchiaImprese;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impresa;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Indirizzi;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.RiepilogoRaccolti
{
    /// <summary>
    /// Servizio che calcola la tabella di riepilogo raccolti CO₂:
    /// espande la filiera, legge le operazioni di raccolta (LavCod=125),
    /// filtra per esercizi chiusi e aggrega i dati per colonna UI.
    /// </summary>
    public class RiepilogoRaccoltiService : BaseServiceSostenibilitaCO2Biz, IRiepilogoRaccoltiService
    {
        /// <summary>LavCod corrispondente all'operazione di raccolta.</summary>
        private const int LavCodRaccolta = 125;

        /// <summary>Udm_Cod corrispondente ai Kg.</summary>
        private const int UdmKg = 2;

        private readonly IGerarchiaImprese _gerarchiaImprese;
        private readonly IAgenda _agenda;
        private readonly IIndirizzi _indirizziSostenibilita;
        private readonly IImpresa _impresa;

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="RiepilogoRaccoltiService"/>.
        /// </summary>
        public RiepilogoRaccoltiService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _gerarchiaImprese = _serviceProvider.GetRequiredService<IGerarchiaImprese>();
            _agenda = _serviceProvider.GetRequiredService<IAgenda>();
            _indirizziSostenibilita = _serviceProvider.GetRequiredService<IIndirizzi>();
            _impresa = _serviceProvider.GetRequiredService<IImpresa>();
        }

        /// <inheritdoc />
        public async Task<RiepilogoRaccoltiResult> GetRiepilogoAsync(RiepilogoRaccoltiRequest request, AgronicaCoreParametriServer objParametriServer)
        {
            // ── STEP 1: Estrazione PIVA figlie dalla filiera ─────────────────────────
            var pivas = await _gerarchiaImprese.LeggiElencoGerarchiaImpreseFiglieAsync(request.PivaFiliera, objParametriServer);

            if (pivas == null || pivas.Count == 0)
                return new RiepilogoRaccoltiResult();

            // ── STEP 2: Inizializzazione struttura aggregazione ───────────────────────

            // Chiave di aggregazione: (PIVA, ProgettoCod)
            var aggr = new Dictionary<(string Piva, string ProgettoCod), AggrData>();
            // Raccolta chiavi geografiche: (Piva, Sa_Cod, Appezza)
            var geoKeys = new HashSet<AppezzamentoGeoKey>();

            var dateInterval = new IntervalloTemporale
            {
                inizio = new DateTime(request.Anno, 1, 1),
                fine   = new DateTime(request.Anno, 12, 31)
            };

            // ── STEP 3: Elaborazione iterativa per azienda ────────────────────────────
            foreach (var piva in pivas)
            {
                // STEP 3.1 – Elenco operazioni di raccolta per questa PIVA nell'anno
                var agendaDt = await _agenda.LeggiAgendaAsync(piva, idAgendas: null, sacod: null, lavCods: new List<int> { LavCodRaccolta }, cauMovs: null, dateInterval: dateInterval, 
                    objParametriServer: objParametriServer, filtroSemineImpiantil: null);

                if (agendaDt == null || agendaDt.Rows.Count == 0)
                    continue;

                // Raggruppa gli id_agenda per questa PIVA
                var idAgendas = agendaDt.Rows.Cast<DataRow>()
                    .Select(r => Convert.ToInt32(r["id_agenda"]))
                    .Distinct()
                    .ToList();

                // STEP 3.2 – Elaborazione iterativa per operazione agenda
                foreach (var idAgenda in idAgendas)
                {
                    var idAgendaList = new List<int> { idAgenda };

                    // STEP 3.2.1 – Prodotti raccolti con flag esercizio chiuso, dati impianto e dati esercizio.
                    // Singola query: LeggiProdottiAsync con leggiFlagEsercizioChiuso=true
                    // sup_imp, Piva, Sa_Cod, Appezza sono ora sempre inclusi nel result set.
                    var prodottiDt = await _agenda.LeggiProdottiAsync(idAgendaList, leggiMateriePrime: true, leggiEsercizi: true, objParametriServer, leggiFlagEsercizioChiuso: true);

                    if (prodottiDt == null || prodottiDt.Rows.Count == 0)
                        continue;

                    // STEP 3.2.2 – Filtro esercizi: tutte le righe devono avere Flag=1

                    bool tuttiChiusi = prodottiDt.Rows.Cast<DataRow>()
                        .All(r => Convert.ToString(r["Flag_Esercizio_Chiuso"]) == "1");

                    if (!tuttiChiusi)
                        continue;

                    // Filtro coltura: se vegCod > 0, almeno una riga deve corrispondere

                    if (request.VegCod > 0)
                    {
                        bool colturaTrovata = prodottiDt.Columns.Contains("Veg_Cod") && prodottiDt.Rows.Cast<DataRow>().Any(r => Convert.ToInt32(r["Veg_Cod"]) == request.VegCod);

                        if (!colturaTrovata)
                            continue;
                    }

                    // Data operazione dalla riga agenda (per DataUltimaRaccolta)
                    var agendaRow = agendaDt.Rows.Cast<DataRow>().FirstOrDefault(r => Convert.ToInt32(r["id_agenda"]) == idAgenda);

                    var dataMovimento = agendaRow != null ? Convert.ToDateTime(agendaRow["Data_Movimento"]) : (DateTime?)null;

                    // STEP 3.2.3 – Aggregazione per (Piva, ProgettoCod), solo Udm=Kg
                    foreach (System.Data.DataRow prodRow in prodottiDt.Rows)
                    {
                        int udmCod = Convert.ToInt32(prodRow["Udm_Cod"]);
                        if (udmCod != UdmKg)
                            continue;

                        string progettoCod = Convert.ToString(prodRow["Progetto_Cod"]) ?? string.Empty;
                        var aggrKey = (piva, progettoCod);

                        decimal qtaDett = prodRow.Table.Columns.Contains("Qta_Dett") ? Convert.ToDecimal(prodRow["Qta_Dett"]) : 0m;

                        string matDes = Convert.ToString(prodRow["Mat_Des"]) ?? string.Empty;
                        string lotto  = Convert.ToString(prodRow["Lotto"])   ?? string.Empty;

                        if (!aggr.TryGetValue(aggrKey, out var aggrData))
                        {
                            // CASO A: nuova riga — tutti i campi letti direttamente da prodottiDt
                            string appNome = Convert.ToString(prodRow["APP_NOME"])      ?? string.Empty;
                            string vegDes = prodottiDt.Columns.Contains("Veg_Des") ? (Convert.ToString(prodRow["Veg_Des"]) ?? string.Empty) : string.Empty;
                            string progettoNome = prodottiDt.Columns.Contains("Progetto_Nome") ? (Convert.ToString(prodRow["Progetto_Nome"]) ?? string.Empty) : string.Empty;
                            // Fallback su Progetto_Cod se il nome è vuoto

                            if (string.IsNullOrWhiteSpace(progettoNome))
                                progettoNome = progettoCod;

                            decimal supImp  = Convert.ToDecimal(prodRow["sup_imp"]);
                            int saCod   = Convert.ToInt32(prodRow["Sa_Cod"]);
                            int appezza = Convert.ToInt32(prodRow["Appezza"]);

                            aggrData = new AggrData
                            {
                                Esercizio  = progettoCod,
                                AppNome    = appNome,
                                VegDes     = vegDes,
                                SupImp     = supImp,
                                SaCod      = saCod,
                                Appezza    = appezza,
                                TotKg      = qtaDett,
                                DataUltima = dataMovimento,
                                MatDesSet  = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                                LottiSet   = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                            };
                            aggr[aggrKey] = aggrData;

                            // Raccolta chiave geografica per batch lookup
                            if (saCod > 0 && appezza > 0)
                                geoKeys.Add(new AppezzamentoGeoKey(piva, saCod, appezza));
                        }
                        else
                        {
                            // CASO B: aggiorna riga esistente
                            aggrData.TotKg += qtaDett;
                            if (dataMovimento.HasValue &&
                                (!aggrData.DataUltima.HasValue || dataMovimento.Value > aggrData.DataUltima.Value))
                            {
                                aggrData.DataUltima = dataMovimento;
                            }
                        }

                        // Accodamento deduplicato Prodotti e Lotti
                        if (!string.IsNullOrWhiteSpace(matDes))
                            aggrData.MatDesSet.Add(matDes.Trim());
                        if (!string.IsNullOrWhiteSpace(lotto))
                            aggrData.LottiSet.Add(lotto.Trim());
                    }
                }
            }

            if (aggr.Count == 0)
                return new RiepilogoRaccoltiResult();

            // ── STEP 4: Batch lookup dati geografici ─────────────────────────────────
            var geoMap = new Dictionary<AppezzamentoGeoKey, (string Nazione, string Regione, string IstatReg, string Provincia)>();
            if (geoKeys.Count > 0)
            {
               var geoDt = await _indirizziSostenibilita.LeggiIndirizzoxAppezzamentoAsync(geoKeys, objParametriServer);

                if (geoDt != null)
                {
                    foreach (System.Data.DataRow geoRow in geoDt.Rows)
                    {
                        var geoKey = new AppezzamentoGeoKey(
                            Convert.ToString(geoRow["PIVA"])  ?? string.Empty,
                            Convert.ToInt32(geoRow["SA_COD"]),
                            Convert.ToInt32(geoRow["APPEZZA"]));

                        geoMap[geoKey] = (
                            Convert.ToString(geoRow["Nazione"])   ?? string.Empty,
                            Convert.ToString(geoRow["Regione"])   ?? string.Empty,
                            Convert.ToString(geoRow["REG"])       ?? string.Empty,
                            Convert.ToString(geoRow["Provincia"]) ?? string.Empty);
                    }
                }
            }

            // ── STEP 5: Batch lookup Ragione Sociale per ogni PIVA unica ─────────────
            var pivasUniche = aggr.Keys.Select(k => k.Piva).Distinct().ToList();
            var ragSocMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var piva in pivasUniche)
            {
                var impresaDt = await _impresa.LeggiAsync(piva, objParametriServer);
                if (impresaDt != null && impresaDt.Rows.Count > 0)
                    ragSocMap[piva] = Convert.ToString(impresaDt.Rows[0]["Rag_Soc"]) ?? piva;
                else
                    ragSocMap[piva] = piva;
            }

            // ── STEP 6: Costruzione risultato finale ──────────────────────────────────
            var righe = new List<RigaRiepilogoRaccolti>();

            foreach (var kvp in aggr)
            {
                var (pivaCurr, _) = kvp.Key;
                var d = kvp.Value;

                var geoKey = new AppezzamentoGeoKey(pivaCurr, d.SaCod, d.Appezza);
                geoMap.TryGetValue(geoKey, out var geo);

                righe.Add(new RigaRiepilogoRaccolti
                {
                    Azienda           = ragSocMap.TryGetValue(pivaCurr, out var rs) ? rs : pivaCurr,
                    PartitaIva        = pivaCurr,
                    Appezzamento      = d.AppNome,
                    Esercizio         = d.Esercizio,
                    Nazione           = geo.Nazione   ?? string.Empty,
                    Regione           = geo.Regione   ?? string.Empty,
                    ISTAT_reg         = geo.IstatReg  ?? string.Empty,
                    Provincia         = geo.Provincia ?? string.Empty,
                    Superficie        = d.SupImp,
                    SpecieColturale   = d.VegDes,
                    ProdottiRaccolti  = string.Join(", ", d.MatDesSet),
                    CodiceLotti       = string.Join(", ", d.LottiSet),
                    DataUltimaRaccolta = d.DataUltima,
                    TotaleRaccoltaKg  = d.TotKg
                });
            }

            return new RiepilogoRaccoltiResult { Righe = righe };
        }

        // ─── Classe interna di supporto all'aggregazione ────────────────────────────

        private sealed class AggrData
        {
            public string Esercizio    { get; set; } = string.Empty;
            public string AppNome      { get; set; } = string.Empty;
            public string VegDes       { get; set; } = string.Empty;
            public decimal SupImp      { get; set; }
            public int SaCod           { get; set; }
            public int Appezza         { get; set; }
            public decimal TotKg       { get; set; }
            public DateTime? DataUltima{ get; set; }
            public HashSet<string> MatDesSet { get; set; } = new(StringComparer.OrdinalIgnoreCase);
            public HashSet<string> LottiSet  { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        }
    }
}
