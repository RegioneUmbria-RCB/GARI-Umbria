using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.DatamarsHttpService;
using AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.Exceptions;
using AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.ValidazionePayload;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.Fabbricati;
using OutData.Zoo.DataMars;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.StagingPesate;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.StagingSessioniApi;
using InData.Zoo.DataMars;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Text.Json;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.AcquisizionePesate;

/// <summary>
/// Orchestratore del ciclo completo di acquisizione pesate bovine dall'API Datamars.
/// <para>
/// Flusso (DS01-BL Step 1–7):
/// <list type="number">
///   <item>Autentica tramite OAuth2 usando le credenziali ricevute in input; gestisce il rinnovo automatico del token 10 minuti prima della scadenza.</item>
///   <item>Leggi URL base da <c>Configurazione_Siti</c> (chiave <c>urlDatamarsApi</c>).</item>
///   <item>Ottieni lista FarmID dalla tabella FABBRICATI/FABBRICATI_CODICI.</item>
///   <item>Per ogni FarmID: GET <c>/farms/{farmId}/integrationSessions</c>.</item>
///   <item>Per ogni sessionId: verifica deduplicazione; se già presente con stato valido, skip.</item>
///   <item>GET <c>/integrationSessions/{sessionId}</c>, valida payload, costruisci JSON atomico.</item>
///   <item>Apri transazione ACID; inserisci STAGING_PESATE + STAGING_SESSIONI_API; chiudi transazione.</item>
///   <item>Aggrega conteggi e ritorna <see cref="RisultatoAcquisizionePesate"/>.</item>
/// </list>
/// </para>
/// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI.</para>
/// </summary>
public sealed class AcquisizionePesateDatamarsService : BaseServiceOperazioniZooBIZ, IAcquisizionePesateDatamarsService
{
    private const string ChiaveUrlDatamarsApi = "urlDatamarsApi";
    private const string ChiaveUrlDatamarsApiAuth = "urlDatamarsApiAuth";
    private static readonly TimeSpan TokenRenewalBuffer = TimeSpan.FromMinutes(10);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly ISecurityLayerDAL _securityLayerDal;
    private readonly IFabbricatiDatamarsRepository _fabbricatiRepository;
    private readonly IStagingPesateRepository _stagingPesateRepository;
    private readonly IStagingSessioniApiRepository _stagingSessioniApiRepository;
    private readonly IDatamarsHttpService _datamarsHttpService;
    private readonly IValidazionePayloadJsonDatamarsService _validazioneService;

    public AcquisizionePesateDatamarsService(
        IServiceProvider provider,
        IStringLocalizer<Resources.Messages> localizer)
        : base(provider, localizer)
    {
        _securityLayerDal = provider.GetRequiredService<ISecurityLayerDAL>();
        _fabbricatiRepository = provider.GetRequiredService<IFabbricatiDatamarsRepository>();
        _stagingPesateRepository = provider.GetRequiredService<IStagingPesateRepository>();
        _stagingSessioniApiRepository = provider.GetRequiredService<IStagingSessioniApiRepository>();
        _datamarsHttpService = provider.GetRequiredService<IDatamarsHttpService>();
        _validazioneService = provider.GetRequiredService<IValidazionePayloadJsonDatamarsService>();
    }

    /// <inheritdoc/>
    public async Task<RisultatoAcquisizionePesate> AcquisisciPesateAsync(
        DatamarsCredentials credenziali,
        AgronicaCoreParametriServer objParametriServer,
        AgronicaCoreParametriSuperServer objParametriSuperServer,
        CancellationToken cancellationToken = default)
    {
        var dataEsecuzione = DateTime.Now;

        // ── Step 1: Leggi URL base e URL auth da Configurazione_Siti ─────────
        var baseUrl = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync(
            ChiaveUrlDatamarsApi, objParametriServer, objParametriSuperServer);
        //baseUrl = "https://test-api-monitoring.livestock.datamars.com";
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException(
                $"Configurazione mancante: chiave '{ChiaveUrlDatamarsApi}' non trovata in Configurazione_Siti.");

        var authEndpoint = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync(
            ChiaveUrlDatamarsApiAuth, objParametriServer, objParametriSuperServer);
        //authEndpoint = "https://test-account.livestock.datamars.com/oauth2/token";
        if (string.IsNullOrWhiteSpace(authEndpoint))
            throw new InvalidOperationException(
                $"Configurazione mancante: chiave '{ChiaveUrlDatamarsApiAuth}' non trovata in Configurazione_Siti.");

        // ── Step 1b: Ottieni token OAuth2 ────────────────────────────────────
        var authResponse = await _datamarsHttpService.AuthenticateAsync(
            authEndpoint, credenziali, cancellationToken);

        var apiToken = authResponse.AccessToken;
        var tokenExpiresAt = DateTime.UtcNow.AddSeconds(authResponse.ExpiresIn);

        // ── Step 2: Estrai FarmID ─────────────────────────────────────────────
        var farmIds = await _fabbricatiRepository.GetFarmIdsAsync(objParametriServer);

        var totPesateAcquisite = 0;
        var totPesateScartate = 0;
        var totSessioni = 0;

        // ── Step 3–6: Per ogni Farm → per ogni sessione ───────────────────────
        foreach (var farmId in farmIds)
        {
            // ── Rinnova il token se scade entro il buffer ─────────────────────
            (apiToken, tokenExpiresAt) = await RenewTokenIfNeededAsync(
                apiToken, tokenExpiresAt, authEndpoint, credenziali, objParametriServer, cancellationToken);

            List<DatamarsIntegrationSessionItem> sessions;
            try
            {
                sessions = await _datamarsHttpService.GetIntegrationSessionsAsync(
                    farmId, baseUrl, apiToken, cancellationToken);
            }
            catch (ApiRetryExhaustedException ex)
            {
                LogError($"FarmID '{farmId}': API non raggiungibile dopo tutti i retry.", objParametriServer, ex);
                continue;
            }

            foreach (var session in sessions)
            {
                var sessionId = session.Id;
                var timestampInizio = DateTime.Now;

                // ── Rinnova il token se scade entro il buffer ─────────────────
                (apiToken, tokenExpiresAt) = await RenewTokenIfNeededAsync(
                    apiToken, tokenExpiresAt, authEndpoint, credenziali, objParametriServer, cancellationToken);

                // ── Deduplicazione (Step 3b) ──────────────────────────────────
                bool esistente;
                try
                {
                    esistente = await _stagingPesateRepository.SessioneEsistenteAsync(sessionId, objParametriServer);
                }
                catch (Exception ex)
                {
                    LogError($"Errore verifica deduplicazione sessione '{sessionId}'.", objParametriServer, ex);
                    continue;
                }

                if (esistente)
                    continue;

                // ── GET dettaglio sessione (Step 3c) ──────────────────────────
                DatamarsSessioneDettaglio sessioneDettaglio;
                try
                {
                    sessioneDettaglio = await _datamarsHttpService.GetSessioneDettaglioAsync(
                        sessionId, baseUrl, apiToken, cancellationToken);
                }
                catch (ApiRetryExhaustedException ex)
                {
                    LogError($"Sessione '{sessionId}': impossibile recuperare dettaglio da API.", objParametriServer, ex);
                    await PersistiSessioneErroreAsync(
                        sessionId, timestampInizio, 0, 0, 0, 0,
                        $"API non raggiungibile: {ex.Message}", ex.Tentativi, objParametriServer);
                    continue;
                }
                catch (JsonParsingException ex)
                {
                    LogError($"Sessione '{sessionId}': payload JSON non parsabile.", objParametriServer, ex);
                    await PersistiSessioneErroreAsync(
                        sessionId, timestampInizio, 0, 0, 0, 0,
                        $"JSON non valido: {ex.Message}", 1, objParametriServer);
                    continue;
                }

                // ── Validazione payload (Step 4 / DS09) ───────────────────────
                var validazione = _validazioneService.ValidaPayload(sessioneDettaglio, sessionId);
                var recordRicevuti = sessioneDettaglio.SessionAnimals.Count;
                var timestampFine = DateTime.Now;

                if (!validazione.ValidazioneOK)
                {
                    await PersistiSessioneErroreAsync(
                        sessionId, timestampInizio, recordRicevuti, 0, validazione.NumPesateScartate, 0,
                        validazione.ErrorMessage, 1, objParametriServer);

                    totPesateScartate += validazione.NumPesateScartate;
                    totSessioni++;
                    continue;
                }

                // ── Costruisci payload JSON atomico (Step 5) ──────────────────
                var payloadJson = JsonSerializer.Serialize(sessioneDettaglio, JsonOptions);

                // ── Persistenza atomica ACID (Step 6) ─────────────────────────
                try
                {
                    await OpenConnectionAsync(objParametriServer);

                    var stagingRecord = new StagingPesataRecord
                    {
                        IdSessione = sessionId,
                        TimestampInizioSessione = timestampInizio,
                        PayloadJson = payloadJson,
                        FlagValidazione = validazione.StatusValidazione,
                        IdFarm = farmId,
                        ErroreDescrizione = null
                    };

                    await _stagingPesateRepository.InsertAsync(stagingRecord, objParametriServer);

                    var sessioneRecord = new StagingSessioneApiRecord
                    {
                        IdSessione = sessionId,
                        TimestampInizio = timestampInizio,
                        TimestampFine = timestampFine,
                        RecordRicevuti = recordRicevuti,
                        RecordValidati = validazione.NumPesateValidate,
                        RecordErrori = validazione.NumPesateScartate,
                        RecordPersistiti = validazione.NumPesateValidate,
                        Status = "COMPLETATA",
                        NumeroTentativi = 1
                    };

                    await _stagingSessioniApiRepository.InsertAsync(sessioneRecord, objParametriServer);
                }
                catch (Exception ex)
                {
                    CloseTransaction(objParametriServer, rollback: true);
                    LogError($"Errore transazione per sessione '{sessionId}'. Rollback eseguito.", objParametriServer, ex);
                    throw new DatabaseTransactionException(sessionId, ex);
                }
                finally
                {
                    CloseConnection(objParametriServer);
                }

                totPesateAcquisite += validazione.NumPesateValidate;
                totPesateScartate += validazione.NumPesateScartate;
                totSessioni++;
            }
        }

        return new RisultatoAcquisizionePesate
        {
            RispostaOK = true,
            NumPesateAcquisite = totPesateAcquisite,
            NumSessioni = totSessioni,
            NumPesateScartate = totPesateScartate,
            DataEsecuzione = dataEsecuzione
        };
    }

    /// <summary>
    /// Rinnova il token OAuth2 se mancano meno di 10 minuti alla scadenza.
    /// Restituisce il token aggiornato e il nuovo timestamp di scadenza.
    /// </summary>
    /// <returns>Tupla (token, expiresAt): invariata se non serve rinnovo, aggiornata altrimenti.</returns>
    /// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI — Gestione della Scadenza del Token.</para>
    private async Task<(string Token, DateTime ExpiresAt)> RenewTokenIfNeededAsync(
        string currentToken,
        DateTime tokenExpiresAt,
        string authEndpoint,
        DatamarsCredentials credenziali,
        AgronicaCoreParametriServer objParametriServer,
        CancellationToken cancellationToken)
    {
        if (DateTime.UtcNow < tokenExpiresAt - TokenRenewalBuffer)
            return (currentToken, tokenExpiresAt);

        var authResponse = await _datamarsHttpService.AuthenticateAsync(
            authEndpoint, credenziali, cancellationToken);

        LogInformation("Token Datamars rinnovato.", objParametriServer);
        return (authResponse.AccessToken, DateTime.UtcNow.AddSeconds(authResponse.ExpiresIn));
    }

    /// <summary>
    /// Persiste un record di errore in STAGING_SESSIONI_API senza aprire una nuova transazione.
    /// Usato quando la sessione fallisce prima della fase di persistenza principale.
    /// </summary>
    private async Task PersistiSessioneErroreAsync(
        string sessionId,
        DateTime timestampInizio,
        int recordRicevuti,
        int recordValidati,
        int recordErrori,
        int recordPersistiti,
        string? erroreDescrizione,
        int numeroTentativi,
        AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            var record = new StagingSessioneApiRecord
            {
                IdSessione = sessionId,
                TimestampInizio = timestampInizio,
                TimestampFine = DateTime.Now,
                RecordRicevuti = recordRicevuti,
                RecordValidati = recordValidati,
                RecordErrori = recordErrori,
                RecordPersistiti = recordPersistiti,
                Status = "ERRORE",
                ErroreDescrizione = erroreDescrizione,
                NumeroTentativi = numeroTentativi
            };

            await _stagingSessioniApiRepository.InsertAsync(record, objParametriServer);
        }
        catch (Exception ex)
        {
            LogError($"Impossibile persistere record di errore per sessione '{sessionId}'.", objParametriServer, ex);
        }
    }

    /// <summary>
    /// Chiude la transazione corrente, con eventuale rollback.
    /// </summary>
    private void CloseTransaction(AgronicaCoreParametriServer objParametriServer, bool rollback)
    {
        if (rollback)
            CloseConnection(objParametriServer, RollbackTransaction: true);
    }
}
