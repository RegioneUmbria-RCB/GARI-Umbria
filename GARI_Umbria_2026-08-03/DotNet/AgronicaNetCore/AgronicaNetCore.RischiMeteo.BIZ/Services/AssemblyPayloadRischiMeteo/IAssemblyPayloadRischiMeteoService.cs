using AgronicaNetCore.Base.Models;
using AgronicaNetCore.RischiMeteo.BIZ.Models;

namespace AgronicaNetCore.RischiMeteo.BIZ.Services.AssemblyPayloadRischiMeteo
{
    /// <summary>
    /// Contratto per la business logic di costruzione automatica dei payload JSON
    /// per l'Engine Rischi Meteoclimatici (M2).
    /// Per ogni Esercizio selezionato viene assemblato un payload conforme alla specifica API M2,
    /// integrando dati da tabelle GIAS (Agenda, Impianti, GIS) secondo il mapping definito.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Descrizione.
    /// </summary>
    public interface IAssemblyPayloadRischiMeteoService
    {
        /// <summary>
        /// Costruisce i payload JSON M2 per tutti gli Esercizi in <paramref name="input"/>.
        /// Gli Esercizi che causano errori bloccanti vengono saltati senza interrompere gli altri.
        /// </summary>
        /// <param name="input">Filiera e lista di Esercizi selezionati.</param>
        /// <param name="objParametriServer">Parametri di connessione al server GIAS.</param>
        /// <returns>
        /// <see cref="CostruzionePayloadRischiMeteoOutput"/> con i payload costruiti e il dettaglio
        /// degli eventuali errori per Esercizio.
        /// </returns>
        Task<CostruzionePayloadRischiMeteoOutput> AssembleAsync(
            CostruzionePayloadRischiMeteoInput input,
            AgronicaCoreParametriServer objParametriServer);
    }
}
