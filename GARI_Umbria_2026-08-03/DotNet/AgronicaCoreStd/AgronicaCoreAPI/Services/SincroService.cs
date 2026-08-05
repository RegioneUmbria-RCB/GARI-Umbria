using AgronicaCoreAPI.adapters;
using AgronicaCoreAPI.Controllers;
using AgronicaCoreAPI.Messaging.Contracts;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreVarieBizSTD;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static AgronicaCoreDataProviderSTD.CostantiPersonalizzate;
using static AgronicaCoreDataProviderSTD.TipiEnumerativi;
using static AgronicaCoreModelsSTD.attivita.Attivita;
using DocumentoPerScarico = AgronicaCoreModelloSTD.DocumentoPerScarico;

namespace AgronicaCoreAPI.Services
{
    /// <summary>
    /// Implementazione di ISincroService.
    /// Centralizza la logica di scrittura su CoreWS condividendola tra
    /// il path sincrono (controller REST) e il path asincrono (RabbitMQ handler).
    /// </summary>
    public class SincroService : ISincroService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SincroService> _logger;
        private readonly string _defaultCoreWSBaseURL;

        public SincroService(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<SincroService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _defaultCoreWSBaseURL = config.GetValue<string>("AgronicaWSBaseURL") ?? "";
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        private CoreWSController CreateCoreWS(AuthInfo auth)
        {
            var hc = _httpClientFactory.CreateClient("base");
            var baseUrl = auth?.CoreWSBaseURL ?? _defaultCoreWSBaseURL;
            return new CoreWSController(hc, baseUrl, auth?.Bearer, null, isRabbitMQ: true);
        }

        private static CoreWSRequest<CoreWS_Generic<T>> BuildRequest<T>(T data, AuthInfo auth)
        {
            var objP = new CoreWS_GenericObjP
            {
                objP_super_server = auth?.ObjP_super_server,
                objP_server = auth?.ObjP_server,
                objP_utenti = auth?.ObjP_utenti,
                user_Agent = auth?.UserAgent
            };
            var generic = new CoreWS_Generic<T>(objP, data);
            return new CoreWSRequest<CoreWS_Generic<T>>(generic);
        }

        private static SyncResult FromRisposta(RispostaStandard risposta, string guid)
        {
            if (risposta == null)
                return SyncResult.Fail("Risposta nulla da CoreWS");
            return risposta.RispostaOK
                ? SyncResult.Ok(guid, risposta.RispostaStringa?.ToString())
                : SyncResult.Fail(risposta.Errore ?? "Errore CoreWS senza dettagli");
        }

        // ─────────────────────────────────────────────────────────────────────
        // Attività
        // ─────────────────────────────────────────────────────────────────────

        public Task<SyncResult> ScriviAttivitaAsync(Attivita attivita, AuthInfo auth, CancellationToken ct = default)
        {
            try
            {
                var adapter = new AttivitaAdapter();
                var dati = adapter.initDati();
                dati.cancellato = attivita.cancellato;
                dati.guid = attivita.guid;
                dati.versione = attivita.versione;
                dati.riferimentoPianificata = attivita.riferimentoPianificata;
                dati.isPianificata = attivita.stato == Stati.Da_Eseguire;

                if (!attivita.cancellato)
                {
                    Job job = attivita.job;
                    adapter.scriviAttivita(dati, attivita, job, auth?.User);
                    if (!adapter.isZero(attivita.latitude) && !adapter.isZero(attivita.longitude))
                        dati.posizione = (attivita.latitude + "|" + attivita.longitude).Replace(",", ".");
                    adapter.scriviDatiComuni(dati, auth?.Username);
                }

                var result = CreateCoreWS(auth).ScriviAttivita(BuildRequest(dati, auth));
                return Task.FromResult(FromRisposta(result, attivita.guid));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScriviAttivitaAsync error guid={Guid}", attivita?.guid);
                return Task.FromResult(SyncResult.Fail(ex.Message));
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Attività Miste
        // ─────────────────────────────────────────────────────────────────────

        public Task<SyncResult> ScriviAttivitaMisteAsync(List<Attivita> attivita, AuthInfo auth, CancellationToken ct = default)
        {
            try
            {
                var adapter = new AttivitaAdapter();
                var dati = adapter.initDati();
                dati.cancellato = attivita[0].cancellato;
                dati.guid = attivita[0].guid;
                dati.riferimentoPianificata = attivita[0].riferimentoPianificata;
                dati.isPianificata = attivita[0].stato == Stati.Da_Eseguire;

                if (!attivita[0].cancellato)
                {
                    foreach (var a in attivita)
                    {
                        adapter.scriviAttivita(dati, a, a.job, auth?.User);
                        if (string.IsNullOrEmpty(dati.posizione) && !adapter.isZero(a.latitude) && !adapter.isZero(a.longitude))
                            dati.posizione = (a.latitude + "|" + a.longitude).Replace(",", ".");
                    }
                    adapter.scriviDatiComuni(dati, auth?.Username);
                }

                var result = CreateCoreWS(auth).ScriviAttivita(BuildRequest(dati, auth));
                return Task.FromResult(FromRisposta(result, attivita[0].guid));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScriviAttivitaMisteAsync error");
                return Task.FromResult(SyncResult.Fail(ex.Message));
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Rilievo
        // ─────────────────────────────────────────────────────────────────────

        public Task<SyncResult> ScriviRilievoAsync(Attivita rilievo, AuthInfo auth, CancellationToken ct = default)
        {
            try
            {
                var result = CreateCoreWS(auth).ScriviRilievo(BuildRequest(rilievo, auth));
                return Task.FromResult(FromRisposta(result, rilievo.guid));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScriviRilievoAsync error guid={Guid}", rilievo?.guid);
                return Task.FromResult(SyncResult.Fail(ex.Message));
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Visite
        // ─────────────────────────────────────────────────────────────────────

        public Task<SyncResult> ScriviVisiteAsync(Attivita attivita, AuthInfo auth, CancellationToken ct = default)
        {
            try
            {
                var adapter = new VisiteAdapter();
                var dati = adapter.initDati();
                dati.cancellato = attivita.cancellato;
                dati.guid = attivita.guid;

                if (!attivita.cancellato)
                {
                    Job job = attivita.job;
                    Job job2 = null;

                    if (job.getTipo() == TipiJob.JOB_COMPOSITE)
                    {
                        var jobComposito = (JobComposito)attivita.job;
                        job = jobComposito.lavorazione;
                        job2 = jobComposito.attivitaCDG;
                    }

                    adapter.scriviAttivita(dati, attivita, job2 != null ? job2.getCodice() : 0, auth?.User);
                    adapter.scriviDatiComuni(dati, auth?.Username);

                    if (job.getCodice() != LAVCOD_VISITA && !attivita.cancellato)
                    {
                        var rilieviAdapter = new AttivitaAdapter();
                        var rilievi = rilieviAdapter.initDati();
                        if (!adapter.isZero(attivita.latitude) && !adapter.isZero(attivita.longitude))
                            rilievi.posizione = (attivita.latitude + "|" + attivita.longitude).Replace(",", ".");
                        rilieviAdapter.scriviAttivita(rilievi, attivita, attivita.job, auth?.User);
                        rilieviAdapter.scriviDatiComuni(rilievi, auth?.Username);
                        dati.VisiteRilievi = rilievi;
                    }
                }

                var result = CreateCoreWS(auth).ScriviVisite(BuildRequest(dati, auth));
                return Task.FromResult(FromRisposta(result, attivita.guid));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScriviVisiteAsync error guid={Guid}", attivita?.guid);
                return Task.FromResult(SyncResult.Fail(ex.Message));
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Documenti
        // ─────────────────────────────────────────────────────────────────────

        public Task<SyncResult> ScriviDocumentiAsync(DocumentoPerScarico documento, AuthInfo auth, CancellationToken ct = default)
        {
            try
            {
                var result = CreateCoreWS(auth).ScriviDocumenti(BuildRequest(documento, auth));
                return Task.FromResult(FromRisposta(result, documento.guid));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScriviDocumentiAsync error guid={Guid}", documento?.guid);
                return Task.FromResult(SyncResult.Fail(ex.Message));
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Movimenti
        // ─────────────────────────────────────────────────────────────────────

        public Task<SyncResult> ScriviMovimentiAsync(List<MovimentoDiMagazzino> movimenti, AuthInfo auth, CancellationToken ct = default)
        {
            try
            {
                var result = CreateCoreWS(auth).ScriviMovimenti(BuildRequest(movimenti, auth));
                return Task.FromResult(FromRisposta(result, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScriviMovimentiAsync error");
                return Task.FromResult(SyncResult.Fail(ex.Message));
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Acquisto
        // ─────────────────────────────────────────────────────────────────────

        public Task<SyncResult> ScriviAcquistoAsync(Acquisto acquisto, AuthInfo auth, CancellationToken ct = default)
        {
            try
            {
                var result = CreateCoreWS(auth).ScriviAcquisto(BuildRequest(acquisto, auth));
                return Task.FromResult(FromRisposta(result, acquisto.guid));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScriviAcquistoAsync error guid={Guid}", acquisto?.guid);
                return Task.FromResult(SyncResult.Fail(ex.Message));
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Manutenzioni
        // ─────────────────────────────────────────────────────────────────────

        public Task<SyncResult> ScriviManutenzioniAsync(List<Manutenzione> manutenzioni, AuthInfo auth, CancellationToken ct = default)
        {
            try
            {
                var result = CreateCoreWS(auth).ScriviManutenzioni(BuildRequest(manutenzioni, auth));
                return Task.FromResult(FromRisposta(result, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScriviManutenzioniAsync error");
                return Task.FromResult(SyncResult.Fail(ex.Message));
            }
        }
    }
}
