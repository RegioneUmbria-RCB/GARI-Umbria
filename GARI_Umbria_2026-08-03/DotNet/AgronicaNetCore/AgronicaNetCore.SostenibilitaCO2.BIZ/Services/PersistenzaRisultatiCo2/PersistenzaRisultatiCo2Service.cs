using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Aziendale_Chiavi;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Aziendale_Payload;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Colture_Chiavi;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Colture_Payload;
using InData.FoodMetaVerse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Text.Json;
using System.Transactions;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.PersistenzaRisultatiCo2
{
    /// <summary>
    /// Implementazione della business logic di persistenza dei risultati M4 nelle tabelle di lookup
    /// (DS08-BL PersistenzaRisultatiM4LookupTable).
    /// <para>
    /// Esegue un insert atomico su due coppie di tabelle in base alla modalità:
    /// <list type="bullet">
    ///   <item>
    ///     <term>Per Colture</term>
    ///     <description>
    ///       <c>Lookup_Sost_CO2_Colture_Chiavi</c> (N righe, una per ogni combinazione
    ///       azienda/appezzamento/impianto) e <c>Lookup_Sost_CO2_Colture_Payload</c>
    ///       (1 riga per invocazione).
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>Aziendale</term>
    ///     <description>
    ///       <c>Lookup_Sost_CO2_Aziendale_Chiavi</c> (N righe, una per azienda) e
    ///       <c>Lookup_Sost_CO2_Aziendale_Payload</c> (1 riga per invocazione).
    ///     </description>
    ///   </item>
    /// </list>
    /// </para>
    /// Riferimento spec: DS08-BL PersistenzaRisultatiM4LookupTable.
    /// </summary>
    public class PersistenzaRisultatiCo2Service : BaseServiceSostenibilitaCO2Biz, IPersistenzaRisultatiCo2Service
    {
        private const string ModalitaPerColture = "Per Colture";

        private const string TabellaChiaviColture = "Lookup_Sost_CO2_Colture_Chiavi";
        private const string TabellaPayloadColture = "Lookup_Sost_CO2_Colture_Payload";
        private const string TabellaChiaviAziendale = "Lookup_Sost_CO2_Aziendale_Chiavi";
        private const string TabellaPayloadAziendale = "Lookup_Sost_CO2_Aziendale_Payload";

        private readonly ILookup_Sost_CO2_Colture_Chiavi _coltureChiaviDAL;
        private readonly ILookup_Sost_CO2_Colture_Payload _colturePayloadDAL;
        private readonly ILookup_Sost_CO2_Aziendale_Chiavi _aziendaleChiaviDAL;
        private readonly ILookup_Sost_CO2_Aziendale_Payload _aziendalePayloadDAL;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = false
        };

        public PersistenzaRisultatiCo2Service(
            IServiceProvider provider,
            IStringLocalizer<Resources.Messages> localizer)
            : base(provider, localizer)
        {
            _coltureChiaviDAL = provider.GetRequiredService<ILookup_Sost_CO2_Colture_Chiavi>();
            _colturePayloadDAL = provider.GetRequiredService<ILookup_Sost_CO2_Colture_Payload>();
            _aziendaleChiaviDAL = provider.GetRequiredService<ILookup_Sost_CO2_Aziendale_Chiavi>();
            _aziendalePayloadDAL = provider.GetRequiredService<ILookup_Sost_CO2_Aziendale_Payload>();
        }

        /// <inheritdoc/>
        public async Task<PersistenzaRisultatiCo2Output> EseguiAsync(
            PersistenzaRisultatiCo2Input input,
            AgronicaCoreParametriServer objParametriServer)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(input.PayloadRequestCo2);
            ArgumentNullException.ThrowIfNull(input.PayloadResponseCo2);

            var jsonRichiesta = SerializzaPayload(input.PayloadRequestCo2);
            var jsonRisposta = SerializzaPayload(input.PayloadResponseCo2);

            string idInvocazione = GeneraIdInvocazione();

            return input.Modalita.Equals(ModalitaPerColture, StringComparison.OrdinalIgnoreCase)
                ? await PersistiPerColtureAsync(input, idInvocazione, jsonRichiesta, jsonRisposta, objParametriServer)
                : await PersistiAziendaleAsync(input, idInvocazione, jsonRichiesta, jsonRisposta, objParametriServer);
        }

        // ────────────────────────────────────────────────────────────────
        // Persistenza "Per Colture"
        // ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Esegue insert atomico su <c>Lookup_Sost_CO2_Colture_Chiavi</c> (una riga per combinazione
        /// azienda/appezzamento/impianto) e <c>Lookup_Sost_CO2_Colture_Payload</c> (una riga totale).
        /// DS08-BL Regola 2.
        /// </summary>
        private async Task<PersistenzaRisultatiCo2Output> PersistiPerColtureAsync(
            PersistenzaRisultatiCo2Input input,
            string idInvocazione,
            string jsonRichiesta,
            string jsonRisposta,
            AgronicaCoreParametriServer objParametriServer)
        {
            var filiera = ParseFiliera(input.PayloadRequestCo2.codice_raggruppamento);
            int righeInserite = 0;

            using var ts = new TransactionScope(
                objParametriServer.objTransazione != null
                    ? TransactionScopeOption.Required
                    : TransactionScopeOption.RequiresNew,
                new TimeSpan(0, 5, 0),
                TransactionScopeAsyncFlowOption.Enabled);

            foreach (var azienda in input.PayloadRequestCo2.aziende)
            {
                foreach (var appezzamento in azienda.appezzamenti)
                {
                    if (!TryParseAppezzamento(appezzamento.id_appezzamento, out int appezzamentoId))
                    {
                        LogWarning(
                            "DS08-BL: appezzamento '{Id}' non parsabile come intero — riga omessa.",
                            objParametriServer, null, appezzamento.id_appezzamento);
                        continue;
                    }

                    foreach (var impianto in appezzamento.impianti)
                    {
                        if (!TryParseVegCod(impianto.id_coltura, out int vegCod))
                        {
                            LogWarning(
                                "DS08-BL: id_coltura '{Id}' non parsabile come VegCod — riga omessa.",
                                objParametriServer, null, impianto.id_coltura);
                            continue;
                        }

                        var progettoCodJson = BuildProgettoCodJson(impianto.id_impianto);

                        var chiave = new WriteLookupSostCO2ColtureChiavi
                        {
                            Id = 0,
                            Id_Invocazione = idInvocazione,
                            Data_Invocazione = input.TimestampInvocazione,
                            Anno = azienda.campagna,
                            Piva_Filiera = filiera,
                            Piva_Azienda = azienda.id_azienda,
                            Appezzamento = appezzamentoId,
                            Veg_Cod = vegCod,
                            Progetto_Cod = progettoCodJson,
                            Inviato = 1,
                            DataInvio = input.TimestampInvocazione,
                            Username_Creazione = input.Username,
                            Username_Modifica = input.Username
                        };

                        await _coltureChiaviDAL.ScriviModificaAsync(chiave, objParametriServer);
                        righeInserite++;
                    }
                }
            }

            var payload = new WriteLookupSostCO2ColturePayload
            {
                Id_Invocazione = idInvocazione,
                Json_Richiesta = jsonRichiesta,
                Json_Risposta = jsonRisposta,
                Inviato = 1,
                DataInvio = input.TimestampInvocazione,
                Username_Creazione = input.Username,
                Username_Modifica = input.Username
            };

            await _colturePayloadDAL.CreateAsync(payload, objParametriServer);

            ts.Complete();

            LogInformation(
                "DS08-BL (Per Colture): id_invocazione={Id}, righe_chiavi={Righe}, filiera={Filiera}",
                objParametriServer, null, idInvocazione, righeInserite, filiera);

            return new PersistenzaRisultatiCo2Output
            {
                IdInvocazione = idInvocazione,
                PersistenzaEsito = true,
                TabellaChiavi = TabellaChiaviColture,
                TabellaPayload = TabellaPayloadColture,
                RigheInserite = righeInserite
            };
        }

        // ────────────────────────────────────────────────────────────────
        // Persistenza "Aziendale"
        // ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Esegue insert atomico su <c>Lookup_Sost_CO2_Aziendale_Chiavi</c> (una riga per azienda)
        /// e <c>Lookup_Sost_CO2_Aziendale_Payload</c> (una riga totale).
        /// DS08-BL Regola 3.
        /// </summary>
        private async Task<PersistenzaRisultatiCo2Output> PersistiAziendaleAsync(
            PersistenzaRisultatiCo2Input input,
            string idInvocazione,
            string jsonRichiesta,
            string jsonRisposta,
            AgronicaCoreParametriServer objParametriServer)
        {
            var filiera = ParseFiliera(input.PayloadRequestCo2.codice_raggruppamento);
            int righeInserite = 0;

            using var ts = new TransactionScope(
                objParametriServer.objTransazione != null
                    ? TransactionScopeOption.Required
                    : TransactionScopeOption.RequiresNew,
                new TimeSpan(0, 5, 0),
                TransactionScopeAsyncFlowOption.Enabled);

            foreach (var azienda in input.PayloadRequestCo2.aziende)
            {
                var chiave = new WriteLookupSostCO2AziendaleChiavi
                {
                    Id = 0,
                    Id_Invocazione = idInvocazione,
                    Data_Invocazione = input.TimestampInvocazione,
                    Anno = azienda.campagna,
                    Filiera = filiera,
                    Azienda = azienda.id_azienda,
                    Inviato = 1,
                    DataInvio = input.TimestampInvocazione,
                    Username_Creazione = input.Username,
                    Username_Modifica = input.Username
                };

                await _aziendaleChiaviDAL.ScriviModificaAsync(chiave, objParametriServer);
                righeInserite++;
            }

            var payload = new WriteLookupSostCO2AziendalePayload
            {
                Id_Invocazione = idInvocazione,
                Json_Richiesta = jsonRichiesta,
                Json_Risposta = jsonRisposta,
                Inviato = 1,
                DataInvio = input.TimestampInvocazione,
                Username_Creazione = input.Username,
                Username_Modifica = input.Username
            };

            await _aziendalePayloadDAL.CreateAsync(payload, objParametriServer);

            ts.Complete();

            LogInformation(
                "DS08-BL (Aziendale): id_invocazione={Id}, righe_chiavi={Righe}, filiera={Filiera}",
                objParametriServer, null, idInvocazione, righeInserite, filiera);

            return new PersistenzaRisultatiCo2Output
            {
                IdInvocazione = idInvocazione,
                PersistenzaEsito = true,
                TabellaChiavi = TabellaChiaviAziendale,
                TabellaPayload = TabellaPayloadAziendale,
                RigheInserite = righeInserite
            };
        }

        // ────────────────────────────────────────────────────────────────
        // Helper privati
        // ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Genera un identificativo univoco per l'invocazione (GUID stringa senza separatori).
        /// DS08-BL Regola 1.
        /// </summary>
        private static string GeneraIdInvocazione() => Guid.NewGuid().ToString("N");

        /// <summary>
        /// Estrae la filiera dal campo <c>codice_raggruppamento</c> prendendo il primo segmento
        /// separato da <c>'|'</c>. Es.: <c>"F01|Coltura|2025"</c> → <c>"F01"</c>.
        /// </summary>
        private static string ParseFiliera(string codiceRaggruppamento)
        {
            if (string.IsNullOrWhiteSpace(codiceRaggruppamento))
                return string.Empty;

            var idx = codiceRaggruppamento.IndexOf('|');
            return idx > 0 ? codiceRaggruppamento[..idx] : codiceRaggruppamento;
        }

        /// <summary>
        /// Estrae il codice numerico dell'appezzamento dall'identificativo composto
        /// <c>PIVA|Sa_Cod|Appezza</c> (ultimo segmento).
        /// </summary>
        private static bool TryParseAppezzamento(string idAppezzamento, out int appezzamentoId)
        {
            appezzamentoId = 0;

            if (string.IsNullOrWhiteSpace(idAppezzamento))
                return false;

            var last = idAppezzamento.AsSpan().TrimEnd();
            var sep = last.LastIndexOf('|');
            var segment = sep >= 0 ? last[(sep + 1)..] : last;

            return int.TryParse(segment, out appezzamentoId);
        }

        /// <summary>
        /// Converte <c>id_coltura</c> (stringa numerica del <c>Veg_Cod</c>) in intero.
        /// </summary>
        private static bool TryParseVegCod(string idColtura, out int vegCod)
            => int.TryParse(idColtura, out vegCod);

        /// <summary>
        /// Estrae il <c>Progetto_Cod</c> dall'identificativo impianto
        /// <c>PIVA|Sa_Cod|Appezza|ID_Reg|Progetto_Cod</c> (ultimo segmento) e lo serializza
        /// come array JSON (es. <c>["E32"]</c>) per il campo <c>Progetto_Cod</c> nel DB.
        /// DS08-BL Regola 2 — colonna <c>esercizio</c>.
        /// </summary>
        private static int BuildProgettoCodJson(string idImpianto)
        {
            if (string.IsNullOrWhiteSpace(idImpianto))
                return 0;

            var span = idImpianto.AsSpan().TrimEnd();
            var sep = span.LastIndexOf('|');
            var cod = sep >= 0 ? span[(sep + 1)..].ToString() : span.ToString();

            return int.TryParse(cod, out var result) ? result : 0;
        }

        /// <summary>
        /// Serializza un oggetto a JSON; solleva <see cref="SerializationException"/> se fallisce.
        /// DS08-BL — SerializationException.
        /// </summary>
        private static string SerializzaPayload<T>(T payload)
        {
            try
            {
                return JsonSerializer.Serialize(payload, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new SerializationException(
                    $"Payload non serializzabile: {typeof(T).Name}. {ex.Message}", ex);
            }
        }
    }
}
