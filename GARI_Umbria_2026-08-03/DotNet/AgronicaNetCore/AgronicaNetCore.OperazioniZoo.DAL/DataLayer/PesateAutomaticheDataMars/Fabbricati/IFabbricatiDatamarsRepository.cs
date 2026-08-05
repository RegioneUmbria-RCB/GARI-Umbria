using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.Fabbricati;

/// <summary>
/// Accesso ai dati per la lettura dei FarmID Datamars associati all'azienda.
/// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI — Step 1 Estrazione FarmID.</para>
/// </summary>
public interface IFabbricatiDatamarsRepository
{
    /// <summary>
    /// Restituisce la lista dei FarmID Datamars per l'azienda identificata in <paramref name="objParametriServer"/>.
    /// </summary>
    Task<List<string>> GetFarmIdsAsync(AgronicaCoreParametriServer objParametriServer);
}
