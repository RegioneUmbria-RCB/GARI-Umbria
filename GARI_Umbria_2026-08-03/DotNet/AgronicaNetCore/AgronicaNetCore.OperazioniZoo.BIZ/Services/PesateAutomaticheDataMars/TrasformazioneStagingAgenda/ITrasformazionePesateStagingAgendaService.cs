using AgronicaNetCore.Base.Models;
using InData.Zoo.DataMars;
using OutData.Zoo.DataMars;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.TrasformazioneStagingAgenda;

/// <summary>
/// Orchestratore del ciclo di trasformazione pesate staging → operazioni agenda zoo.
/// <para>Riferimento spec: DS02-BL TrasformazionePesateStagingAgenda.</para>
/// </summary>
public interface ITrasformazionePesateStagingAgendaService
{
    /// <summary>
    /// Legge un batch di record da STAGING_PESATE (processato=0), deserializza il payload JSON
    /// e, per ogni pesata valida, crea l'operazione di pesatura sull'agenda zoo.
    /// </summary>
    Task<RisultatoTrasformazionePesate> TrasformaAsync(
        ParametriTrasformazionePesate parametri,
        AgronicaCoreParametriServer objParametriServer,
        CancellationToken cancellationToken = default);
}
