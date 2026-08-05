using AgronicaDataProvider6.Interfaces;
using AgronicaNetCore.Base.Interfaces;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Services.Security;
using AgronicaNetCore.Base.Utility;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace AgronicaNetCore.Base.Base
{
    public class BaseService : Serilog_Base, IDBConnection, IDBTransaction
    {
        protected readonly IDataProvider6Factory? _dataProviderFactory;
        protected readonly ISecurityService? _securityService;

        /// <summary>
        /// Creates a new instance of BaseService
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="securityServiceBypass">Set to true for implementations where SecurityService is not needed</param>
        public BaseService(IServiceProvider provider, bool securityServiceBypass = false) : base (provider){
            _dataProviderFactory = provider.GetRequiredService<IDataProvider6Factory>();
            if (!securityServiceBypass) //don't remove securityServiceBypass, is needed in solutions where SecurityService is not available
            {
                _securityService = provider.GetRequiredService<ISecurityService>();
            }
        }

        /// <summary>
        /// Metodo BIZ per la chiusura di una connessione per il dataprovider.
        /// RollbackTransaction (opzionale, di deafult a false)  che esegue il rollback della transazione se presente 
        /// </summary>
        /// <param name="objParametri"></param>
        /// <param name="RollbackTransaction"></param>
        /// <exception cref="Exception"></exception>
        public void CloseConnection(AgronicaCoreParametri objParametri, bool RollbackTransaction = false)
        {
            if (_dataProviderFactory == null)
                throw new Exception("Riferimento a dataprovider factory null");
            _securityService?.SetConnectionString(objParametri);
            var dp = (IDataProvider)_dataProviderFactory.GetDataProvider(UtilityAgronica.getSqlConnectionFromObjParametri(objParametri), objParametri.Lingua_Cod,objParametri.objConnessione,objParametri.objTransazione);
            var tmpObjConn = objParametri.objConnessione;
            dp.CloseConnection(ref tmpObjConn,RollbackTransaction);
            objParametri.objConnessione = tmpObjConn;
            objParametri.objTransazione = null; //Lavez/Netso pulizia transazioni da objParametri (può dare problemi per due transazioni consecutive)
        }

        /// <summary>
        /// Metodo BIZ per la chiusura di una transazione per il dataprovider.
        /// Rollback (opzionale, di deafult a false) che esegue il rollback della stessa
        /// </summary>
        /// <param name="objParametri"></param>
        /// <param name="Rollback"></param>
        /// <exception cref="Exception"></exception>
        public void CloseTransaction(AgronicaCoreParametri objParametri, bool Rollback = false)
        {
            if (_dataProviderFactory == null)
                throw new Exception("Riferimento a dataprovider factory null");
            _securityService?.SetConnectionString(objParametri);
            var dp = (IDataProvider)_dataProviderFactory.GetDataProvider(UtilityAgronica.getSqlConnectionFromObjParametri(objParametri), objParametri.Lingua_Cod, objParametri.objConnessione, objParametri.objTransazione);
            var tmpObjTransact = objParametri.objTransazione;
            dp.CloseTransaction(ref tmpObjTransact!, Rollback);
            objParametri.objTransazione = tmpObjTransact;
        }

        /// <summary>
        /// Metodo BIZ per l'apertura di una connessione per il dataprovider.
        /// OpenTransaction (opzionale, di deafult a true) che esegue la creazione della transazione collegata alla connessione stessa
        /// IsolationLevel (opzionale, di default a null) che permette di specificare il Tipo di IsolationLevel da applicare alla Transazione (ReadCommitted/ReadUnCommitted/Chaos/Snapshot/etc...)
        /// </summary>
        /// <param name="objParametri"></param>
        /// <param name="OpenTransaction"></param>
        /// <param name="level"></param>
        /// <exception cref="Exception"></exception>
        public async Task OpenConnectionAsync(AgronicaCoreParametri objParametri, bool OpenTransaction = true, IsolationLevel? level = null)
        {
            if (_dataProviderFactory == null)
                throw new Exception("Riferimento a dataprovider factory null");
            _securityService?.SetConnectionString(objParametri);
            var dp = (IDataProvider)_dataProviderFactory.GetDataProvider(UtilityAgronica.getSqlConnectionFromObjParametri(objParametri), objParametri.Lingua_Cod, objParametri.objConnessione, objParametri.objTransazione);
            var ret = await dp.OpenConnectionAsync(OpenTransaction,level);
            objParametri.objConnessione = ret.Item1;
            objParametri.objTransazione = ret.Item2;
        }

        /// <summary>
        /// Metodo BIZ per l'apertura di una transazione per il dataprovider.
        /// Deve essere già attiva una connessione 
        /// IsolationLevel (opzionale, di default a null) che permette di specificare il Tipo di IsolationLevel da applicare alla Transazione (ReadCommitted/ReadUnCommitted/Chaos/Snapshot/etc...)
        /// </summary>
        /// <param name="objParametri"></param>
        /// <param name="level"></param>
        /// <exception cref="Exception"></exception>
        public async Task OpenTransactionAsync(AgronicaCoreParametri objParametri, IsolationLevel? level = null)
        {
            if (_dataProviderFactory == null)
                throw new Exception("Riferimento a dataprovider factory null");
            _securityService?.SetConnectionString(objParametri);
            var dp = (IDataProvider)_dataProviderFactory.GetDataProvider(UtilityAgronica.getSqlConnectionFromObjParametri(objParametri), objParametri.Lingua_Cod, objParametri.objConnessione, objParametri.objTransazione);
            objParametri.objTransazione = await dp.OpenTransactionAsync(objParametri.objConnessione!,level);
        }
    }
}
