using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.AssemblyPayloadCo2
{
    /// <summary>
    /// Contratto per la business logic che costruisce i livelli top-level del payload M4
    /// (root e aziende[]) per l'invocazione dell'Engine di Sostenibilità CO2.
    /// Riferimento spec: DS03-BL AssemblyPayloadM4FilieraAzienda.
    /// </summary>
    public interface IAssemblyPayloadCo2FilieraAziendaService
    {
        /// <summary>
        /// Assembla la radice del payload M4 (<c>codice_raggruppamento</c>, <c>tipo_raggruppamento</c>)
        /// e la lista <c>aziende[]</c> con i relativi consumi e centroide.
        /// Il campo <c>appezzamenti</c> viene inizializzato come array vuoto (placeholder per DS04-BL).
        /// </summary>
        /// <param name="input">
        /// Dati aggregati: perimetro validato (DS01-BL), consumi validati (DS02-BL),
        /// modalità, filiera, anno e colture.
        /// </param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// Radice del payload M4 con <c>aziende[]</c> popolate.
        /// </returns>
        /// <exception cref="Exceptions.SostenibilitaCO2.DataNotFoundException">
        /// Se un'azienda del perimetro non è trovata in tabella <c>Aziende</c>.
        /// </exception>
        /// <exception cref="Exceptions.SostenibilitaCO2.PayloadStructureException">
        /// Se il payload risultante non è strutturalmente valido.
        /// </exception>
        Task<PayloadCo2Root> AssembleAsync(AssemblyPayloadCo2Input input, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
    }
}
