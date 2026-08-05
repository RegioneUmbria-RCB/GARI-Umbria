using OutData.Zoo;
using AgronicaNetCore.Base.Models;
using AgronicaCoreModelsSTD.attivita;
using InData.Zoo;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.Trattamenti
{
    public interface ITrattamentoZooService
    {
        public interface ITrattamentoResult { }

        Task<SomministrazioneProdotti> LeggiSomministrazioneProdottiAsync(string sommNumero, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Dato il Protocollo da cui partire, ribalta il Protocollo in una x nuove Prescrizioni (Indicazioni Terapeutiche), in base al campo Numero_Somm delle Righe del Protocollo, 
        /// contenenti i dettagli mancanti per il Trattamento, con le rispettive Righe di Prescrizione.
        /// </summary>
        /// <param name="Id_Protocollo">Protocollo da cui si sta creando il trattamento (corrisponde a Ricette_Zoo.IdRicetta)</param>
        /// <param name="somministrazioni">Prime somministrazioni da effettuare (contiene anche i dati delle somministrazioni seguenti in base all'intervallo impostato su Ricette_Zoo_Agenda)</param>
        /// <param name="objP_Server"></param>
        /// <param name="objP_Utenti"></param>
        /// <returns></returns>
        Task<ITrattamentoResult> CreaTrattamentoDaProtocollo(int Id_Protocollo, List<Attivita> somministrazioni, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti);

        /// <summary>
        /// Crea o modifica una Somministrazione dall'oggetto Attivita passato
        /// </summary>
        /// <param name="somministrazione"></param>
        /// <param name="objParametriServer"></param>
        /// <param name="objParametriUtenti"></param>
        /// <returns></returns>
        Task<ITrattamentoResult> ScriviModificaSomministrazione(Attivita somministrazione, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Elimina una Somministrazione (futura o confermata)
        /// </summary>
        /// <param name="dtoDelete"></param>
        /// <param name="objParametriServer"></param>
        /// <returns></returns>
        Task<bool> EliminaSomministrazione(DeleteSomministrazione dtoDelete, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Piva"></param>
        /// <param name="Id_Agenda"></param>
        /// <param name="objParametriServer"></param>
        /// <returns></returns>
        Task<Attivita?> GetAttivitaFromAgenda(string Piva, int Id_Agenda, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Data la Prescrizione Veterinaria o l'Indicazione Terapeutica da cui partire, 
        /// crea le Somministrazioni indicate dal numero di righe della Prescrizione
        /// </summary>
        /// <param name="Id_Prescrizione"></param>
        /// <param name="somministrazioni"></param>
        /// <param name="objP_Server"></param>
        /// <param name="objP_Utenti"></param>
        /// <returns></returns>
        Task<ITrattamentoResult> CreaTrattamentoDaPrescrizione(int Id_Prescrizione, List<Attivita> somministrazioni, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti);

        /// <summary>
        /// Permette di creare n protocolli in corso, compreso il primo, a partire da un Protocollo esistente
        /// </summary>
        /// <param name="Id_Protocollo"></param>
        /// <param name="somministrazioni"></param>
        /// <param name="objP_Server"></param>
        /// <param name="objP_Utenti"></param>
        /// <returns></returns>
        Task<ITrattamentoResult> ProgrammaTrattamentoDaProtocollo(int Id_Protocollo, List<Attivita> somministrazioni, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti);
    }
}
