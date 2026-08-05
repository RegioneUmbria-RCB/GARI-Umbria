using AgronicaNetCore.Base.Models;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.Models;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.ZooAnimaliDatamars;

/// <summary>
/// Accesso ai dati per la tabella ZOO_ANIMALI — lookup anagrafica per Matricola (LID).
/// <para>Riferimento spec: DS05-BL ValidazioneAnagraficaAnimaliMappingStalla — Persistenze Coinvolte,
/// ZOO_ANIMALI (SELECT); DS02-BL Step 1 Lookup Anagrafica.</para>
/// </summary>
public interface IZooAnimaliDatamarsRepository
{
    /// <summary>
    /// Recupera le informazioni anagrafiche dell'animale da ZOO_ANIMALI tramite Matricola (LID Datamars).
    /// Ritorna null se l'animale non esiste in anagrafica.
    /// </summary>
    Task<AnimaleInfo?> GetAnimaleByMatricolaAsync(string matricola, string piva, AgronicaCoreParametriServer objParametriServer);
}
