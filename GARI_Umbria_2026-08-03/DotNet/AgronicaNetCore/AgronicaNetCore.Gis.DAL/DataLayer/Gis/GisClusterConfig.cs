using AgronicaNetCore.Base.Models;                
using AgronicaNetCore.Gis.DAL.Resources;        
using AgronicaCoreDTOStd.InData.Gis;            
using Microsoft.Extensions.Localization;          
using System;                                    
using System.Collections.Generic;                 
using System.Data;                                
using System.Dynamic;                           
using System.Text;                               
using System.Threading.Tasks;                  
using InData.Gis;                              

namespace AgronicaNetCore.Gis.DAL.DataLayer.Gis
{
    /// <summary>
    /// Classe per l'accesso ai dati (DAL) per la gestione delle configurazioni di clustering GIS.
    /// </summary>
    public class GisClusterConfig : BaseDALGis, IGisClusterConfig
    {
        #region Costruttore 


        public GisClusterConfig(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
        }

        #endregion

        #region Read Operations

        /// <summary>
        /// Recupera una configurazione di clustering completa (testata + dettagli).
        /// </summary>
        /// <param name="configTypeCod">ID del tipo di algoritmo.</param>
        /// <param name="layerCod">ID del layer.</param>
        /// <param name="utente">Username dell'utente.</param>
        /// <param name="objParametriServer">Oggetto di contesto contenente filtri come finestra temporale e visibilità.</param>
        /// <returns>Un DataTable con i dati della configurazione.</returns>
        public async Task<DataTable> GetConfigAsDataTableAsync(int? configTypeCod, int? layerCod, string utente, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            stbQuery.AppendLine(" SELECT");
            stbQuery.AppendLine("     cfg.LayerElementiGrafici_Clustering_Config_Cod,");
            stbQuery.AppendLine("     cfg.LayerElementiGrafici_Clustering_Type_Cod,");
            stbQuery.AppendLine("     cfg.LayerElementiGrafici_Cod,");
            stbQuery.AppendLine("     l.LayerElementiGrafici_Des,");
            stbQuery.AppendLine("     cfg.Utente,");
            stbQuery.AppendLine("     cfg.Livello_Zoom_Massimo_Visualizzazione_Raggruppata,");
            stbQuery.AppendLine("     det.LayerElementiGrafici_Clustering_Config_Detail_Cod,");
            stbQuery.AppendLine("     det.Descrizione,");
            stbQuery.AppendLine("     det.Parametri");
            stbQuery.AppendLine(" FROM dbo.GIS_LayerElementiGrafici_Clustering_Config AS cfg");
            stbQuery.AppendLine(" INNER JOIN dbo.GIS_LayerElementiGrafici AS l");
            stbQuery.AppendLine("     ON cfg.LayerElementiGrafici_Cod = l.LayerElementiGrafici_Cod");
            stbQuery.AppendLine(" LEFT JOIN dbo.GIS_LayerElementiGrafici_Clustering_Config_Detail AS det");
            stbQuery.AppendLine("     ON cfg.LayerElementiGrafici_Clustering_Config_Cod = det.LayerElementiGrafici_Clustering_Config_Cod");

            // Filtri per identificare il record.
            stbQuery.AppendLine(" WHERE 1 = 1");

            if (configTypeCod.HasValue)
            {
                stbQuery.AppendLine("   AND cfg.LayerElementiGrafici_Clustering_Type_Cod = @ConfigTypeCod");
                parametriSql.Add("@ConfigTypeCod", configTypeCod);
            }

            if (layerCod.HasValue)
            {
                stbQuery.AppendLine("   AND cfg.LayerElementiGrafici_Cod = @LayerCod");
                parametriSql.Add("@LayerCod", layerCod);
            }
    
            stbQuery.AppendLine("   AND cfg.Utente = @Utente");
            stbQuery.AppendLine("   AND cfg.Validita_Inizio <= @dtFine");
            stbQuery.AppendLine("   AND cfg.Validita_Fine >= @dtInizio");

            if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati)
                stbQuery.AppendLine(" AND cfg.inviato >= 0 ");
            else if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati)
                stbQuery.AppendLine(" AND cfg.inviato = -1 ");

            parametriSql.Add("@Utente", utente);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);
            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);

            try
            {
                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        public async Task<string?> GetCentroideWktAsync(
            string piva,
            int saCod,
            int appezza,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(piva)) throw new ArgumentException("Specificare la partita IVA.", nameof(piva));
            if (saCod == 0) throw new ArgumentException("Specificare il codice Sa_Cod.", nameof(saCod));
            if (appezza < 0) throw new ArgumentException("Specificare il codice Appezza.", nameof(appezza));

            const string sql = @"
                SELECT TOP 1 cl.Centroide_GeoEntity_WKT
                FROM GIS_ElementiGrafici_Clustering cl
                INNER JOIN GIS_ElementiGrafici g ON g.ElementoGrafico_Cod = cl.ElementoGrafico_Cod
                INNER JOIN GIS_Entita e           ON e.Entita_Cod          = g.Entita_Cod
                WHERE e.Piva    = @piva
                  AND e.Sa_Cod  = @saCod
                  AND e.Appezza = @appezza";

            var sqlParams = new Dictionary<string, object>
            {
                ["@piva"] = piva,
                ["@saCod"] = saCod,
                ["@appezza"] = appezza
            };

            try
            {
                var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, sqlParams);

                if (dt.Rows.Count == 0)
                    return null;

                var value = dt.Rows[0]["Centroide_GeoEntity_WKT"];
                return value == DBNull.Value ? null : value?.ToString();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<string?> GetPoligonoWktAsync(string piva, int saCod, int appezza, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(piva)) throw new ArgumentException("Specificare la partita IVA.", nameof(piva));
            if (saCod == 0) throw new ArgumentException("Specificare il codice Sa_Cod.", nameof(saCod));
            if (appezza < 0) throw new ArgumentException("Specificare il codice Appezza.", nameof(appezza));

            const string sql = @"
                SELECT TOP 1 COALESCE(g.Poligono_GeoEntity_WKT, g.Poligono_GeoEntity.STAsText()) AS Poligono_GeoEntity_WKT
                FROM GIS_ElementiGrafici g
                INNER JOIN GIS_Entita e ON e.Entita_Cod = g.Entita_Cod
                WHERE e.Piva    = @piva
                  AND e.Sa_Cod  = @saCod
                  AND e.Appezza = @appezza";

            var sqlParams = new Dictionary<string, object>
            {
                ["@piva"] = piva,
                ["@saCod"] = saCod,
                ["@appezza"] = appezza
            };

            try
            {
                var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, sqlParams);
                if (dt.Rows.Count == 0) return null;
                var value = dt.Rows[0]["Poligono_GeoEntity_WKT"];
                return value == DBNull.Value ? null : value?.ToString();
                //var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, sqlParams);
                //if (dt.Rows.Count == 0) return "POLYGON ((8.6755004051702542 39.9504270996087, 8.6755030253845824 39.950414493526161, 8.675522665624289 39.950267253962394, 8.6755416464719559 39.950240413268851, 8.6755271214814478 39.950233849061824, 8.6755280599919473 39.950226813183463, 8.675549224046641 39.9499993460666, 8.6756212826290788 39.949981708769506, 8.6755985078991529 39.950226489886973, 8.675598481523906 39.950226773363759, 8.6755976424322139 39.950235791824923, 8.6755976311516747 39.950235913067175, 8.6755875614920956 39.950344140412739, 8.675587550176493 39.950344262031592, 8.6755867111328389 39.950353279943485, 8.6755866836805726 39.9503535749959, 8.6755862961832744 39.950353553639864, 8.6755862426466468 39.950353756926269, 8.6755856952351262 39.950355835527112, 8.6755863324641425 39.950355913982285, 8.6755849104119775 39.950362755648356, 8.675584793650513 39.950363317401958, 8.67558292961769 39.950372285488932, 8.6755754593189867 39.950408225972986, 8.6755735809647678 39.950417262949649, 8.6755734655508849 39.950417818219087, 8.6755715944298011 39.9504268203933, 8.6755715663459654 39.950426955507773, 8.67556474632694 39.950459767367988, 8.6755004051702542 39.9504270996087))";
                //var value = dt.Rows[0]["Poligono_GeoEntity_WKT"];
                //return value == DBNull.Value ? "POLYGON ((8.6755004051702542 39.9504270996087, 8.6755030253845824 39.950414493526161, 8.675522665624289 39.950267253962394, 8.6755416464719559 39.950240413268851, 8.6755271214814478 39.950233849061824, 8.6755280599919473 39.950226813183463, 8.675549224046641 39.9499993460666, 8.6756212826290788 39.949981708769506, 8.6755985078991529 39.950226489886973, 8.675598481523906 39.950226773363759, 8.6755976424322139 39.950235791824923, 8.6755976311516747 39.950235913067175, 8.6755875614920956 39.950344140412739, 8.675587550176493 39.950344262031592, 8.6755867111328389 39.950353279943485, 8.6755866836805726 39.9503535749959, 8.6755862961832744 39.950353553639864, 8.6755862426466468 39.950353756926269, 8.6755856952351262 39.950355835527112, 8.6755863324641425 39.950355913982285, 8.6755849104119775 39.950362755648356, 8.675584793650513 39.950363317401958, 8.67558292961769 39.950372285488932, 8.6755754593189867 39.950408225972986, 8.6755735809647678 39.950417262949649, 8.6755734655508849 39.950417818219087, 8.6755715944298011 39.9504268203933, 8.6755715663459654 39.950426955507773, 8.67556474632694 39.950459767367988, 8.6755004051702542 39.9504270996087))" : value?.ToString();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public Task<string?> GetFirstPointCentroideWktAsync(string poligonoWkt)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(poligonoWkt))
                    return Task.FromResult<string?>(null);

                // Estrae il contenuto tra le prime parentesi tonde interne: "lon lat, lon lat, ..."
                var start = poligonoWkt.IndexOf('(');
                var end = poligonoWkt.LastIndexOf(')');

                if (start < 0 || end < 0 || end <= start)
                    return Task.FromResult<string?>(null);

                // Rimuove le parentesi esterne e prende la prima coppia (prima della virgola)
                var inner = poligonoWkt.Substring(start + 1, end - start - 1).Trim().Trim('(', ')');
                var primaVoce = inner.Split(',')[0].Trim();

                // Valida che la coppia contenga esattamente 2 valori numerici
                var parti = primaVoce.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parti.Length < 2)
                    return Task.FromResult<string?>(null);

                return Task.FromResult<string?>($"POINT({parti[0]} {parti[1]})");
            }
            catch (Exception ex)
            {
                LogError(ex.Message, null, ex);
                throw;
            }
        }

        #endregion

        #region Write Operations

        /// <summary>
        /// Inserisce una nuova riga di testata di configurazione.
        /// </summary>
        /// <param name="newConfigCod">L'ID primario pre-generato dal BIZ.</param>
        /// <param name="config">Il DTO con i dati da inserire.</param>
        /// <param name="username">L'utente che esegue l'operazione.</param>
        /// <param name="objParametriServer">Oggetto di contesto.</param>
        /// <returns>L'ID della riga inserita.</returns>
        public async Task<int> InsertConfigHeaderAsync(int newConfigCod, GisClusterConfigSave_InData config, string username, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            dynamic parametriSql = new ExpandoObject();

            stbQuery.AppendLine(" INSERT INTO dbo.GIS_LayerElementiGrafici_Clustering_Config");
            stbQuery.AppendLine(" (LayerElementiGrafici_Clustering_Config_Cod, LayerElementiGrafici_Clustering_Type_Cod, LayerElementiGrafici_Cod, Utente, Livello_Zoom_Massimo_Visualizzazione_Raggruppata, Username_Creazione, Username_Modifica, inviato)");
            stbQuery.AppendLine(" VALUES");
            stbQuery.AppendLine(" (@ConfigCod, @TypeCod, @LayerCod, @Utente, @ZoomMax, @Username, @Username, 0)");

            parametriSql.ConfigCod = newConfigCod;
            parametriSql.TypeCod = config.LayerElementiGraficiConfigTypeCod;
            parametriSql.LayerCod = config.LayerElementiGraficiCod;
            parametriSql.Utente = config.Utente;
            parametriSql.ZoomMax = config.LivelloZoomMassimoVisualizzazioneRaggruppata;
            parametriSql.Username = username;

            try
            {
                await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return newConfigCod;
        }

        /// <summary>
        /// Inserisce una nuova riga di dettaglio di configurazione.
        /// </summary>
        /// <param name="configCod">L'ID della testata a cui il dettaglio è associato.</param>
        /// <param name="detail">Il DTO con i dati del dettaglio.</param>
        /// <param name="username">L'utente che esegue l'operazione.</param>
        /// <param name="objParametriServer">Oggetto di contesto.</param>
        public async Task<bool> InsertConfigDetailAsync(int configCod, GisClusterConfigDetail_InData detail, string username, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            dynamic parametriSql = new ExpandoObject();
            bool success;

            stbQuery.AppendLine(" INSERT INTO dbo.GIS_LayerElementiGrafici_Clustering_Config_Detail");
            stbQuery.AppendLine(" (LayerElementiGrafici_Clustering_Config_Cod, LayerElementiGrafici_Clustering_Config_Detail_Cod, Descrizione, Parametri, Username_Creazione, Username_Modifica, inviato)");
            stbQuery.AppendLine(" VALUES");
            stbQuery.AppendLine(" (@ConfigCod, @DetailCod, @Descrizione, @Parametri, @Username, @Username, 0)");

            parametriSql.ConfigCod = configCod;
            parametriSql.DetailCod = detail.Id;
            parametriSql.Descrizione = detail.Description;
            parametriSql.Parametri = detail.Parameters;
            parametriSql.Username = username;

            try
            {
               success =  await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return success;
        }
        #endregion

        #region Update Operations

        /// <summary>
        /// Aggiorna la riga di testata di una configurazione esistente.
        /// </summary>
        /// <param name="config">Il DTO con i dati aggiornati.</param>
        /// <param name="username">L'utente che esegue l'operazione.</param>
        /// <param name="objParametriServer">Oggetto di contesto.</param>
        /// <returns>True se l'aggiornamento ha avuto successo.</returns>
        public async Task<bool> UpdateConfigHeaderAsync(GisClusterConfigSave_InData config, string username, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            dynamic parametriSql = new ExpandoObject();
            bool success;

            stbQuery.AppendLine(" UPDATE dbo.GIS_LayerElementiGrafici_Clustering_Config");
            stbQuery.AppendLine(" SET Livello_Zoom_Massimo_Visualizzazione_Raggruppata = @ZoomMax,");
            stbQuery.AppendLine("     Data_Modifica = GETDATE(),");
            stbQuery.AppendLine("     Username_Modifica = @Username");
            stbQuery.AppendLine(" WHERE LayerElementiGrafici_Clustering_Type_Cod = @TypeCod");
            stbQuery.AppendLine("   AND LayerElementiGrafici_Cod = @LayerCod");
            stbQuery.AppendLine("   AND Utente = @Utente");

            parametriSql.ZoomMax = config.LivelloZoomMassimoVisualizzazioneRaggruppata;
            parametriSql.Username = username;
            parametriSql.TypeCod = config.LayerElementiGraficiConfigTypeCod;
            parametriSql.LayerCod = config.LayerElementiGraficiCod;
            parametriSql.Utente = config.Utente;

            try
            {
                success = await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return success;
        }
        #endregion

        #region Delete Operations

        /// <summary>
        /// Cancella tutte le righe di dettaglio associate a un ID di configurazione.
        /// </summary>
        /// <param name="configCod">L'ID della configurazione di cui cancellare i dettagli.</param>
        /// <param name="objParametriServer">Oggetto di contesto.</param>
        public async Task DeleteConfigDetailsAsync(int configCod, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            dynamic parametriSql = new ExpandoObject();

            stbQuery.AppendLine(" DELETE FROM dbo.GIS_LayerElementiGrafici_Clustering_Config_Detail");
            stbQuery.AppendLine(" WHERE LayerElementiGrafici_Clustering_Config_Cod = @ConfigCod");

            parametriSql.ConfigCod = configCod;

            try
            {
                await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        /// <summary>
        /// Cancella una riga di testata di configurazione.
        /// </summary>
        /// <param name="configTypeCod">ID del tipo di algoritmo.</param>
        /// <param name="layerCod">ID del layer.</param>
        /// <param name="utente">Username dell'utente.</param>
        /// <param name="objParametriServer">Oggetto di contesto.</param>
        /// <returns>True se la cancellazione ha avuto successo.</returns>
        public async Task<bool> DeleteConfigHeaderAsync(int configTypeCod, int layerCod, string utente, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            dynamic parametriSql = new ExpandoObject();
            bool success;

            stbQuery.AppendLine(" DELETE FROM dbo.GIS_LayerElementiGrafici_Clustering_Config");
            stbQuery.AppendLine(" WHERE LayerElementiGrafici_Clustering_Type_Cod = @ConfigTypeCod");
            stbQuery.AppendLine("   AND LayerElementiGrafici_Cod = @LayerCod");
            stbQuery.AppendLine("   AND Utente = @Utente");

            parametriSql.ConfigTypeCod = configTypeCod;
            parametriSql.LayerCod = layerCod;
            parametriSql.Utente = utente;

            try
            {
                success = await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return success;
        }
        #endregion
    }
}