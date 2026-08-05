using AgronicaNetCore.Base.Models;
using AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.Exceptions;
using OutData.Zoo.DataMars;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.StallaMappingDatamars;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.ZooAnimaliDatamars;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.ValidazioneAnagrafica;

/// <summary>
/// Implementazione della validazione anagrafica animali e mapping stalla.
/// <para>Riferimento spec: DS05-BL ValidazioneAnagraficaAnimaliMappingStalla.</para>
/// </summary>
public sealed class ValidazioneAnagraficaAnimaliMappingService : BaseServiceOperazioniZooBIZ, IValidazioneAnagraficaAnimaliMappingService
{
    // Tolleranza per il confronto temporale (±1 minuto per timezone).
    private static readonly TimeSpan TolleranzaTemporale = TimeSpan.FromMinutes(1);

    private readonly IZooAnimaliDatamarsRepository _zooAnimaliRepository;
    private readonly IStallaMappingDatamarsRepository _stallaMappingRepository;

    public ValidazioneAnagraficaAnimaliMappingService(
        IServiceProvider provider,
        IStringLocalizer<Resources.Messages> localizer)
        : base(provider, localizer)
    {
        _zooAnimaliRepository = provider.GetRequiredService<IZooAnimaliDatamarsRepository>();
        _stallaMappingRepository = provider.GetRequiredService<IStallaMappingDatamarsRepository>();
    }

    /// <inheritdoc/>
    public async Task<string?> GetPivaDaFarmIdAsync(string farmId, AgronicaCoreParametriServer objParametriServer)
    {
        var stallaInfo = await _stallaMappingRepository.GetStallaInfoByFarmIdAsync(farmId, objParametriServer);
        return stallaInfo?.Piva;
    }

    /// <inheritdoc/>
    public async Task<ValidazioneAnagraficaResult> ValidaAsync(
        string lid,
        string farmId,
        DateTime dataPesata,
        AgronicaCoreParametriServer objParametriServer)
    {
        // ── Step 1: Lookup stalla ──────────────────────────────────────────
        var stallaInfo = await _stallaMappingRepository.GetStallaInfoByFarmIdAsync(farmId, objParametriServer);
        if (stallaInfo is null)
        {
            return new ValidazioneAnagraficaResult
            {
                ValidazioneOK = false,
                StatusValidazione = "STALLA_MAPPING_FAILED",
                Messaggio = $"FarmID='{farmId}' non è mappato a nessuna stalla GIAS.",
                ErrorType = nameof(StallaMappingException),
                ErrorMessage = new StallaMappingException(farmId).Message
            };
        }

        // ── Step 2: Lookup animale ─────────────────────────────────────────
        var animaleInfo = await _zooAnimaliRepository.GetAnimaleByMatricolaAsync(lid, stallaInfo.Piva, objParametriServer);
        if (animaleInfo is null)
        {
            return new ValidazioneAnagraficaResult
            {
                ValidazioneOK = false,
                StatusValidazione = "ANIMAL_NOT_FOUND",
                Messaggio = $"Animale con LID='{lid}' non trovato in ZOO_ANIMALI.",
                ErrorType = nameof(AnimalNotFoundException),
                ErrorMessage = new AnimalNotFoundException(lid).Message
            };
        }

        // ── Step 3: Validazione Cod_Progetto ───────────────────────────────
        if (animaleInfo.CodProgetto == 0)
        {
            return new ValidazioneAnagraficaResult
            {
                ValidazioneOK = false,
                StatusValidazione = "INVALID_COD_PROGETTO",
                Messaggio = $"Animale LID='{lid}' ha Cod_Progetto=0 (anagrafica incompleta).",
                ErrorType = "InvalidCodProgettoException",
                ErrorMessage = $"Cod_Progetto non valorizzato per LID='{lid}'."
            };
        }

        // ── Step 4: Validazione finestra temporale ─────────────────────────
        var from = animaleInfo.ValiditaInizio;
        var to   = animaleInfo.ValiditaFine;

        if (dataPesata < from.Subtract(TolleranzaTemporale) || dataPesata > to.Add(TolleranzaTemporale))
        {
            return new ValidazioneAnagraficaResult
            {
                ValidazioneOK = false,
                StatusValidazione = "ANIMAL_INACTIVE",
                Messaggio = $"Animale LID='{lid}' inattivo alla data di pesata {dataPesata:O}. ValiditaFine={to:O}.",
                ErrorType = nameof(AnimalInactiveException),
                ErrorMessage = new AnimalInactiveException(lid, dataPesata, to).Message
            };
        }

        return new ValidazioneAnagraficaResult
        {
            ValidazioneOK = true,
            CodProgetto = animaleInfo.CodProgetto,
            Piva = stallaInfo.Piva,
            SaCod = stallaInfo.SaCod,
            StaNum = stallaInfo.StaNum,
            StatusValidazione = "OK",
            Messaggio = "Animale valido e stalla mappata.",
            ValiditaInizio = animaleInfo.ValiditaInizio,
            ValiditaFine = animaleInfo.ValiditaFine
        };
    }
}
