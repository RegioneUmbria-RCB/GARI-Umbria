using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Operazione.DAL.DataLayer.Agenda;
using AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Destinazioni;
using AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti;
using AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti_Dettagli;
using AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.Exceptions;
using AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.PrevenzioneDuplicati;
using InData.Zoo.DataMars;
using OutData.Zoo.DataMars;
using AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.ValidazioneAnagrafica;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.OperazioniZoo;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.AgendaPesatura;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.StagingPesate;
using InData.Agenda;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text.Json;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.TrasformazioneStagingAgenda;

/// <summary>
/// Orchestratore del ciclo completo di trasformazione pesate staging â†’ operazioni agenda zoo.
/// <para>
/// Flusso (DS02-BL Step 1â€“7) con raggruppamento per data:
/// <list type="number">
///   <item>Legge batch di record da STAGING_PESATE con processato=0.</item>
///   <item>Deserializza <c>payload_json</c> in <see cref="DatamarsSessioneDettaglio"/>.</item>
///   <item>Raggruppa le pesate per data (parte data del timestamp, ignorando l'ora).</item>
///   <item>Per ogni gruppo data: DS05 validazione anagrafica/stalla per ogni pesata (con cache per LID).</item>
///   <item>DS06 prevenzione duplicati per il gruppo data (lista di Cod_Progetto).</item>
///   <item>Insert atomico: una Agenda + un Movimenti + N Movimenti_Dettagli (uno per pesata valida) + una Mov_Destinazioni.</item>
///   <item>Update STAGING_PESATE con id_agenda e num_pesate_accorpate.</item>
/// </list>
/// </para>
/// <para>Riferimento spec: DS02-BL TrasformazionePesateStagingAgenda.</para>
/// </summary>
public sealed class TrasformazionePesateStagingAgendaService : BaseServiceOperazioniZooBIZ, ITrasformazionePesateStagingAgendaService
{
    private const string UsernameImport = "SYSTEM_IMPORT";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IStagingPesateRepository _stagingPesateRepository;
    private readonly IValidazioneAnagraficaAnimaliMappingService _validazioneService;
    private readonly IPrevenzioneDuplicatiOperazioniAgendaService _duplicatiService;
    private readonly IAgendaPesaturaRepository _agendaPesaturaRepository;
    private readonly IOperazioniZoo _operazioniZoo;
    private readonly IAgenda _agenda;
    private readonly IMovimenti _movimenti;
    private readonly IMovimenti_Dettagli _movimentiDettagli;
    private readonly IMov_Destinazioni _movDestinazioni;

    public TrasformazionePesateStagingAgendaService(
        IServiceProvider provider,
        IStringLocalizer<Resources.Messages> localizer)
        : base(provider, localizer)
    {
        _stagingPesateRepository = provider.GetRequiredService<IStagingPesateRepository>();
        _validazioneService = provider.GetRequiredService<IValidazioneAnagraficaAnimaliMappingService>();
        _duplicatiService = provider.GetRequiredService<IPrevenzioneDuplicatiOperazioniAgendaService>();
        _agendaPesaturaRepository = provider.GetRequiredService<IAgendaPesaturaRepository>();
        _operazioniZoo = provider.GetRequiredService<IOperazioniZoo>();
        _agenda = provider.GetRequiredService<IAgenda>();
        _movimenti = provider.GetRequiredService<IMovimenti>();
        _movimentiDettagli = provider.GetRequiredService<IMovimenti_Dettagli>();
        _movDestinazioni = provider.GetRequiredService<IMov_Destinazioni>();
    }

    /// <inheritdoc/>
    public async Task<RisultatoTrasformazionePesate> TrasformaAsync(
        ParametriTrasformazionePesate parametri,
        AgronicaCoreParametriServer objParametriServer,
        CancellationToken cancellationToken = default)
    {
        var dataEsecuzione = DateTime.Now;
        var erroriDettagli = new List<ErroreDettaglioTrasformazione>();
        var numOperazioniCreate = 0;
        var numOperazioniErrore = 0;
        var numPesateProcessate = 0;
        var numSessioniProcessate = 0;
        var numPesatePerOperazione = new Dictionary<string, int>();

        // Step 1: Leggi batch STAGING_PESATE (processato=0)
        DataTable dtStaging;
        try
        {
            dtStaging = await _stagingPesateRepository.GetPesateNonProcessateAsync(parametri.BatchSize, objParametriServer);
        }
        catch (Exception ex)
        {
            LogError("Errore lettura STAGING_PESATE non processate.", objParametriServer, ex);
            throw;
        }

        foreach (DataRow stagingRow in dtStaging.Rows)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var stagingId = Convert.ToInt64(stagingRow["id"]);
            var idSessione = stagingRow["id_sessione"]?.ToString() ?? string.Empty;
            var payloadJson = stagingRow["payload_json"]?.ToString() ?? string.Empty;
            var timestampSessione = Convert.ToDateTime(stagingRow["timestamp_inizio_sessione"]);
            var farmId = stagingRow["id_farm"]?.ToString() ?? string.Empty;

            numSessioniProcessate++;

            // Step 2: Deserializza payload JSON
            DatamarsSessioneDettaglio sessioneDettaglio;
            try
            {
                sessioneDettaglio = JsonSerializer.Deserialize<DatamarsSessioneDettaglio>(payloadJson, JsonOptions)
                    ?? throw new InvalidOperationException($"Deserializzazione payload JSON restituita null per sessione '{idSessione}'.");
            }
            catch (Exception ex)
            {
                LogError($"Sessione '{idSessione}': payload JSON non deserializzabile.", objParametriServer, ex);
                await _stagingPesateRepository.UpdateErroreAsync(stagingId,
                    $"JSON non deserializzabile: {ex.Message}", objParametriServer);
                continue;
            }

            //var farmId = sessioneDettaglio.Id;
            //var piva = objParametriServer.PivaSuperUser;
            string piva = string.Empty;

            // Verifica mapping stalla (bloccante per la sessione): recupera la PIVA reale
            if (!parametri.SkipAnagraficaValidation)
            {
                piva = await _validazioneService.GetPivaDaFarmIdAsync(farmId, objParametriServer) ?? string.Empty;

                if (string.IsNullOrEmpty(piva))
                {
                    LogError($"Sessione '{idSessione}': FarmID='{farmId}' non mappato. Sessione saltata.", objParametriServer, null);
                    var errMsg = new StallaMappingException(farmId).Message;
                    await _stagingPesateRepository.UpdateErroreAsync(stagingId, errMsg, objParametriServer);

                    erroriDettagli.Add(new ErroreDettaglioTrasformazione
                    {
                        IdSessione = idSessione,
                        Lid = string.Empty,
                        TipoErrore = nameof(StallaMappingException),
                        Messaggio = errMsg
                    });
                    numOperazioniErrore += sessioneDettaglio.SessionAnimals.Count;
                    numPesateProcessate += sessioneDettaglio.SessionAnimals.Count;
                    continue;
                }
            }

            // Step 1b: Raggruppa le pesate per data (ignora la parte ora)
            var pesatePerData = new Dictionary<DateOnly, List<DatamarsAnimalePesata>>();
            foreach (var pesata in sessioneDettaglio.SessionAnimals)
            {
                numPesateProcessate++;

                if (!DateTime.TryParse(pesata.Timestamp, null, System.Globalization.DateTimeStyles.RoundtripKind, out var ts))
                {
                    var lid = pesata.Animal?.LifetimeIdentifierTag?.Value ?? string.Empty;
                    numOperazioniErrore++;
                    erroriDettagli.Add(new ErroreDettaglioTrasformazione
                    {
                        IdSessione = idSessione,
                        Lid = lid,
                        TipoErrore = nameof(InvalidTimestampException),
                        Messaggio = $"Timestamp '{pesata.Timestamp}' non valido."
                    });
                    continue;
                }

                var dateKey = DateOnly.FromDateTime(ts.ToUniversalTime());
                if (!pesatePerData.TryGetValue(dateKey, out var lista))
                {
                    lista = new List<DatamarsAnimalePesata>();
                    pesatePerData[dateKey] = lista;
                }
                lista.Add(pesata);
            }

            // Step 2: Elaborazione per ogni data nel raggruppamento
            foreach (var (dateKey, pesateGruppo) in pesatePerData)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var dataGruppo = dateKey.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                var dateKeyStr = dateKey.ToString("yyyy-MM-dd");

                // Accumula pesate valide e relative info per il gruppo
                var pesateValide = new List<(DatamarsAnimalePesata Pesata, int CodProgetto, int SaCod, int StaNum, int RaggruppamentoCod)>();

                // Step 2a: Validazione e lookup per ogni pesata della data 
                foreach (var pesata in pesateGruppo)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var lid = pesata.Animal?.LifetimeIdentifierTag?.Value ?? string.Empty;
                    DateTime dataPesata = DateTime.Parse(pesata.Timestamp!).ToUniversalTime();

                    ValidazioneAnagraficaResult validazione;
                    if (!parametri.SkipAnagraficaValidation)
                    {
                        try
                        {
                            validazione = await _validazioneService.ValidaAsync(lid, farmId, dataPesata, objParametriServer);
                        }
                        catch (Exception ex)
                        {
                            LogError($"Sessione '{idSessione}', LID='{lid}': errore imprevisto validazione.", objParametriServer, ex);
                            erroriDettagli.Add(new ErroreDettaglioTrasformazione
                            {
                                IdSessione = idSessione,
                                Lid = lid,
                                TipoErrore = ex.GetType().Name,
                                Messaggio = ex.Message
                            });
                            await _stagingPesateRepository.UpdateErroreAsync(stagingId,
                                $"Errore validazione LID={lid}: {ex.Message}", objParametriServer);
                            continue;
                        }

                        if (!validazione.ValidazioneOK)
                        {
                            erroriDettagli.Add(new ErroreDettaglioTrasformazione
                            {
                                IdSessione = idSessione,
                                Lid = lid,
                                TipoErrore = validazione.ErrorType ?? validazione.StatusValidazione,
                                Messaggio = validazione.Messaggio
                            });
                            await _stagingPesateRepository.UpdateErroreAsync(stagingId,
                                $"{validazione.StatusValidazione}: {validazione.Messaggio}", objParametriServer);
                            continue;
                        }
                    }
                    else
                    {
                        validazione = await _validazioneService.ValidaAsync(lid, farmId, dataPesata, objParametriServer);
                        if (!validazione.ValidazioneOK)
                        {
                            await _stagingPesateRepository.UpdateErroreAsync(stagingId,
                                $"{validazione.StatusValidazione}: {validazione.Messaggio}", objParametriServer);
                            continue;
                        }
                    }

                    var codProgetto = validazione.CodProgetto!.Value;
                    var saCod = validazione.SaCod!.Value;
                    var staNum = validazione.StaNum!.Value;

                    // Lookup RaggruppamentoCod (box animale alla data pesata)
                    int raggruppamentoCod;
                    try
                    {
                        var dtGiacenze = await _operazioniZoo.Leggi_GiacenzeAsync(
                            Piva: piva,
                            Sa_Cod: saCod,
                            STA_NUM: staNum,
                            Raggruppamento_Cod: 0,
                            Cod_Animale: 0,
                            Data: dataPesata,
                            objParametriServer: objParametriServer,
                            Matricola: lid);

                        raggruppamentoCod = (dtGiacenze?.Rows.Count > 0 && dtGiacenze.Rows[0]["Raggruppamento_Cod"] != DBNull.Value)
                            ? Convert.ToInt32(dtGiacenze.Rows[0]["Raggruppamento_Cod"])
                            : 0;
                    }
                    catch (Exception ex)
                    {
                        LogError($"Sessione '{idSessione}', LID='{lid}': errore lookup giacenze.", objParametriServer, ex);
                        erroriDettagli.Add(new ErroreDettaglioTrasformazione
                        {
                            IdSessione = idSessione,
                            Lid = lid,
                            TipoErrore = ex.GetType().Name,
                            Messaggio = $"Errore lookup RaggruppamentoCod: {ex.Message}"
                        });
                        await _stagingPesateRepository.UpdateErroreAsync(stagingId,
                            $"Errore lookup giacenze LID={lid}: {ex.Message}", objParametriServer);
                        continue;
                    }

                    pesateValide.Add((pesata, codProgetto, saCod, staNum, raggruppamentoCod));
                }

                // Step 2b: Check pesate valide per data
                if (pesateValide.Count == 0)
                {
                    LogWarning($"Sessione '{idSessione}', data='{dateKeyStr}': nessuna pesata valida. Data scartata.", objParametriServer);
                    numOperazioniErrore++;
                    continue;
                }

                // Usa la stalla del primo animale valido come riferimento per la destinazione
                var (_, _, refSaCod, refStaNum, refRaggruppamentoCod) = pesateValide[0];

                // Step 2c: Prevenzione duplicati per data
                if (!parametri.SkipAnagraficaValidation)
                {
                    var codProgettiGruppo = pesateValide.Select(p => p.CodProgetto).Distinct().ToList();
                    DuplicatoCheckResult duplicato;
                    try
                    {
                        duplicato = await _duplicatiService.CheckDuplicatoPerDataAsync(
                            piva, refSaCod, refStaNum, codProgettiGruppo, dataGruppo, objParametriServer);
                    }
                    catch (Exception ex)
                    {
                        LogError($"Sessione '{idSessione}', data='{dateKeyStr}': errore check duplicati.", objParametriServer, ex);
                        numOperazioniErrore++;
                        erroriDettagli.Add(new ErroreDettaglioTrasformazione
                        {
                            IdSessione = idSessione,
                            Lid = string.Empty,
                            TipoErrore = ex.GetType().Name,
                            Messaggio = $"Errore check duplicati per data {dateKeyStr}: {ex.Message}"
                        });
                        continue;
                    }

                    if (duplicato.IsDuplicate)
                    {
                        LogWarning($"Sessione '{idSessione}', data='{dateKeyStr}': duplicato rilevato. Data scartata.", objParametriServer);
                        numOperazioniErrore++;
                        erroriDettagli.Add(new ErroreDettaglioTrasformazione
                        {
                            IdSessione = idSessione,
                            Lid = string.Empty,
                            TipoErrore = nameof(DuplicateWeightException),
                            Messaggio = $"Operazione di pesatura giÃ  presente per data {dateKeyStr}."
                        });
                        await _stagingPesateRepository.UpdateErroreAsync(stagingId,
                            $"Duplicato: operazione giÃ  presente per data {dateKeyStr}.", objParametriServer);
                        continue;
                    }
                }

                // Step 2d: Creazione operazione agenda unificata per data
                var adInizio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
                var adFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);

                try
                {
                    await OpenConnectionAsync(objParametriServer);

                    // 1. Una sola Agenda per data
                    var wAgenda = new WriteAgenda
                    {
                        Piva = piva,
                        Sa_Cod = refSaCod,
                        Id_Agenda = 0,
                        Lav_Cod = 3033,
                        Des_Lib = "Pesatura Animali",
                        Sta_Num = refStaNum,
                        Validita_Inizio = dataGruppo,
                        Validita_Fine = adFine,
                        Blocco_Data = adInizio,
                        Blocco_Flag = 0,
                        Inviato = 0,
                        Stato_Export = 0
                    };
                    var idAgenda = await _agenda.ScriviModificaAsync(wAgenda, objParametriServer);

                    // 2. Un solo Movimenti per data
                    var wMov = new WriteMovimenti
                    {
                        Piva = piva,
                        Sa_Cod = 0,
                        Id_Agenda = idAgenda,
                        Id_Mov = 0,
                        Cau_Mov = 3800,
                        Mov_Desc = string.Empty,
                        Data_Movimento = dataGruppo,
                        Validita_Inizio = adInizio,
                        Validita_Fine = adFine
                    };
                    var idMov = await _movimenti.ScriviModificaAsync(wMov, objParametriServer);

                    // 3. Un Movimenti_Dettagli per ogni pesata valida del gruppo
                    var lastIdMovDet = 0;
                    foreach (var (pesata, codProgetto, saCod, _, _) in pesateValide)
                    {
                        var lid = pesata.Animal?.LifetimeIdentifierTag?.Value ?? string.Empty;
                        var wDet = new WriteMovDettagli
                        {
                            Piva = piva,
                            Sa_Cod = saCod,
                            Id_Agenda = idAgenda,
                            Id_Mov = idMov,
                            Id_Mov_Det = 0,
                            Elem_Cod = 300,
                            Udm_Cod = 2,
                            Qta = (double)pesata.Weight,
                            Cod_Progetto = codProgetto,
                            Mov_Det_Des = $"Pesatura {lid}",
                            Validita_Inizio = adInizio,
                            Validita_Fine = adFine
                        };
                        lastIdMovDet = await _movimentiDettagli.ScriviModificaAsync(wDet, objParametriServer);
                    }

                    // 4. Una sola Mov_Destinazioni per il gruppo (basata sul primo animale)
                    int idDest = refRaggruppamentoCod > 0 ? refRaggruppamentoCod : refStaNum;
                    int tipoDest = refRaggruppamentoCod > 0
                        ? TIPO_DESTINAZIONE.TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA
                        : TIPO_DESTINAZIONE.STALLA;
                    var wDest = new WriteMovDestinazioni
                    {
                        Piva = piva,
                        Sa_Cod = refSaCod,
                        Id_Agenda = idAgenda,
                        Id_Mov = idMov,
                        Id_Mov_Det = lastIdMovDet,
                        Id_Destinazione = idDest,
                        Tipo_Destinazione = tipoDest,
                        Validita_Inizio = adInizio,
                        Validita_Fine = adFine
                    };
                    await _movDestinazioni.ScriviModificaAsync(wDest, objParametriServer);

                    CloseConnection(objParametriServer);

                    numOperazioniCreate++;
                    numPesatePerOperazione[dateKeyStr] = pesateValide.Count;

                    // Aggiorna staging con id_agenda e num_pesate_accorpate
                    await _stagingPesateRepository.UpdateProcessatoConAccorpamentoAsync(
                        stagingId, idAgenda, pesateValide.Count, objParametriServer);
                }
                catch (Exception ex)
                {
                    CloseConnection(objParametriServer, RollbackTransaction: true);
                    LogError($"Sessione '{idSessione}', data='{dateKeyStr}': rollback transazione.", objParametriServer, ex);
                    numOperazioniErrore++;
                    erroriDettagli.Add(new ErroreDettaglioTrasformazione
                    {
                        IdSessione = idSessione,
                        Lid = string.Empty,
                        TipoErrore = "TransactionRollbackException",
                        Messaggio = $"Data {dateKeyStr}: {ex.Message}"
                    });
                    await _stagingPesateRepository.UpdateErroreAsync(stagingId,
                        $"Rollback transazione per data {dateKeyStr}: {ex.Message}", objParametriServer);
                }
            }
        }

        return new RisultatoTrasformazionePesate
        {
            NumOperazioniCreate = numOperazioniCreate,
            NumOperazioniAccorpate = numOperazioniCreate,
            NumOperazioniErrore = numOperazioniErrore,
            NumPesateProcessate = numPesateProcessate,
            NumSessioniProcessate = numSessioniProcessate,
            NumPesatePerOperazione = numPesatePerOperazione,
            DataEsecuzione = dataEsecuzione,
            ErroriDettagli = erroriDettagli
        };
    }
}

