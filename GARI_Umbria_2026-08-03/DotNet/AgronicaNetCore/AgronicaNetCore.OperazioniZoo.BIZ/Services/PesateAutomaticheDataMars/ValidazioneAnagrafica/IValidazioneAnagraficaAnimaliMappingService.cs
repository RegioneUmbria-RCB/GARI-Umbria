using AgronicaNetCore.Base.Models;
using OutData.Zoo.DataMars;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.ValidazioneAnagrafica;

/// <summary>
/// Valida la coerenza anagrafica di un animale (LID) e verifica il mapping FarmID → stalla GIAS.
/// <para>Riferimento spec: DS05-BL ValidazioneAnagraficaAnimaliMappingStalla.</para>
/// </summary>
public interface IValidazioneAnagraficaAnimaliMappingService
{
    /// <summary>
    /// Esegue il solo mapping FarmID → stalla GIAS e restituisce la PIVA dell'azienda.
    /// Restituisce null se il FarmID non è mappato a nessuna stalla.
    /// </summary>
    Task<string?> GetPivaDaFarmIdAsync(string farmId, AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Esegue la validazione per un singolo animale:
    /// lookup ZOO_ANIMALI, controllo finestra temporale, mapping FarmID → stalla.
    /// </summary>
    Task<ValidazioneAnagraficaResult> ValidaAsync(
        string lid,
        string farmId,
        DateTime dataPesata,
        AgronicaCoreParametriServer objParametriServer);
}
