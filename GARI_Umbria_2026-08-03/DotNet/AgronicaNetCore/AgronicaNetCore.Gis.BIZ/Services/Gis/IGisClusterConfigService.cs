using AgronicaNetCore.Base.Models;
using AgronicaCoreDTOStd.InData.Gis;
using System.Threading.Tasks;
using InData.Gis;

namespace AgronicaNetCore.Gis.BIZ.Services.Gis
{
    /// <summary>
    /// Definisce il contratto per il servizio di logica di business (BIZ)
    /// relativo alla gestione delle configurazioni di clustering GIS.
    /// Ogni metodo rappresenta un'operazione di business completa.
    /// Questi sono i metodi che il Controller (l'API) potrà chiamare.
    /// </summary>
    public interface IGisClusterConfigService
    {
        /// <summary>
        /// Ottiene una configurazione di clustering completa, mappata in un DTO.
        /// </summary>
        /// <param name="configTypeCod">ID del tipo di algoritmo.</param>
        /// <param name="layerCod">ID del layer.</param>
        /// <param name="utente">Username dell'utente.</param>
        /// <param name="objParametriServer">Oggetto di contesto con filtri e informazioni sulla sessione.</param>
        /// <returns>Il DTO della configurazione completa, o null se non trovata.</returns>
        Task<GisClusterConfigSave_InData> GetConfigurationAsync(int configTypeCod, int layerCod, string utente, AgronicaCoreParametriServer objParametriServer);
        
        Task<GisClusterConfigSave_InData[]> GetConfigurationsAsync(string utente, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Crea una nuova configurazione di clustering completa (testata e dettagli).
        /// </summary>
        /// <param name="config">Il DTO contenente i dati della nuova configurazione.</param>
        /// <param name="objParametriServer">Oggetto di contesto con filtri e informazioni sulla sessione.</param>
        Task <bool> CreateConfigurationAsync(GisClusterConfigSave_InData config, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Aggiorna una configurazione di clustering esistente.
        /// </summary>
        /// <param name="config">Il DTO con i dati aggiornati.</param>
        /// <param name="objParametriServer">Oggetto di contesto con filtri e informazioni sulla sessione.</param>
        /// <returns>Il DTO della configurazione aggiornata.</returns>
        Task<GisClusterConfigSave_InData> UpdateConfigurationAsync(GisClusterConfigSave_InData config, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Cancella una configurazione di clustering basandosi sulla sua chiave logica.
        /// </summary>
        /// <param name="configTypeCod">ID del tipo di algoritmo.</param>
        /// <param name="layerCod">ID del layer.</param>
        /// <param name="utente">Username dell'utente.</param>
        /// <param name="objParametriServer">Oggetto di contesto con filtri e informazioni sulla sessione.</param>
        /// <returns>True se la cancellazione è avvenuta con successo.</returns>
        Task<bool> DeleteConfigurationAsync(int configTypeCod, int layerCod, string utente, AgronicaCoreParametriServer objParametriServer);
    }
}