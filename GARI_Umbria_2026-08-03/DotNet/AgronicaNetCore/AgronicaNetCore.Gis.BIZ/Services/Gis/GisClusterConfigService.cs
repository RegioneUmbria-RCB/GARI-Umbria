using AgronicaNetCore.Gis.DAL.DataLayer.Gis;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using AgronicaNetCore.Base.Models;
using AgronicaCoreDTOStd.InData.Gis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using InData.Gis;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using AgronicaNetCore.Gis.BIZ.Resources;

namespace AgronicaNetCore.Gis.BIZ.Services.Gis
{
    /// <summary>
    /// Servizio di logica di business (BIZ) per la gestione delle configurazioni di clustering GIS.
    /// </summary>
    public class GisClusterConfigService : BaseServiceGisBIZ, IGisClusterConfigService
    {
        private readonly IGisClusterConfig _gisDal;
        private readonly IAgro_Sequence _agroSequence;

        public GisClusterConfigService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider,
            localizer)
        {
            _gisDal = _serviceProvider.GetRequiredService<IGisClusterConfig>();
            _agroSequence = _serviceProvider.GetRequiredService<IAgro_Sequence>();
        }

        /// <summary>
        /// Ottiene e mappa una configurazione di clustering.
        /// </summary>
        public async Task<GisClusterConfigSave_InData> GetConfigurationAsync(int configTypeCod, int layerCod,
            string utente, AgronicaCoreParametriServer objParametriServer)
        {
            var dtConfig = await _gisDal.GetConfigAsDataTableAsync(configTypeCod, layerCod, utente, objParametriServer);
            return MapDataTableToDto(dtConfig);
        }

        /// <summary>
        /// Ottiene e mappa una configurazione di clustering.
        /// </summary>
        public async Task<GisClusterConfigSave_InData[]> GetConfigurationsAsync(string utente,
            AgronicaCoreParametriServer objParametriServer)
        {
            var dtConfigs = await _gisDal.GetConfigAsDataTableAsync(null, null, utente, objParametriServer);
            return MapDataTableToDtos(dtConfigs).ToArray();
        }

        /// <summary>
        /// Crea una nuova configurazione completa.
        /// </summary>
        public async Task<bool> CreateConfigurationAsync(GisClusterConfigSave_InData config,
            AgronicaCoreParametriServer objParametriServer)
        {
            var res = false;

            try
            {
                if (config == null || config.LayerElementiGraficiCod <= 0 || string.IsNullOrWhiteSpace(config.Utente))
                    throw new ArgumentException("Dati di input per la creazione non validi.");

                await OpenConnectionAsync(objParametriServer);

                var dtExisting = await _gisDal.GetConfigAsDataTableAsync(config.LayerElementiGraficiConfigTypeCod,
                    config.LayerElementiGraficiCod, config.Utente, objParametriServer);
                if (dtExisting != null && dtExisting.Rows.Count > 0)
                    throw new InvalidOperationException("Una configurazione per questo utente e layer esiste già.");

                var newConfigId = await _agroSequence.NuovoId_TabellaAsync("GIS_LayerElementiGrafici_Clustering_Config",
                    0, 0, objParametriServer);
                if (newConfigId <= 0)
                    throw new InvalidOperationException("Impossibile generare un nuovo ID.");

                // 2. Inserisce la testata, passando il contesto.
                await _gisDal.InsertConfigHeaderAsync(newConfigId, config, objParametriServer.UsernameOperazione,
                    objParametriServer);

                // 3. Inserisce i dettagli, passando il contesto.
                if (config.Details != null)
                {
                    foreach (var detail in config.Details)
                    {
                        await _gisDal.InsertConfigDetailAsync(newConfigId, detail,
                            objParametriServer.UsernameOperazione, objParametriServer);
                    }
                }

                CloseTransaction(objParametriServer, false);
                res = true;
            }
            catch (Exception ex)
            {
                CloseTransaction(objParametriServer, true);

                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            finally
            {
                CloseConnection(objParametriServer);
            }

            return res;
        }

        /// <summary>
        /// Aggiorna una configurazione esistente.
        /// </summary>
        public async Task<GisClusterConfigSave_InData> UpdateConfigurationAsync(GisClusterConfigSave_InData config,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (config == null || config.LayerElementiGraficiCod <= 0 || string.IsNullOrWhiteSpace(config.Utente))
                throw new ArgumentException("Dati di input per l'aggiornamento non validi.");


            // 1. Recupera l'ID della configurazione esistente.
            var dtExisting = await _gisDal.GetConfigAsDataTableAsync(config.LayerElementiGraficiConfigTypeCod,
                config.LayerElementiGraficiCod, config.Utente, objParametriServer);
            if (dtExisting == null || dtExisting.Rows.Count == 0)
                throw new InvalidOperationException("Configurazione non trovata. Impossibile aggiornare.");
            var configCod = Convert.ToInt32(dtExisting.Rows[0]["LayerElementiGrafici_Clustering_Config_Cod"]);

            // 2. Aggiorna la testata.
            await _gisDal.UpdateConfigHeaderAsync(config, objParametriServer.UsernameOperazione, objParametriServer);

            // 3. Strategia "Delete-then-Insert" per i dettagli.
            await _gisDal.DeleteConfigDetailsAsync(configCod, objParametriServer);
            if (config.Details != null)
            {
                foreach (var detail in config.Details)
                {
                    await _gisDal.InsertConfigDetailAsync(configCod, detail, objParametriServer.UsernameOperazione,
                        objParametriServer);
                }
            }

            // 4. Ritorna la configurazione aggiornata per conferma.
            return await GetConfigurationAsync(config.LayerElementiGraficiConfigTypeCod, config.LayerElementiGraficiCod,
                config.Utente, objParametriServer);
        }

        /// <summary>
        /// Cancella una configurazione.
        /// </summary>
        public async Task<bool> DeleteConfigurationAsync(int configTypeCod, int layerCod, string utente,
            AgronicaCoreParametriServer objParametriServer)
        {
            // 1. Trova l'ID della configurazione da cancellare.
            var dtExisting =
                await _gisDal.GetConfigAsDataTableAsync(configTypeCod, layerCod, utente, objParametriServer);
            if (dtExisting == null || dtExisting.Rows.Count == 0)
            {
                return true; // Non esiste, quindi è già "cancellata".
            }

            var configCod = Convert.ToInt32(dtExisting.Rows[0]["LayerElementiGrafici_Clustering_Config_Cod"]);

            // 2. Cancella prima i dettagli.
            await _gisDal.DeleteConfigDetailsAsync(configCod, objParametriServer);

            // 3. Cancella la testata.
            return await _gisDal.DeleteConfigHeaderAsync(configTypeCod, layerCod, utente, objParametriServer);
        }


        #region Metodi Privati (Helper)

        /// <summary>
        /// Metodo helper privato che trasforma un DataTable grezzo (dal DAL) 
        /// in un DTO tipizzato e strutturato.
        /// </summary>
        private GisClusterConfigSave_InData? MapDataTableToDto(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            var firstRow = dt.Rows[0];

            var configDto = new GisClusterConfigSave_InData
            {
                LayerElementiGraficiConfigTypeCod =
                    Convert.ToInt32(firstRow["LayerElementiGrafici_Clustering_Type_Cod"]),
                LayerElementiGraficiCod = Convert.ToInt32(firstRow["LayerElementiGrafici_Cod"]),
                LayerElementiGraficiDes = firstRow["LayerElementiGrafici_Des"].ToString(),
                Utente = firstRow["Utente"].ToString(),
                LivelloZoomMassimoVisualizzazioneRaggruppata =
                    Convert.ToInt32(firstRow["Livello_Zoom_Massimo_Visualizzazione_Raggruppata"]),
                Details = new List<GisClusterConfigDetail_InData>()
            };

            foreach (DataRow row in dt.Rows)
            {
                if (row["LayerElementiGrafici_Clustering_Config_Detail_Cod"] != DBNull.Value)
                {
                    configDto.Details.Add(new GisClusterConfigDetail_InData
                    {
                        Id = Convert.ToInt32(row["LayerElementiGrafici_Clustering_Config_Detail_Cod"]),
                        Description = row["Descrizione"].ToString(),
                        Parameters = row["Parametri"].ToString()
                    });
                }
            }

            return configDto;
        }

        /// <summary>
        /// Metodo helper privato che trasforma un DataTable grezzo (dal DAL) 
        /// in un DTO tipizzato e strutturato.
        /// </summary>
        private ICollection<GisClusterConfigSave_InData> MapDataTableToDtos(DataTable dt)
        {
            var result = new List<GisClusterConfigSave_InData>();
            if (dt.Rows.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dt.Rows)
            {
                var configTypeCod = Convert.ToInt32(row["LayerElementiGrafici_Clustering_Type_Cod"]);
                var layerCod = Convert.ToInt32(row["LayerElementiGrafici_Cod"]);

                var configDto = result.FirstOrDefault(x =>
                    x.LayerElementiGraficiConfigTypeCod == configTypeCod && x.LayerElementiGraficiCod == layerCod);
                if (configDto == null)
                {
                    configDto = new GisClusterConfigSave_InData
                    {
                        LayerElementiGraficiConfigTypeCod = configTypeCod,
                        LayerElementiGraficiCod = layerCod,
                        LayerElementiGraficiDes = row["LayerElementiGrafici_Des"].ToString(),
                        Utente = row["Utente"].ToString(),
                        LivelloZoomMassimoVisualizzazioneRaggruppata =
                            Convert.ToInt32(row["Livello_Zoom_Massimo_Visualizzazione_Raggruppata"]),
                        Details = new List<GisClusterConfigDetail_InData>()
                    };
                    result.Add(configDto);
                }

                if (row["LayerElementiGrafici_Clustering_Config_Detail_Cod"] != DBNull.Value)
                {
                    configDto.Details.Add(new GisClusterConfigDetail_InData
                    {
                        Id = Convert.ToInt32(row["LayerElementiGrafici_Clustering_Config_Detail_Cod"]),
                        Description = row["Descrizione"].ToString(),
                        Parameters = row["Parametri"].ToString()
                    });
                }
            }

            return result;
        }

        #endregion
    }
}