namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.Models;

/// <summary>
/// Risultato del lookup anagrafica animale da ZOO_ANIMALI tramite Matricola (LID).
/// <para>Riferimento spec: DS05-BL ValidazioneAnagraficaAnimaliMappingStalla — Persistenze Coinvolte,
/// ZOO_ANIMALI (SELECT).</para>
/// </summary>
public sealed class AnimaleInfo
{
    /// <summary>Chiave tabella ZOO_ANIMALI, usata come Cod_Progetto in Movimenti_Dettagli.</summary>
    public int CodProgetto { get; init; }

    /// <summary>Data inizio validità dell'anagrafica animale (UTC).</summary>
    public DateTime ValiditaInizio { get; init; }

    /// <summary>Data fine validità dell'anagrafica animale (UTC).</summary>
    public DateTime ValiditaFine { get; init; }
}
