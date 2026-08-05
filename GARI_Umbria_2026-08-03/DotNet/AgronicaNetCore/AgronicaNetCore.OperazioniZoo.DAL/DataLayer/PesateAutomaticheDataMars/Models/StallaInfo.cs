namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.Models;

/// <summary>
/// Risultato del lookup FarmID → stalla GIAS.
/// <para>Riferimento spec: DS05-BL ValidazioneAnagraficaAnimaliMappingStalla — Persistenze Coinvolte,
/// Configurazione Mapping FarmID-Stalla.</para>
/// </summary>
public sealed class StallaInfo
{
    /// <summary>Partita IVA dell'azienda.</summary>
    public string Piva { get; init; } = string.Empty;

    /// <summary>Codice centro aziendale.</summary>
    public int SaCod { get; init; }

    /// <summary>Codice stalla (Sta_Num), usato come chiave per Agenda.Sta_Num.</summary>
    public int StaNum { get; init; }
}
