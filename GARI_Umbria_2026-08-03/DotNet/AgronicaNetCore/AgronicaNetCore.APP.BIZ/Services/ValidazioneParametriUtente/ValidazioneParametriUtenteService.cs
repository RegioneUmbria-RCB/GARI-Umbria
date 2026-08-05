using System.Data;
using System.Diagnostics;
using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaNetCore.APP.BIZ.Common;
using Microsoft.Extensions.Options;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.UtentiImpostazioni;
using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.APP.BIZ.Services.ImpostazioniApp;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.ParamUtenteDatiAzienda;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazioni;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.SpecieVegetali;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.ParamUtenteDatiComuni;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioniFiltroMono;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static AgronicaNetCore.APP.BIZ.Services.ValidazioneParametriUtente.ValidazioneParametriUtenteDatiComuniResult;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.APP.BIZ.Services.ValidazioneParametriUtente;

/// <summary>
/// Validates user API call parameters, permissions and visibility filters for the
/// synchronisation flows of common data and company data.
/// Ref: DS07-BL – Nome: ValidazioneParametriUtente.
/// Ref: DS01-BL – Nome: LetturaeConfrontoParametriUtente.
/// </summary>
public sealed class ValidazioneParametriUtenteService : IValidazioneParametriUtenteService
{
    private readonly SincroWeb2AppSettings _settings;
    private readonly IParamUtenteDatiComuni _paramUtenteDatiComuni;
    private readonly IParamUtenteDatiAzienda _paramUtenteDatiAzienda;
    private readonly IImpostazioniAppService _impostazioniAppService;
    private readonly ISpecieVegetali _specieVegetali;
    private readonly IOperazioni _operazioni;
    private readonly IUtentiImpostazioni _utentiImpostazioni;
    private readonly IUtentiImpostazioniFiltroMono _utentiImpostazioniFiltroMono;

    private readonly IUtentiVisibilitaAppoggio _utentiVisibilitaAppoggio;

    private readonly ILoggingService _loggingService;

    // Thread-safe, shared serializer used for deterministic JSON normalisation.
    // Ref: DS07-BL – Regole di Business: "Normalizzare JSON ricevuti (sorted keys alfabeticamente, minimal whitespace)".
    private static readonly JsonSerializerSettings _normalizedJsonSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.None,
        ContractResolver = new AlphabeticalContractResolver(),
    };

    public ValidazioneParametriUtenteService(
        IParamUtenteDatiComuni paramUtenteDatiComuni,
        IParamUtenteDatiAzienda paramUtenteDatiAzienda,
        IImpostazioniAppService impostazioniAppService,
        ISpecieVegetali specieVegetali,
        IOperazioni operazioni,
        IUtentiImpostazioni utentiImpostazioni,
        IUtentiImpostazioniFiltroMono utentiImpostazioniFiltroMono,
        IUtentiVisibilitaAppoggio utentiVisibilitaAppoggio,
        ILoggingService loggingService,
        IOptions<SincroWeb2AppSettings> settings
    )
    {
        _paramUtenteDatiComuni = paramUtenteDatiComuni;
        _paramUtenteDatiAzienda = paramUtenteDatiAzienda;
        _impostazioniAppService = impostazioniAppService;
        _specieVegetali = specieVegetali;
        _operazioni = operazioni;
        _utentiImpostazioni = utentiImpostazioni;
        _utentiImpostazioniFiltroMono = utentiImpostazioniFiltroMono;
        _utentiVisibilitaAppoggio = utentiVisibilitaAppoggio;
        _loggingService = loggingService;
        _settings = settings.Value;
    }

    /// <inheritdoc/>
    public async Task<ValidazioneParametriUtenteDatiComuniResult> ValidaParametriDatiComuniAsync(
        string paramRichiamoApiRicevuto,
        AgronicaCoreParametriUtenti objParametriUtenti,
        AgronicaCoreParametriServer objParametriServer
    )
    {
        var sw = Stopwatch.StartNew();

        // Fail-fast: validate and normalise the received parameter before any async work.
        // Ref: DS07-BL – Eccezioni: InvalidParameterFormatException.
        string paramRichiamoNormalizzato = NormalizzaJson(paramRichiamoApiRicevuto);

        // Read current permissions and visibility from their respective sources.
        // These are always needed regardless of first-access or comparison path.
        // Ref: DS07-BL – Recupero Permessi Correnti; Recupero Filtri Visibilità Correnti.
        var permessiAttuali = await LeggiPermessiUtenteNormalizzatiAsync(
            objParametriUtenti,
            objParametriServer
        );
        WarnIfValidationTimeout(sw, objParametriUtenti.UtenteUsername, objParametriServer);

        var visibilitaAttuale = await LeggiVisibilitaUtenteNormalizzataAsync(
            objParametriUtenti,
            objParametriServer
        );
        WarnIfValidationTimeout(sw, objParametriUtenti.UtenteUsername, objParametriServer);

        // Read stored parameters for this user. An empty result indicates first access.
        // Ref: DS07-BL – Regole di Business: Query tabella app_param_utente_daticomuni_web2app.
        DataTable recordEsistente;
        try
        {
            recordEsistente = await _paramUtenteDatiComuni.LeggiParametriUtenteAsync(
                objParametriUtenti.UtenteUsername,
                objParametriServer
            );
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                $"Errore nella lettura da app_param_utente_daticomuni_web2app per utente '{objParametriUtenti.UtenteUsername}'.",
                ex
            );
        }
        WarnIfValidationTimeout(sw, objParametriUtenti.UtenteUsername, objParametriServer);

        // First access: no prior record found.
        // Ref: DS07-BL – Regole di Business: Se record non esiste (primo accesso).
        if (recordEsistente.Rows.Count == 0)
        {
            return new ValidazioneParametriUtenteDatiComuniResult(
                richiediDatasetCompleto: true,
                motivo: MotivoValidazione.PrimoAccesso,
                paramPermessiEstratti: permessiAttuali.PermessiUtenti,
                paramPermessiEstrattiJson: permessiAttuali.PermessiUtentiJson,
                paramVisibilitaEstratti: visibilitaAttuale.VisibilitaUtente,
                paramVisibilitaEstrattiJson: visibilitaAttuale.VisibilitaUtenteJson,
                validationTimeMs: sw.ElapsedMilliseconds
            );
        }

        // Record exists: compare each stored value against the current value.
        // Ref: DS07-BL – Regole di Business: confronto stringa-per-stringa dopo normalizzazione.
        DataRow row = recordEsistente.Rows[0];
        string storedRichiamoApiNorm = NormalizzaJson(
            row["param_richiamo_api"]?.ToString() ?? string.Empty
        );
        string storedPermessiNorm = NormalizzaJson(
            row["param_permessi_utente"]?.ToString() ?? string.Empty
        );
        string storedVisibilitaNorm = NormalizzaJson(
            row["param_visibilita_utente"]?.ToString() ?? string.Empty
        );

        if (
            !string.Equals(
                paramRichiamoNormalizzato,
                storedRichiamoApiNorm,
                StringComparison.Ordinal
            )
        )
        {
            return new ValidazioneParametriUtenteDatiComuniResult(
                richiediDatasetCompleto: true,
                motivo: MotivoValidazione.ParamRichiamoDiverso,
                paramPermessiEstratti: permessiAttuali.PermessiUtenti,
                paramPermessiEstrattiJson: permessiAttuali.PermessiUtentiJson,
                paramVisibilitaEstratti: visibilitaAttuale.VisibilitaUtente,
                paramVisibilitaEstrattiJson: visibilitaAttuale.VisibilitaUtenteJson,
                validationTimeMs: sw.ElapsedMilliseconds
            );
        }

        if (
            !string.Equals(
                permessiAttuali.PermessiUtentiJson,
                storedPermessiNorm,
                StringComparison.Ordinal
            )
        )
        {
            return new ValidazioneParametriUtenteDatiComuniResult(
                richiediDatasetCompleto: true,
                motivo: MotivoValidazione.ParamPermessiDiverso,
                paramPermessiEstratti: permessiAttuali.PermessiUtenti,
                paramPermessiEstrattiJson: permessiAttuali.PermessiUtentiJson,
                paramVisibilitaEstratti: visibilitaAttuale.VisibilitaUtente,
                paramVisibilitaEstrattiJson: visibilitaAttuale.VisibilitaUtenteJson,
                validationTimeMs: sw.ElapsedMilliseconds
            );
        }

        if (
            !string.Equals(
                visibilitaAttuale.VisibilitaUtenteJson,
                storedVisibilitaNorm,
                StringComparison.Ordinal
            )
        )
        {
            return new ValidazioneParametriUtenteDatiComuniResult(
                richiediDatasetCompleto: true,
                motivo: MotivoValidazione.FiltriVisibilitaDiversi,
                paramPermessiEstratti: permessiAttuali.PermessiUtenti,
                paramPermessiEstrattiJson: permessiAttuali.PermessiUtentiJson,
                paramVisibilitaEstratti: visibilitaAttuale.VisibilitaUtente,
                paramVisibilitaEstrattiJson: visibilitaAttuale.VisibilitaUtenteJson,
                validationTimeMs: sw.ElapsedMilliseconds
            );
        }

        return new ValidazioneParametriUtenteDatiComuniResult(
            richiediDatasetCompleto: false,
            motivo: MotivoValidazione.ParametriCorrispondenti,
            paramPermessiEstratti: permessiAttuali.PermessiUtenti,
            paramPermessiEstrattiJson: permessiAttuali.PermessiUtentiJson,
            paramVisibilitaEstratti: visibilitaAttuale.VisibilitaUtente,
            paramVisibilitaEstrattiJson: visibilitaAttuale.VisibilitaUtenteJson,
            validationTimeMs: sw.ElapsedMilliseconds
        );
    }

    /// <summary>
    /// Reads current user permissions via <see cref="IImpostazioniAppService"/> and returns
    /// them serialised to a normalised JSON string.
    /// Ref: DS07-BL – Recupero Permessi Correnti; LeggiPermessiUtenteNormalizzati.
    /// </summary>
    private async Task<(
        string PermessiUtentiJson,
        PermessiUtenteSincronizzazioneEntity PermessiUtenti
    )> LeggiPermessiUtenteNormalizzatiAsync(
        AgronicaCoreParametriUtenti objParametriUtenti,
        AgronicaCoreParametriServer objParametriServer
    )
    {
        PermessiUtenteSincronizzazioneEntity? permessiEntity;

        try
        {
            permessiEntity = await _impostazioniAppService.LeggiPermessiAppAsync(
                objParametriUtenti,
                objParametriServer
            );
        }
        catch (Exception ex)
        {
            throw new SetupSystemUnavailableException(
                "Impossibile leggere permessi da sistema setup.",
                ex
            );
        }

        if (permessiEntity == null)
            throw new SetupSystemUnavailableException(
                "Impossibile leggere permessi utente da IImpostazioniAppService: risultato null."
            );

        return (
            JsonConvert.SerializeObject(permessiEntity, _normalizedJsonSettings),
            permessiEntity
        );
    }

    /// <summary>
    /// Aggregates visibility filters from four sources (vegetable groups, crop species,
    /// varieties, operations) and returns them serialised to a normalised JSON string.
    /// Ref: DS07-BL – Recupero Filtri Visibilità Correnti; LeggiVisibilitaUtenteNormalizzata.
    /// </summary>
    private async Task<(
        string VisibilitaUtenteJson,
        ParamVisibilitaUtente VisibilitaUtente
    )> LeggiVisibilitaUtenteNormalizzataAsync(
        AgronicaCoreParametriUtenti objParametriUtenti,
        AgronicaCoreParametriServer objParametriServer
    )
    {
        var visibilitaAttuale = new ParamVisibilitaUtente();

        try
        {
            // 1. Visibility: vegetable groups.
            // Ref: DS07-BL – Visibilità GruppoVegetale.
            DataTable visibilitaGruppiVegetaliDt =
                await _specieVegetali.GruppoVegetale_GestioneFiltroUtente_LeggiAsync(
                    0,
                    objParametriUtenti,
                    objParametriServer
                );

            // Sostituisci questa parte:
            // visibilitaAttuale.CodiciGruppiVegetali = visibilitaGruppiVegetaliDt
            //     .AsEnumerable()
            //     .Select(r => r["Gru_Cod"]?.ToString())
            //     .Where(c => !string.IsNullOrEmpty(c))
            //     .OrderBy(c => c, StringComparer.Ordinal)
            //     .Select(c => c!)
            //     .ToList();

            // Con questa versione che effettua il parsing a int:
            visibilitaAttuale.CodiciGruppiVegetali = visibilitaGruppiVegetaliDt
                .AsEnumerable()
                .Select(r => r["Gru_Cod"]?.ToString())
                .Where(c => int.TryParse(c, out _))
                .Select(c => int.Parse(c!))
                .OrderBy(c => c)
                .ToList();

            // 2. Visibility: crop species.
            // Ref: DS07-BL – Visibilità Specie Vegetali.
            DataTable utentiImpostazioniMonoDt_SpecieVegetali =
                await _utentiImpostazioniFiltroMono.LeggiAsync(
                    Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI,
                    0,
                    objParametriUtenti,
                    objParametriServer
                );

            if (utentiImpostazioniMonoDt_SpecieVegetali.Rows.Count > 0)
            {
                DataTable visibilitaSpecieDt = (
                    await _specieVegetali.SpecieVegetali_GestioneFiltroUtente_LeggiAsync(
                        new LeggiSpecieVegetali_IN(),
                        utentiImpostazioniMonoDt_SpecieVegetali,
                        objParametriUtenti,
                        objParametriServer
                    )
                ).DataTable;

                visibilitaAttuale.CodiciSpecieVegetali = visibilitaSpecieDt
                    .AsEnumerable()
                    .Select(r => r["Veg_Cod"]?.ToString())
                    .Where(c => int.TryParse(c, out _))
                    .Select(c => int.Parse(c!))
                    .OrderBy(c => c)
                    .ToList();
            }

            // 3. Visibility: varieties (cultivar).
            // Ref: DS07-BL – Visibilità Varietà.
            DataTable utentiImpostazioniMonoDt_Varieta =
                await _utentiImpostazioniFiltroMono.LeggiAsync(
                    Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_VARIETA,
                    0,
                    objParametriUtenti,
                    objParametriServer
                );

            if (utentiImpostazioniMonoDt_Varieta.Rows.Count > 0)
            {
                DataTable visibilitaVarietaDt =
                    await _specieVegetali.Cultivar_GestioneFiltroUtente_LeggiAsync(
                        new Cultivar_GestioneFiltroUtente_Leggi_IN(),
                        utentiImpostazioniMonoDt_Varieta,
                        objParametriUtenti,
                        objParametriServer
                    );

                visibilitaAttuale.CodiciVarieta = visibilitaVarietaDt
                    .AsEnumerable()
                    .Select(r => r["Cul_Cod"]?.ToString())
                    .Where(c => int.TryParse(c, out _))
                    .Select(c => int.Parse(c!))
                    .OrderBy(c => c)
                    .ToList();
            }

            // 4. Visibility: operations / lavorazioni.
            // Ref: DS07-BL – Visibilità Operazioni/Lavorazioni.
            // Note: the two filters (groups and individual operations) are NOT cascading – checked separately.
            DataTable utentiImpostazioniDt_GruppoOperazioni =
                await _utentiImpostazioni.Read_JoinWithFiltroMonoAsync(
                    Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_OPERAZIONI,
                    0,
                    objParametriUtenti,
                    objParametriServer
                );

            DataTable utentiImpostazioniDt_Operazioni =
                await _utentiImpostazioni.Read_JoinWithFiltroMonoAsync(
                    Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                    1,
                    objParametriUtenti,
                    objParametriServer
                );

            if (utentiImpostazioniDt_Operazioni.Rows.Count > 0)
            {
                visibilitaAttuale.CodiciGruppoOperazioni = utentiImpostazioniDt_GruppoOperazioni
                    .AsEnumerable()
                    .Select(r => r.Field<int>("ID_0"))
                    .OrderBy(c => c)
                    .Distinct()
                    .ToList();

                DataTable visibilitaOperazioniDt = (
                    await _operazioni.Operazioni_GestioneFiltroUtente_LeggiAsync(
                        new LeggiOperazioni_IN
                        {
                            GruppoOperazioni = visibilitaAttuale.CodiciGruppoOperazioni,
                        },
                        utentiImpostazioniDt_GruppoOperazioni,
                        objParametriServer,
                        objParametriUtenti
                    )
                ).DataTable;

                visibilitaAttuale.CodiciOperazioni = visibilitaOperazioniDt
                    .AsEnumerable()
                    .Select(r => r.Field<int>("Lav_Cod"))
                    .OrderBy(c => c)
                    .Distinct()
                    .ToList();
            }
        }
        catch (SetupSystemUnavailableException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new SetupSystemUnavailableException(
                "Impossibile leggere filtri visibilità da sistema setup.",
                ex
            );
        }
        return (
            JsonConvert.SerializeObject(visibilitaAttuale, _normalizedJsonSettings),
            visibilitaAttuale
        );
    }

    /// <summary>
    /// Aggregates visibility filters from four sources (vegetable groups, crop species,
    /// varieties, operations) and returns them serialised to a normalised JSON string.
    /// Ref: DS07-BL – Recupero Filtri Visibilità Correnti; LeggiVisibilitaAziendeUtenteNormalizzata.
    /// </summary>
    private async Task<(string VisibilitaAziendeUtenteJson, ParamVisibilitaAziendeUtente VisibilitaAziendeUtente)> LeggiVisibilitaAziendeUtenteNormalizzataAsync(
        AgronicaCoreParametriUtenti objParametriUtenti,
        AgronicaCoreParametriServer objParametriServer
    )
    {
        var visibilitaAttuale = new ParamVisibilitaAziendeUtente();

        try
        {
            // 1. Visibility: vegetable groups.
            // Ref: DS07-BL – Visibilità GruppoVegetale.
            DataTable? visiblitaAziende = await _utentiVisibilitaAppoggio.ReadMinimumDataAsync(
                (int)Enum_TipoEntita.Impresa,
                objParametriServer,
                false
            );

            if (visiblitaAziende != null && visiblitaAziende.Rows.Count > 0)
                visibilitaAttuale.Aziende = visiblitaAziende
                    .AsEnumerable()
                    .Select(r => r["Piva"]?.ToString())
                    .Where(piva => !string.IsNullOrWhiteSpace(piva))
                    .OrderBy(c => c)
                    .Select(piva => piva!)
                    .ToList();

            // 2. Visibility: crop species.
            // Ref: DS07-BL – Visibilità Specie Vegetali.
            DataTable? visiblitaCentriAziendali = await _utentiVisibilitaAppoggio.ReadMinimumDataAsync(
                (int)Enum_TipoEntita.Centro,
                objParametriServer,
                true
            );

            if (visiblitaCentriAziendali != null && visiblitaCentriAziendali.Rows.Count > 0)
                visibilitaAttuale.CentriAziendali = visiblitaCentriAziendali
                    .AsEnumerable()
                    .Select(r => (Piva: r["Piva"]?.ToString(), SaCod: r.Field<int>("Sa_Cod")))
                    .Where(item => !string.IsNullOrWhiteSpace(item.Piva))
                    .OrderBy(c => c.Piva)
                    .ThenBy(c => c.SaCod)
                    .Select(item => (item.Piva!, item.SaCod))
                    .ToList();
        }
        catch (SetupSystemUnavailableException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new SetupSystemUnavailableException(
                "Impossibile leggere filtri visibilità da sistema setup.",
                ex
            );
        }
        return (
            JsonConvert.SerializeObject(visibilitaAttuale, _normalizedJsonSettings),
            visibilitaAttuale
        );
    }

    /// <summary>
    /// Validates the company-data synchronisation parameters against the values stored for the
    /// current user and determines whether a full dataset is required.
    /// Ref: DS01-BL – Descrizione; Regole di Business.
    /// </summary>
    public async Task<ValidazioneParametriUtenteDatiAziendaResult> ValidaParametriDatiAziendaAsync(
        string paramRichiamoApiRicevuto,
        AgronicaCoreParametriUtenti objParametriUtenti,
        AgronicaCoreParametriServer objParametriServer
    )
    {
        var sw = Stopwatch.StartNew();

        // Fail-fast: validate and normalise the received parameter before any async work.
        // Ref: DS07-BL – Eccezioni: InvalidParameterFormatException.
        string paramRichiamoNormalizzato = NormalizzaJson(paramRichiamoApiRicevuto);

        // Read current permissions and visibility from their respective sources.
        // These are always needed regardless of first-access or comparison path.
        // Ref: DS07-BL – Recupero Permessi Correnti; Recupero Filtri Visibilità Correnti.
        var permessiAttuali = await LeggiPermessiUtenteNormalizzatiAsync(
            objParametriUtenti,
            objParametriServer
        );
        WarnIfValidationTimeout(sw, objParametriUtenti.UtenteUsername, objParametriServer);

        var visibilitaAttuale = await LeggiVisibilitaUtenteNormalizzataAsync(
            objParametriUtenti,
            objParametriServer
        );
        WarnIfValidationTimeout(sw, objParametriUtenti.UtenteUsername, objParametriServer);

        var visibilitaAziendeAttuale = await LeggiVisibilitaAziendeUtenteNormalizzataAsync(
            objParametriUtenti,
            objParametriServer
        );
        WarnIfValidationTimeout(sw, objParametriUtenti.UtenteUsername, objParametriServer);

        // Read stored parameters for this user. An empty result indicates first access.
        // Ref: DS01-BL – Regole di Business: Query tabella app_param_utente_datiazienda_web2app.
        DataTable recordEsistente;
        try
        {
            recordEsistente = await _paramUtenteDatiAzienda.LeggiParametriUtenteDatiAziendaAsync(
                objParametriUtenti.UtenteUsername,
                objParametriServer
            );
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                $"Errore nella lettura da app_param_utente_datiazienda_web2app per utente '{objParametriUtenti.UtenteUsername}'.",
                ex
            );
        }
        WarnIfValidationTimeout(sw, objParametriUtenti.UtenteUsername, objParametriServer);

        // First access: no prior record found.
        // Ref: DS07-BL – Regole di Business: Se record non esiste (primo accesso).
        if (recordEsistente.Rows.Count == 0)
        {
            return new ValidazioneParametriUtenteDatiAziendaResult(
                richiediDatasetCompleto: true,
                motivo: MotivoValidazione.PrimoAccesso,
                paramPermessiEstratti: permessiAttuali.PermessiUtenti,
                paramPermessiEstrattiJson: permessiAttuali.PermessiUtentiJson,
                paramVisibilitaEstratti: visibilitaAttuale.VisibilitaUtente,
                paramVisibilitaEstrattiJson: visibilitaAttuale.VisibilitaUtenteJson,
                paramVisibilitaAziendeUtenteEstratti: visibilitaAziendeAttuale.VisibilitaAziendeUtente,
                paramVisibilitaAziendeUtenteEstrattiJson: visibilitaAziendeAttuale.VisibilitaAziendeUtenteJson,
                validationTimeMs: sw.ElapsedMilliseconds
            );
        }

        // Record exists: compare each stored value against the current value.
        // Ref: DS07-BL – Regole di Business: confronto stringa-per-stringa dopo normalizzazione.
        DataRow row = recordEsistente.Rows[0];
        string storedRichiamoApiNorm = NormalizzaJson(
            row["param_richiamo_api"]?.ToString() ?? string.Empty
        );
        string storedPermessiNorm = NormalizzaJson(
            row["param_permessi_utente"]?.ToString() ?? string.Empty
        );
        string storedVisibilitaNorm = NormalizzaJson(
            row["param_visibilita_utente"]?.ToString() ?? string.Empty
        );
        string storedVisibilitaAziendeNorm = NormalizzaJson(
            row["param_visibilita_azienda_utente"]?.ToString() ?? string.Empty
        );

        if (
            !string.Equals(
                paramRichiamoNormalizzato,
                storedRichiamoApiNorm,
                StringComparison.Ordinal
            )
        )
        {
            return new ValidazioneParametriUtenteDatiAziendaResult(
                richiediDatasetCompleto: true,
                motivo: MotivoValidazione.ParamRichiamoDiverso,
                paramPermessiEstratti: permessiAttuali.PermessiUtenti,
                paramPermessiEstrattiJson: permessiAttuali.PermessiUtentiJson,
                paramVisibilitaEstratti: visibilitaAttuale.VisibilitaUtente,
                paramVisibilitaEstrattiJson: visibilitaAttuale.VisibilitaUtenteJson,
                paramVisibilitaAziendeUtenteEstratti: visibilitaAziendeAttuale.VisibilitaAziendeUtente,
                paramVisibilitaAziendeUtenteEstrattiJson: visibilitaAziendeAttuale.VisibilitaAziendeUtenteJson,
                validationTimeMs: sw.ElapsedMilliseconds
            );
        }

        if (
            !string.Equals(
                permessiAttuali.PermessiUtentiJson,
                storedPermessiNorm,
                StringComparison.Ordinal
            )
        )
        {
            return new ValidazioneParametriUtenteDatiAziendaResult(
                richiediDatasetCompleto: true,
                motivo: MotivoValidazione.ParamPermessiDiverso,
                paramPermessiEstratti: permessiAttuali.PermessiUtenti,
                paramPermessiEstrattiJson: permessiAttuali.PermessiUtentiJson,
                paramVisibilitaEstratti: visibilitaAttuale.VisibilitaUtente,
                paramVisibilitaEstrattiJson: visibilitaAttuale.VisibilitaUtenteJson,
                paramVisibilitaAziendeUtenteEstratti: visibilitaAziendeAttuale.VisibilitaAziendeUtente,
                paramVisibilitaAziendeUtenteEstrattiJson: visibilitaAziendeAttuale.VisibilitaAziendeUtenteJson,
                validationTimeMs: sw.ElapsedMilliseconds
            );
        }

        if (
            !string.Equals(
                visibilitaAttuale.VisibilitaUtenteJson,
                storedVisibilitaNorm,
                StringComparison.Ordinal
            )
        )
        {
            return new ValidazioneParametriUtenteDatiAziendaResult(
                richiediDatasetCompleto: true,
                motivo: MotivoValidazione.FiltriVisibilitaDiversi,
                paramPermessiEstratti: permessiAttuali.PermessiUtenti,
                paramPermessiEstrattiJson: permessiAttuali.PermessiUtentiJson,
                paramVisibilitaEstratti: visibilitaAttuale.VisibilitaUtente,
                paramVisibilitaEstrattiJson: visibilitaAttuale.VisibilitaUtenteJson,
                paramVisibilitaAziendeUtenteEstratti: visibilitaAziendeAttuale.VisibilitaAziendeUtente,
                paramVisibilitaAziendeUtenteEstrattiJson: visibilitaAziendeAttuale.VisibilitaAziendeUtenteJson,
                validationTimeMs: sw.ElapsedMilliseconds
            );
        }

        if (
            !string.Equals(
                visibilitaAziendeAttuale.VisibilitaAziendeUtenteJson,
                storedVisibilitaAziendeNorm,
                StringComparison.Ordinal
            )
        )
        {
            return new ValidazioneParametriUtenteDatiAziendaResult(
                richiediDatasetCompleto: true,
                motivo: MotivoValidazione.FiltriVisibilitaDiversi,
                paramPermessiEstratti: permessiAttuali.PermessiUtenti,
                paramPermessiEstrattiJson: permessiAttuali.PermessiUtentiJson,
                paramVisibilitaEstratti: visibilitaAttuale.VisibilitaUtente,
                paramVisibilitaEstrattiJson: visibilitaAttuale.VisibilitaUtenteJson,
                paramVisibilitaAziendeUtenteEstratti: visibilitaAziendeAttuale.VisibilitaAziendeUtente,
                paramVisibilitaAziendeUtenteEstrattiJson: visibilitaAziendeAttuale.VisibilitaAziendeUtenteJson,
                validationTimeMs: sw.ElapsedMilliseconds
            );
        }

        return new ValidazioneParametriUtenteDatiAziendaResult(
            richiediDatasetCompleto: false,
            motivo: MotivoValidazione.ParametriCorrispondenti,
            paramPermessiEstratti: permessiAttuali.PermessiUtenti,
            paramPermessiEstrattiJson: permessiAttuali.PermessiUtentiJson,
            paramVisibilitaEstratti: visibilitaAttuale.VisibilitaUtente,
            paramVisibilitaEstrattiJson: visibilitaAttuale.VisibilitaUtenteJson,
            paramVisibilitaAziendeUtenteEstratti: visibilitaAziendeAttuale.VisibilitaAziendeUtente,
            paramVisibilitaAziendeUtenteEstrattiJson: visibilitaAziendeAttuale.VisibilitaAziendeUtenteJson,
            validationTimeMs: sw.ElapsedMilliseconds
        );
    }

    private void WarnIfValidationTimeout(Stopwatch stopwatch, string username, AgronicaCoreParametriServer objParametriServer)
    {
        if (stopwatch.ElapsedMilliseconds <= _settings.TimeoutMs)
            return;

        _loggingService.LogWarning(
            "Validazione parametri utente ha superato il timeout di {TimeoutMs} ms: {ElapsedMs} ms per utente '{Username}'.",
            objParametriServer,
            null,
            _settings.TimeoutMs,
            stopwatch.ElapsedMilliseconds,
            username
        );
    }

    /// <summary>
    /// Parses <paramref name="jsonInput"/> and re-serialises it with alphabetically sorted
    /// keys and minimal whitespace to produce a deterministic string suitable for equality
    /// comparison.
    /// Ref: DS07-BL – Regole di Business: normalizzazione JSON.
    /// </summary>
    /// <exception cref="InvalidParameterFormatException">
    /// Thrown when <paramref name="jsonInput"/> is not valid JSON.
    /// </exception>
    private static string NormalizzaJson(string jsonInput)
    {
        if (string.IsNullOrWhiteSpace(jsonInput))
            return "{}";

        try
        {
            object? parsed = JsonConvert.DeserializeObject(jsonInput);
            return JsonConvert.SerializeObject(parsed, _normalizedJsonSettings);
        }
        catch (JsonException ex)
        {
            throw new InvalidParameterFormatException(
                $"Il parametro ricevuto non è un JSON valido: {ex.Message}",
                ex
            );
        }
    }

    /// <summary>
    /// Orders all JSON object properties alphabetically (ordinal) to guarantee determinism
    /// across serialisations of the same object.
    /// Ref: DS07-BL – Regole di Business: "Ordinare alfabeticamente i codici prima di serializzazione JSON".
    /// </summary>
    private sealed class AlphabeticalContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(
            Type type,
            MemberSerialization memberSerialization
        )
        {
            var properties = base.CreateProperties(type, memberSerialization);
            return properties.OrderBy(p => p.PropertyName, StringComparer.Ordinal).ToList();
        }
    }
}
