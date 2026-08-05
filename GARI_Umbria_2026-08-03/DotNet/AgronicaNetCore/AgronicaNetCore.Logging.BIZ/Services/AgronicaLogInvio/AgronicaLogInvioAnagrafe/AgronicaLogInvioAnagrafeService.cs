using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Logging.BIZ.Services.AgronicaLogInvio.AgronicaLogInvioChiamate;
using AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogInvio.AgronicaLogInvioAnagrafe;
using InData.Log.AgronicaLogInvio;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.Logging.BIZ.Services.AgronicaLogInvio.AgronicaLogInvioAnagrafe
{
    /// <summary>
    /// Implementazione del servizio di business (BIZ) per il logging degli invii di anagrafiche.
    /// </summary>
    public class AgronicaLogInvioAnagrafeService : BaseService, IAgronicaLogInvioAnagrafeService
    {
        // Dipendenza verso il DAL della tabella di collegamento _Anagrafe
        private readonly IAgronicaLogInvioAnagrafe _logAnagrafeDAL;
        // Dipendenza verso il BIZ della tabella _Chiamate (per riutilizzarne la logica)
        private readonly IAgronicaLogInvioChiamateService _logChiamateBIZ;

        public AgronicaLogInvioAnagrafeService(IServiceProvider provider) : base(provider)
        {
            _logAnagrafeDAL = provider.GetRequiredService<IAgronicaLogInvioAnagrafe>();
            _logChiamateBIZ = provider.GetRequiredService<IAgronicaLogInvioChiamateService>();
        }

        /// <summary>
        /// Esegue una scrittura transazionale su entrambe le tabelle di log (_Chiamate e _Anagrafe).
        /// Replica la logica della funzione VB 'Scrivi_Log_Invio_Anagrafe'.
        /// </summary>
        public async Task<bool> WriteLogCompletoAsync(
            WriteAgronicaLogInvioChiamate writeChiamateModel,
            WriteAgronicaLogInvioAnagrafe writeAnagrafeModel,
            AgronicaCoreParametriServer objParametriServer)
        {
            bool result = false;
            try
            {
                // Avvia la connessione e la transazione usando i metodi ereditati da BaseService
                await OpenConnectionAsync(objParametriServer);

                // 1. Scrive sulla tabella _Chiamate.
                //    Il metodo 'WriteAsync' del DAL sottostante modificherà l'oggetto 'writeChiamateModel'
                //    popolando la sua proprietà 'ID' con il nuovo ID generato.
                result = await _logChiamateBIZ.WriteAsync(writeChiamateModel, objParametriServer);

                // 2. Se la prima scrittura ha successo e ha restituito un ID valido...
                if (result && writeChiamateModel.ID > 0)
                {
                    // ...usa l'ID (ora presente in 'writeChiamateModel.ID') per collegare il secondo record...
                    writeAnagrafeModel.ID_Log_Invio = writeChiamateModel.ID;

                    // ...e scrive sulla tabella _Anagrafe.
                    result = await _logAnagrafeDAL.WriteAsync(writeAnagrafeModel, objParametriServer);
                }
                else
                {
                    // Se la prima scrittura fallisce o non restituisce un ID, l'intera operazione fallisce.
                    throw new InvalidOperationException("La scrittura del log della chiamata (Agronica_Log_Invio_Chiamate) è fallita o non ha generato un ID.");
                }
            }
            catch (Exception ex)
            {
                // In caso di errore in qualsiasi punto, esegue il rollback della transazione
                CloseTransaction(objParametriServer, true);
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            finally
            {
                // Chiude la connessione (e fa il commit della transazione se non ci sono stati errori)
                CloseConnection(objParametriServer);
            }
            return result;
        }
    }
}
