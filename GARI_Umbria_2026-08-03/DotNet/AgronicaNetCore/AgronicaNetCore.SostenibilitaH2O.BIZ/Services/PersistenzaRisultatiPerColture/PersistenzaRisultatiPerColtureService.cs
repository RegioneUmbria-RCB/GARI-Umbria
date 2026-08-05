using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Resources;
using AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.Lookup_Sost_H2O_Lotto;
using InData.FoodMetaVerse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.PersistenzaRisultatiPerColture
{
    /// <summary>
    /// Implementazione della persistenza batch dei risultati di sostenibilità idrica (DS08-BL)
    /// nella tabella <c>Lookup_Sost_H20_Lotto</c> per la modalità "Per Colture".
    /// <para>
    /// Flusso:
    /// <list type="number">
    /// <item><description>Valida i dati obbligatori di ogni riga; se non conformi la salta con log warning.</description></item>
    /// <item><description>Costruisce il payload JSON (valori assoluti m³) e lo inietta in ciascun DTO.</description></item>
    /// <item><description>Delega l'inserimento atomico al DAL tramite <c>InsertBatchAsync</c>.</description></item>
    /// <item><description>Converte eventuali eccezioni DAL in <see cref="TransactionFailureException"/>.</description></item>
    /// </list>
    /// </para>
    /// Riferimento spec: DS08-BL PersistenzaRisultatiPerColture.
    /// </summary>
    public class PersistenzaRisultatiPerColtureService
        : BaseServiceSostenibilitaH2OBiz, IPersistenzaRisultatiPerColtureService
    {
        private readonly ILogger<PersistenzaRisultatiPerColtureService> _logger;
        private readonly ILookup_Sost_H2O_Lotto _lottoDal;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        /// <inheritdoc cref="BaseServiceSostenibilitaH2OBiz"/>
        public PersistenzaRisultatiPerColtureService(
            ILogger<PersistenzaRisultatiPerColtureService> logger,
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _logger = logger;
            _lottoDal = provider.GetRequiredService<ILookup_Sost_H2O_Lotto>();
        }

        /// <inheritdoc/>
        public async Task<PersistenzaRisultatiPerColtureOutput> EseguiAsync(
            PersistenzaRisultatiPerColtureInput input,
            AgronicaCoreParametriServer objParametriServer)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(objParametriServer);

            if (input.DataCalcolo == default)
                input.DataCalcolo = DateTime.UtcNow;

            // ── Step 1: validazione e filtro righe ────────────────────────────────
            var righeValide = new List<WriteLookupSostH2OLotto>(input.Risultati.Count);

            for (int i = 0; i < input.Risultati.Count; i++)
            {
                var riga = input.Risultati[i];

                try
                {
                    ValidaRiga(riga, i);

                    // Iniezione data_calcolo uniforme nel batch
                    riga.DataCalcolo = input.DataCalcolo;

                    righeValide.Add(riga);
                }
                catch (RigaValidationException ex)
                {
                    // Non bloccante: log warning, skip riga, prosegui batch.
                    // Riferimento spec: DS08-BL — ValidationException.
                    _logger.LogWarning(
                        "Riga {RigaIndex} saltata per validazione fallita su campo '{Campo}': {Message}",
                        ex.RigaIndex, ex.Campo, ex.Message);
                }
            }

            if (righeValide.Count == 0)
            {
                _logger.LogWarning(
                    "Nessuna riga valida nel batch id_invocazione={IdInvocazione}. Nessun inserimento eseguito.",
                    input.IdInvocazione);

                return new PersistenzaRisultatiPerColtureOutput
                {
                    IdInvocazione = input.IdInvocazione,
                    RowsInserted = 0
                };
            }

            // ── Step 3: inserimento atomico ────────────────────────────────────────
            int righeInserite;
            try
            {
                righeInserite = await _lottoDal.InsertBatchAsync(righeValide, objParametriServer);
            }
            catch (Exception ex)
            {
                // TransactionFailureException: rollback già effettuato nel DAL.
                // Riferimento spec: DS08-BL — TransactionFailureException.
                throw new TransactionFailureException(
                    $"Il batch di {righeValide.Count} righe per id_invocazione={input.IdInvocazione} " +
                    "è stato sottoposto a rollback a causa di un errore database.", ex);
            }

            _logger.LogInformation(
                "Persistenza DS08-BL completata: {RowsInserted}/{RowsTotal} righe inserite in Lookup_Sost_H20_Lotto. " +
                "id_invocazione={IdInvocazione}.",
                righeInserite, input.Risultati.Count, input.IdInvocazione);

            return new PersistenzaRisultatiPerColtureOutput
            {
                IdInvocazione = input.IdInvocazione,
                RowsInserted = righeInserite
            };
        }

        // ── Validation ────────────────────────────────────────────────────────────

        /// <summary>
        /// Valida i campi obbligatori e la coerenza numerica di una singola riga.
        /// Riferimento spec: DS08-BL — Validazione: superficie_riferimento &gt; 0.
        /// </summary>
        /// <exception cref="RigaValidationException">Riga non valida (non bloccante a livello batch).</exception>
        private static void ValidaRiga(WriteLookupSostH2OLotto riga, int rigaIndex)
        {
            if (string.IsNullOrWhiteSpace(riga.CuaaFiliera))
                throw new RigaValidationException(rigaIndex, nameof(riga.CuaaFiliera), "Il campo 'cuaa_filiera' è obbligatorio.");

            if (string.IsNullOrWhiteSpace(riga.CuaaAzienda))
                throw new RigaValidationException(rigaIndex, nameof(riga.CuaaAzienda), "Il campo 'cuaa_azienda' è obbligatorio.");

            if (riga.Anno <= 0)
                throw new RigaValidationException(rigaIndex, nameof(riga.Anno), "Il campo 'anno' deve essere > 0.");

            if (riga.Appezzamento == 0)
                throw new RigaValidationException(rigaIndex, nameof(riga.Appezzamento), "Il campo 'appezzamento' è obbligatorio.");

            if (riga.CulCod <= 0)
                throw new RigaValidationException(rigaIndex, nameof(riga.CulCod), "Il campo 'cul_cod' deve essere > 0.");

            if (string.IsNullOrWhiteSpace(riga.Esercizio))
                throw new RigaValidationException(rigaIndex, nameof(riga.Esercizio), "Il campo 'esercizio' è obbligatorio.");

            if (riga.SuperficieRiferimento <= 0m)
                throw new RigaValidationException(
                    rigaIndex, nameof(riga.SuperficieRiferimento),
                    $"La superficie_riferimento deve essere > 0. Valore ricevuto = {riga.SuperficieRiferimento}.");
        }
    }
}
