using AgronicaNetCore.Base.Models;
using AgronicaCoreDTOStd.InData.Gis;
using System.Data;
using System.Threading.Tasks;
using InData.Gis;

namespace AgronicaNetCore.Gis.DAL.DataLayer.Gis
{
    /// <summary>
    /// Definisce le operazioni di accesso ai dati (CRUD) 
    /// relative alla configurazione del clustering GIS.
    /// </summary>
    public interface IGisClusterConfig
    {
        #region Read Operations
        /// <summary>
        /// Legge una configurazione completa (testata + dettagli) dal database.
        /// </summary>
        /// <param name="configTypeCod">ID del tipo di algoritmo.</param>
        /// <param name="layerCod">ID del layer.</param>
        /// <param name="utente">Username dell'utente.</param>
        /// <param name="objParametriServer">Oggetto di contesto con filtri (visibilità, date).</param>
        /// <returns>Un DataTable contenente i dati della configurazione.</returns>
        Task<DataTable> GetConfigAsDataTableAsync(int? configTypeCod, int? layerCod, string utente, AgronicaCoreParametriServer objParametriServer);


        /// <summary>
        /// Restituisce il centroide WKT dell'appezzamento leggendolo da
        /// <c>GIS_ElementiGrafici_Clustering</c> tramite join su <c>GIS_ElementiGrafici</c>
        /// e <c>GIS_Entita</c>.
        /// </summary>
        /// <param name="piva">Partita IVA dell'azienda.</param>
        /// <param name="saCod">Codice azienda satellite.</param>
        /// <param name="appezza">Codice appezzamento.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// Stringa WKT del centroide (es. <c>"POINT(12.3456 41.2345)"</c>)
        /// oppure <c>null</c> se nessun record è presente.
        /// </returns>
        Task<string?> GetCentroideWktAsync(string piva, int saCod, int appezza, AgronicaCoreParametriServer objParametriServer);

        Task<string?> GetPoligonoWktAsync(string piva, int saCod, int appezza, AgronicaCoreParametriServer objParametriServer);

        Task<string?> GetFirstPointCentroideWktAsync(string poligonoWkt);

        #endregion

        #region Write Operations
        /// <summary>
        /// Inserisce la riga di "testata" nella tabella _Config utilizzando un ID pre-generato.
        /// </summary>
        /// <param name="newConfigCod">L'ID univoco per la nuova riga.</param>
        /// <param name="config">Il DTO con i dati della testata.</param>
        /// <param name="username">L'utente che sta eseguendo l'operazione.</param>
        /// <param name="objParametriServer">Oggetto di contesto.</param>
        /// <returns>L'ID della riga inserita.</returns>
        Task<int> InsertConfigHeaderAsync(int newConfigCod, GisClusterConfigSave_InData config, string username, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Inserisce una singola riga di "dettaglio" nella tabella _Config_Detail.
        /// </summary>
        /// <param name="configCod">L'ID della testata a cui questo dettaglio è collegato.</param>
        /// <param name="detail">Il DTO con i dati del dettaglio.</param>
        /// <param name="username">L'utente che sta eseguendo l'operazione.</param>
        /// <param name="objParametriServer">Oggetto di contesto.</param>
        Task <bool> InsertConfigDetailAsync(int configCod, GisClusterConfigDetail_InData detail, string username, AgronicaCoreParametriServer objParametriServer);
        #endregion

        #region Update Operations
        /// <summary>
        /// Aggiorna la riga di "testata" nella tabella _Config.
        /// </summary>
        /// <param name="config">Il DTO con i nuovi dati.</param>
        /// <param name="username">L'utente che sta eseguendo l'operazione.</param>
        /// <param name="objParametriServer">Oggetto di contesto.</param>
        /// <returns>True se l'aggiornamento ha avuto successo.</returns>
        Task<bool> UpdateConfigHeaderAsync(GisClusterConfigSave_InData config, string username, AgronicaCoreParametriServer objParametriServer);
        #endregion

        #region Delete Operations
        /// <summary>
        /// Cancella tutte le righe di dettaglio associate a un ID di configurazione.
        /// </summary>
        /// <param name="configCod">L'ID della configurazione di cui cancellare i dettagli.</param>
        /// <param name="objParametriServer">Oggetto di contesto.</param>
        Task DeleteConfigDetailsAsync(int configCod, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Cancella una configurazione di testata basandosi sulla sua chiave logica.
        /// </summary>
        /// <param name="configTypeCod">ID del tipo di algoritmo.</param>
        /// <param name="layerCod">ID del layer.</param>
        /// <param name="utente">Username dell'utente.</param>
        /// <param name="objParametriServer">Oggetto di contesto.</param>
        /// <returns>True se la cancellazione ha avuto successo.</returns>
        Task<bool> DeleteConfigHeaderAsync(int configTypeCod, int layerCod, string utente, AgronicaCoreParametriServer objParametriServer);
        #endregion
    }
}