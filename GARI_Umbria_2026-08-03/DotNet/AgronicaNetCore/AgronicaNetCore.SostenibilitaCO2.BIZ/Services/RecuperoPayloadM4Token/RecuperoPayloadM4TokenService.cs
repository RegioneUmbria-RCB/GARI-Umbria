using AgronicaNetCore.Anagrafe.DAL.DataLayer.GerarchiaImprese;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impresa;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Resources;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Aziendale_Chiavi;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Aziendale_Payload;
using InData.FoodMetaVerse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Data;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.RecuperoPayloadM4Token
{
    /// <summary>
    /// Implementazione della business logic <c>RecuperoPayloadM4DaLookupToken</c> (DS09-BL).
    /// Interroga <c>Lookup_Sost_CO2_Aziendale_Chiavi</c> e
    /// <c>Lookup_Sost_CO2_Aziendale_Payload</c> per popolare la tabella "Token Generabili"
    /// nella pagina di creazione token (FS2.07.1).
    /// </summary>
    /// <remarks>
    /// Riferimento spec: DS09-BL RecuperoPayloadM4DaLookupToken — 01KHX...
    /// Il Cono di Visibilità Utente viene rivalutato ad ogni invocazione (no cache).
    /// </remarks>
    public class RecuperoPayloadM4TokenService : BaseServiceSostenibilitaCO2Biz, IRecuperoPayloadM4TokenService
    {
        private readonly ILookup_Sost_CO2_Aziendale_Chiavi _chiaviDal;
        private readonly ILookup_Sost_CO2_Aziendale_Payload _payloadDal;
        private readonly IGerarchiaImprese _gerarchiaImprese;
        private readonly IImpresa _impresa;

        /// <summary>Inizializza il servizio risolvendo le dipendenze DAL dalla DI.</summary>
        public RecuperoPayloadM4TokenService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _chiaviDal        = provider.GetRequiredService<ILookup_Sost_CO2_Aziendale_Chiavi>();
            _payloadDal       = provider.GetRequiredService<ILookup_Sost_CO2_Aziendale_Payload>();
            _gerarchiaImprese = provider.GetRequiredService<IGerarchiaImprese>();
            _impresa          = provider.GetRequiredService<IImpresa>();
        }

        /// <inheritdoc/>
        public async Task<List<int>> GetAnniDisponibiliAsync(
            string filiera,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            if (string.IsNullOrWhiteSpace(filiera))
                throw new ArgumentException("Filiera è obbligatoria.", nameof(filiera));

            // La lookup salva il CUAA della filiera, non la PIVA passata dall'utente.
            var cuaaFiliera = await ResolveCuaaFromPivaAsync(filiera, objParametriServer);

            // DS09-BL §Regola 4 — Cono di Visibilità: rivalutato ad ogni query, no cache.
            var aziendeVisibili = await ResolveAziendeVisibiliAsync(filiera, objParametriServer);

            // Recupera tutte le righe chiavi per la filiera (senza filtro anno).
            DataTable chiaviTable = await _chiaviDal.ReadAsync(
                new GetSostCO2Aziendale { Filiera = cuaaFiliera },
                objParametriServer);

            return chiaviTable.AsEnumerable()
                .Where(row => aziendeVisibili.Contains(
                    row.Field<string>("cuaa_azienda") ?? string.Empty))
                .Select(row => row.Field<int>("anno"))
                .Distinct()
                .OrderByDescending(a => a)
                .ToList();
        }

        /// <inheritdoc/>
        public async Task<List<TokenGenerabileItem>> GetTokenGenerabiliAsync(
            string filiera,
            int anno,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            if (string.IsNullOrWhiteSpace(filiera))
                throw new ArgumentException("Filiera è obbligatoria.", nameof(filiera));

            // La lookup salva il CUAA della filiera, non la PIVA passata dall'utente.
            var cuaaFiliera = await ResolveCuaaFromPivaAsync(filiera, objParametriServer);

            // DS09-BL §Regola 4 — Cono di Visibilità: rivalutato ad ogni query, no cache.
            var aziendeVisibili = await ResolveAziendeVisibiliAsync(filiera, objParametriServer);

            return await GetTokenGenerabiliCoreAsync(cuaaFiliera, anno, aziendeVisibili, objParametriServer);
        }

        // ── Helpers ───────────────────────────────────────────────────────────────────

        /// <summary>
        /// Risolve il CUAA della filiera a partire dalla PIVA.
        /// La lookup table salva il CUAA, ma l'input dell'utente è la PIVA.
        /// </summary>
        /// <exception cref="AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions.DataNotFoundException">
        /// Lanciata se nessun CUAA è associato alla PIVA fornita.
        /// </exception>
        private async Task<string> ResolveCuaaFromPivaAsync(
            string pivaFiliera,
            AgronicaCoreParametriServer objParametriServer)
        {
            var cuaa = await _impresa.CuaaFromPivaAsync(pivaFiliera, objParametriServer);
            if (string.IsNullOrWhiteSpace(cuaa))
                throw new AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions.DataNotFoundException(
                    $"CUAA non trovato per la filiera PIVA '{pivaFiliera}'.");
            return cuaa;
        }

        // ── DS09-BL §Regola 4 — Cono di Visibilità ───────────────────────────────────

        /// <summary>
        /// Risolve le aziende figlie visibili all'utente per la filiera data.
        /// Lancia <see cref="UnauthorizedAccessException"/> se nessuna azienda è visibile.
        /// </summary>
        private async Task<HashSet<string>> ResolveAziendeVisibiliAsync(
            string filiera,
            AgronicaCoreParametriServer objParametriServer)
        {
            var aziendeVisibili = await _gerarchiaImprese.LeggiElencoGerarchiaImpreseFiglieAsync(
                filiera, objParametriServer);

            if (aziendeVisibili == null || aziendeVisibili.Count == 0)
            {
                LogInformation(
                    $"Nessuna azienda visibile per la filiera '{filiera}'. Utente: {objParametriServer.UtenteUsername}.",
                    objParametriServer);
                throw new UnauthorizedAccessException(
                    $"L'utente non ha visibilità su alcuna azienda della filiera '{filiera}'.");
            }

            return new HashSet<string>(aziendeVisibili, StringComparer.OrdinalIgnoreCase);
        }

        // ── DS09-BL §Regola 2/3 — Query token generabili ────────────────────────────

        private async Task<List<TokenGenerabileItem>> GetTokenGenerabiliCoreAsync(
            string filiera,
            int anno,
            HashSet<string> aziendeVisibili,
            AgronicaCoreParametriServer objParametriServer)
        {
            // ReadAsync ordina già per data_invocazione DESC.
            DataTable chiaviTable = await _chiaviDal.ReadAsync(
                new GetSostCO2Aziendale { Filiera = filiera, Anno = anno },
                objParametriServer);

            // DS09-BL §Regola 4: filtra per Cono di Visibilità.
            // Tutte le righe vengono restituite; il frontend rende selezionabile
            // solo la riga più recente per ciascuna azienda (in base a data_invocazione).
            var righeVisibili = chiaviTable.AsEnumerable()
                .Where(row => aziendeVisibili.Contains(
                    row.Field<string>("cuaa_azienda") ?? string.Empty))
                .ToList();

            // Risolve PIVA e ragione sociale per ogni CUAA azienda distinto (batch, evita N+1).
            var cuaaDistinti = righeVisibili
                .Select(row => row.Field<string>("cuaa_azienda") ?? string.Empty)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // cuaa → (piva, ragSoc)
            var infoAziende = new Dictionary<string, (string Piva, string? RagSoc)>(StringComparer.OrdinalIgnoreCase);
            foreach (var cuaa in cuaaDistinti)
            {
                var piva = await _impresa.PivaFromCuaaAsync(cuaa, objParametriServer);
                string? ragSoc = null;
                if (!string.IsNullOrWhiteSpace(piva))
                {
                    var dt = await _impresa.LeggiAsync(piva, objParametriServer);
                    ragSoc = dt?.Rows.Count > 0 ? dt.Rows[0].Field<string>("rag_soc") : null;
                }
                infoAziende[cuaa] = (piva ?? string.Empty, ragSoc);
            }

            var tokenGenerabili = new List<TokenGenerabileItem>();

            foreach (var chiaveRow in righeVisibili)
            {
                var idInvocazione   = chiaveRow.Field<string>("id_invocazione")   ?? string.Empty;
                var cuaaAzienda     = chiaveRow.Field<string>("cuaa_azienda")     ?? string.Empty;
                var dataInvocazione = chiaveRow.Field<DateTime>("data_invocazione");

                infoAziende.TryGetValue(cuaaAzienda, out var infoAzienda);

                // DS09-BL §Regola 3 — Join con tabella Payload per recuperare json_risposta.
                var (varSoc, numAppezzamenti, jsonRisposta) = await EstraiIndicatoriDaPayloadAsync(
                    idInvocazione, cuaaAzienda, objParametriServer);

                tokenGenerabili.Add(new TokenGenerabileItem
                {
                    IdInvocazione            = idInvocazione,
                    DataInvocazione          = dataInvocazione,
                    PivaAzienda              = infoAzienda.Piva,
                    RagSocAzienda            = infoAzienda.RagSoc,
                    VarSocSoilBiogenicCarbon = varSoc,
                    NumeroAppezzamenti       = numAppezzamenti,
                    JsonRisposta             = jsonRisposta
                });
            }

            // Ordine finale: per azienda ASC, data_invocazione DESC (coerente con FS2.07.1).
            return tokenGenerabili
                .OrderBy(t => t.PivaAzienda)
                .ThenByDescending(t => t.DataInvocazione)
                .ToList();
        }

        // ── DS09-BL §Regola 3 — Estrazione indicatori da JSON response ───────────────

        /// <summary>
        /// Carica il payload JSON e ne estrae <c>var_soc_soil_biogenic_carbon</c>
        /// e il conteggio degli appezzamenti per l'azienda indicata.
        /// </summary>
        /// <returns>
        /// Tuple (<c>varSoc</c>, <c>numeroAppezzamenti</c>).
        /// In caso di JSON corrotto logga un warning e restituisce <c>(null, 0)</c>
        /// per non bloccare le altre righe (best-effort).
        /// </returns>
        private async Task<(decimal? varSoc, int numeroAppezzamenti, string? jsonRisposta)> EstraiIndicatoriDaPayloadAsync(
            string idInvocazione,
            string cuaaAzienda,
            AgronicaCoreParametriServer objParametriServer)
        {
            DataTable payloadTable = await _payloadDal.ReadAsync(idInvocazione, null, objParametriServer);

            if (payloadTable.Rows.Count == 0)
            {
                LogWarning(
                    $"Nessuna riga payload per id_invocazione={idInvocazione}. Indicatori omessi.",
                    objParametriServer);
                return (null, 0, null);
            }

            var jsonRisposta = payloadTable.Rows[0].Field<string>("json_risposta");
            if (string.IsNullOrWhiteSpace(jsonRisposta))
            {
                LogWarning(
                    $"json_risposta vuoto per id_invocazione={idInvocazione}. Indicatori omessi.",
                    objParametriServer);
                return (null, 0, null);
            }

            try
            {
                var m4Response = JsonConvert.DeserializeObject<M4RispostaDto>(jsonRisposta);
                var aziendaData = m4Response?.Aziende?.FirstOrDefault(a =>
                    string.Equals(a.IdAzienda, cuaaAzienda, StringComparison.OrdinalIgnoreCase));

                if (aziendaData == null)
                {
                    LogWarning(
                        $"Azienda '{cuaaAzienda}' non trovata in json_risposta per id_invocazione={idInvocazione}. Indicatori omessi.",
                        objParametriServer);
                    return (null, 0, jsonRisposta);
                }

                // Preferisce il blocco totali canonico se presente, altrimenti il campo radice.
                var varSoc = aziendaData.Totali?.FirstOrDefault()?.VarSocSoilBiogenicCarbon
                             ?? aziendaData.VarSocSoilBiogenicCarbon;

                var numAppezzamenti = aziendaData.Appezzamenti?.Count ?? 0;

                return (varSoc, numAppezzamenti, jsonRisposta);
            }
            catch (JsonException ex)
            {
                // DS09-BL §Eccezioni DeserializationException: JSON risposta corrotto → best-effort.
                LogWarning(
                    $"Deserializzazione json_risposta fallita per id_invocazione={idInvocazione}, azienda='{cuaaAzienda}'. {ex.Message}",
                    objParametriServer, ex);
                return (null, 0, jsonRisposta);
            }
        }

        // ── Private DTOs per la deserializzazione del json_risposta M4 ───────────────

        private sealed class M4RispostaDto
        {
            [JsonProperty("aziende")]
            public List<M4AziendaDto>? Aziende { get; set; }
        }

        private sealed class M4AziendaDto
        {
            [JsonProperty("id_azienda")]
            public string? IdAzienda { get; set; }

            /// <summary>Variazione SOC biogenico (Kg CO₂ eq.), presente a livello radice azienda.</summary>
            [JsonProperty("var_soc_soil_biogenic_carbon")]
            public decimal? VarSocSoilBiogenicCarbon { get; set; }

            /// <summary>Blocco totali canonico restituito dal motore M4.</summary>
            [JsonProperty("totali")]
            public List<M4TotaliDto>? Totali { get; set; }

            [JsonProperty("appezzamenti")]
            public List<M4AppezzamentoDto>? Appezzamenti { get; set; }
        }

        private sealed class M4TotaliDto
        {
            [JsonProperty("CO2eq_tot_var_SOC")]
            public decimal? VarSocSoilBiogenicCarbon { get; set; }
        }

        private sealed class M4AppezzamentoDto
        {
            // Usato solo per il conteggio; non è necessario mappare i campi.
        }
    }
}
