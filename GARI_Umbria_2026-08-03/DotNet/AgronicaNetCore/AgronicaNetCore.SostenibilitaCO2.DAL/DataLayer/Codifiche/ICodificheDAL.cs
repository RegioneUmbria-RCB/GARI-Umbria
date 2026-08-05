using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Codifiche
{
    /// <summary>
    /// Contratto per il recupero dei dati operazione richiesti dal payload M4 ma non
    /// disponibili direttamente nelle query <c>LeggiImpiantiAsync</c> / <c>LeggiProdottiAsync</c>:
    /// tipo operazione (da <c>Codifica_Operazioni_SistemiEsterni</c>), data operazione e resa prevista.
    /// Riferimento spec: DS04-BL AssemblyPayloadM4AppezzamentoImpianto — DS04.2-BL Mappature.
    /// </summary>
    public interface ICodificheDAL
    {
        /// <summary>
        /// Recupera per ciascun esercizio le operazioni con mapping in <c>Codifica_Operazioni_SistemiEsterni</c>
        /// per il sistema CO2 engine, includendo data operazione e resa prevista dell'esercizio.
        /// Il JOIN INNER su <c>Codifica_Operazioni_SistemiEsterni</c> filtra automaticamente le operazioni
        /// prive di mapping (come da DS04.2-BL "join secco").
        /// </summary>
        /// <param name="esercizi">Lista dei codici esercizio (<c>Progetto_Cod</c>) da interrogare.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// Lista di <see cref="OperazioneAgendaCO2Entity"/> con tipo operazione, data e resa prevista,
        /// indicizzati per <c>IdAgenda</c>.
        /// </returns>
        Task<IReadOnlyDictionary<int, string>> GetTipoOperazioneByLavCodAsync(
            IReadOnlyList<int> lavCods,
            AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Recupera il dizionario di decodifica <c>Udm_Cod → Udm_Cod_Esterno</c>
        /// dalla tabella <c>Codifica_UnitaMisura_SistemiEsterni</c> per il sistema CO2 engine.
        /// </summary>
        Task<IReadOnlyDictionary<int, string>> GetUdmCodEsternoByUdmCodAsync(
            IReadOnlyList<int> udmCods,
            AgronicaCoreParametriServer objParametriServer);
    }
}
