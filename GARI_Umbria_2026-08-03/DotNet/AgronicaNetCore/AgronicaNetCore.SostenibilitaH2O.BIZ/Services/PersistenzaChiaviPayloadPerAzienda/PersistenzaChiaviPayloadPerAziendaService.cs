using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Resources;
using AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.Lookup_Sost_H2O_Aziendale_Chiavi;
using AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.Lookup_Sost_H2O_Aziendale_Payload;
using InData.FoodMetaVerse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data.SqlClient;
using System.Text;
using System.Text.Json;
using System.Transactions;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.PersistenzaChiaviPayloadPerAzienda
{
    /// <summary>
    /// Persistenza atomica (TransactionScope) di MetaDati + Payload di sostenibilità idrica
    /// nella modalità "Per Azienda".
    /// <para>
    /// Flusso:
    /// <list type="number">
    /// <item><description>Valida i campi obbligatori dell'input (fail-fast, lato applicativo).</description></item>
    /// <item><description>Verifica che <c>PayloadJson</c> sia JSON sintatticamente valido (parse test).</description></item>
    /// <item><description>In <see cref="TransactionScope"/>: inserisce la riga Chiavi via
    ///   <c>ScriviModificaAsync</c> (include generazione ID da sequenza DB),
    ///   poi inserisce la riga Payload via <c>CreateAsync</c>.</description></item>
    /// <item><description>In caso di violazione FK (SqlException 547) → <see cref="ForeignKeyViolationException"/>.</description></item>
    /// <item><description>In caso di qualsiasi altro errore DB → <see cref="TransactionFailureException"/> + rollback.</description></item>
    /// </list>
    /// </para>
    /// Riferimento spec: DS10-BL PersistenzaChiaviPayloadPerAzienda.
    /// </summary>
    public class PersistenzaChiaviPayloadPerAziendaService
        : BaseServiceSostenibilitaH2OBiz, IPersistenzaChiaviPayloadPerAziendaService
    {
        private readonly ILookup_Sost_H2O_Aziendale_Chiavi _chiaveDal;
        private readonly ILookup_Sost_H2O_Aziendale_Payload _payloadDal;

        /// <inheritdoc cref="BaseServiceSostenibilitaH2OBiz"/>
        public PersistenzaChiaviPayloadPerAziendaService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _chiaveDal = provider.GetRequiredService<ILookup_Sost_H2O_Aziendale_Chiavi>();
            _payloadDal = provider.GetRequiredService<ILookup_Sost_H2O_Aziendale_Payload>();
        }

        /// <inheritdoc/>
        public async Task<PersistenzaChiaviPayloadPerAziendaOutput> EseguiAsync(
            PersistenzaChiaviPayloadPerAziendaInput input,
            AgronicaCoreParametriServer objParametriServer)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(objParametriServer);

            // Fail-fast: validazione campi obbligatori
            if (string.IsNullOrWhiteSpace(input.Filiera))
                throw new ArgumentException("Il campo Filiera è obbligatorio.", nameof(input));
            if (string.IsNullOrWhiteSpace(input.Azienda))
                throw new ArgumentException("Il campo Azienda è obbligatorio.", nameof(input));
            if (input.Anno <= 0)
                throw new ArgumentException("Il campo Anno deve essere > 0.", nameof(input));
            if (string.IsNullOrWhiteSpace(input.Nazione))
                throw new ArgumentException("Il campo Nazione è obbligatorio.", nameof(input));
            if (input.SuperficieColtivataHa <= 0m)
                throw new ArgumentException(
                    $"Il campo SuperficieColtivataHa deve essere > 0. Valore: {input.SuperficieColtivataHa}.",
                    nameof(input));

            // Validazione JSON del payload
            ValidaPayloadJson(input.PayloadJson);

            var chiaviDto = BuildChiaviDto(input, objParametriServer.UtenteUsername ?? "agronica");
            var payloadDto = BuildPayloadDto(input, objParametriServer.UtenteUsername ?? "agronica");

            int idCalcolo = 0;

            try
            {
                // Transazione atomica: prima Chiavi poi Payload
                using var ts = new TransactionScope(
                TransactionScopeOption.RequiresNew,
                new TimeSpan(0, 10, 0),
                TransactionScopeAsyncFlowOption.Enabled);
                {
                    // Upsert Chiavi — ScriviModificaAsync genera dto.Id tramite sequenza DB
                    await _chiaveDal.ScriviModificaAsync(chiaviDto, objParametriServer);

                    // Insert Payload — correlato a Chiavi via id_invocazione (FK)
                    await _payloadDal.CreateAsync(payloadDto, objParametriServer);

                    ts.Complete();

                    idCalcolo = chiaviDto.Id;
                }
            }
            catch (SqlException ex) when (ex.Number == ForeignKeyViolationException.SqlForeignKeyErrorNumber)
            {
                throw new ForeignKeyViolationException(
                    $"Violazione FK durante la persistenza Per Azienda: " +
                    $"id_invocazione={input.IdInvocazione}.",
                    ex);
            }
            catch (Exception ex) when (ex is not (ArgumentException or InvalidJsonException or ForeignKeyViolationException))
            {
                throw new TransactionFailureException(
                    $"Fallimento transazionale durante la persistenza Per Azienda: " +
                    $"azienda='{input.Azienda}' anno={input.Anno} id_invocazione={input.IdInvocazione}.",
                    ex);
            }

            LogInformation(
                    "DS10-BL persistenza completata: azienda={Azienda} anno={Anno} " +
                    "id_invocazione={IdInvocazione} id_calcolo={IdCalcolo}",
                    objParametriServer, null, input.Azienda, input.Anno, input.IdInvocazione, chiaviDto.Id);

            return new PersistenzaChiaviPayloadPerAziendaOutput
            {
                // Il Payload non ha un proprio PK numerico nell'implementazione corrente:
                // usa lo stesso ID della riga Chiavi correlata (1:1 via id_invocazione).
                IdCalcolo = idCalcolo,
                IdPayload = idCalcolo
            };

        }

        // ── Helpers privati ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifica che <paramref name="payloadJson"/> contenga JSON valido.
        /// Riferimento spec: DS10-BL — "Validazione: payload_json deve essere JSON valido (parse test)".
        /// </summary>
        private static void ValidaPayloadJson(string payloadJson)
        {
            if (string.IsNullOrWhiteSpace(payloadJson))
                throw new InvalidJsonException("Il payload_json non può essere vuoto.");

            try
            {
                using var doc = JsonDocument.Parse(payloadJson);
                // Verifica presenza delle chiavi H2O obbligatorie nel payload:
                // la struttura attesa è { "aziende": [ { ..., "anni": [ { "sostenibilita_h2o": { ... } } ] } ] }
                // Un parse riuscito è sufficiente come "parse test" secondo la spec.
            }
            catch (JsonException ex)
            {
                throw new InvalidJsonException(
                    "Il payload_json fornito non è JSON sintatticamente valido.", ex);
            }
        }

        private static WriteLookupSostH2OAziendaleChiavi BuildChiaviDto(
            PersistenzaChiaviPayloadPerAziendaInput input,
            string username)
        {
            return new WriteLookupSostH2OAziendaleChiavi
            {
                Id_Invocazione = input.IdInvocazione.ToString(),
                Data_Calcolo   = input.DataCalcolo == default ? DateTime.UtcNow : input.DataCalcolo,
                Anno           = input.Anno,
                CuaaFiliera    = input.Filiera,
                CuaaAzienda    = input.Azienda,
                Username_Creazione = username,
                Username_Modifica = username
            };
        }

        private static WriteLookupSostH2OAziendalePayload BuildPayloadDto(
            PersistenzaChiaviPayloadPerAziendaInput input,
            string username)
        {
            return new WriteLookupSostH2OAziendalePayload(
                idInvocazione: input.IdInvocazione.ToString(),
                payloadJson:   input.PayloadJson,
                jsonFirmato:   input.JsonFirmato,
                utente:      username);
        }
    }
}
