using AgronicaNetCore.Base.Models;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.Models;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.StallaMappingDatamars;

/// <summary>
/// Accesso ai dati per il mapping FarmID Datamars → stalla GIAS.
/// <para>Riferimento spec: DS05-BL ValidazioneAnagraficaAnimaliMappingStalla — Persistenze Coinvolte,
/// Configurazione Mapping FarmID-Stalla (SELECT).</para>
/// </summary>
public interface IStallaMappingDatamarsRepository
{
    /// <summary>
    /// Recupera le informazioni di stalla GIAS (SaCod, StaNum) associate al FarmID Datamars.
    /// Ritorna null se il FarmID non è mappato a nessuna stalla.
    /// </summary>
    Task<StallaInfo?> GetStallaInfoByFarmIdAsync(string farmId, AgronicaCoreParametriServer objParametriServer);
}
