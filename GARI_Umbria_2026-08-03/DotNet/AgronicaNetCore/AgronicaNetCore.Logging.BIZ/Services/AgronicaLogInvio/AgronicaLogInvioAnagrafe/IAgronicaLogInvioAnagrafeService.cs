using AgronicaNetCore.Base.Models;
using InData.Log.AgronicaLogInvio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Logging.BIZ.Services.AgronicaLogInvio.AgronicaLogInvioAnagrafe
{
    /// <summary>
    /// Interfaccia per il servizio di business (BIZ) che orchestra la scrittura
    /// dei log di invio per le anagrafiche.
    /// </summary>
    public interface IAgronicaLogInvioAnagrafeService
    {
        /// <summary>
        /// Esegue una scrittura transazionale su entrambe le tabelle di log (_Chiamate e _Anagrafe).
        /// Replica la logica della funzione VB 'Scrivi_Log_Invio_Anagrafe'.
        /// </summary>
        /// <param name="writeChiamateModel">I dati per il log della chiamata tecnica.</param>
        /// <param name="writeAnagrafeModel">I dati per il log di collegamento all'anagrafica.</param>
        /// <param name="objParametriServer">I parametri di contesto del server.</param>
        /// <returns>True se l'intera operazione ha successo.</returns>
        Task<bool> WriteLogCompletoAsync(
            WriteAgronicaLogInvioChiamate writeChiamateModel,
            WriteAgronicaLogInvioAnagrafe writeAnagrafeModel,
            AgronicaCoreParametriServer objParametriServer);
    }
}
