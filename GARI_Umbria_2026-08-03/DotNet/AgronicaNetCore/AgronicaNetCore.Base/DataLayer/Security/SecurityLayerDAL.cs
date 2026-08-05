using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Models;
using System.Data;
using System.Dynamic;
using System.Text;

namespace AgronicaNetCore.Base.DataLayer.Security
{
    public class SecurityLayerDAL : DAL_Base, ISecurityLayerDAL
    {
        public SecurityLayerDAL(IServiceProvider provider) : base(provider)
        {
        }

        public async Task<DataTable> LeggiConnessioniAsync(AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT * FROM Connessioni (NOLOCK) ");

            try
            {
                return await GetDataProvider(objParametriSuperServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriSuperServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiConfigurazioneSitiAsync(string chiave, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            return await LeggiConfigurazioneSitiInternalAsync(chiave, objParametriSuperServer);
        }

        public async Task<DataTable> LeggiConfigurazioneSitiAsync(string chiave, AgronicaCoreParametriServer objParametriServer)
        {
            return await LeggiConfigurazioneSitiInternalAsync(chiave, objParametriServer);
        }

        public async Task<DataTable> LeggiConfigurazioneSitiAsync(List<string> chiavi, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            return await LeggiConfigurazioneSitiInternalAsync(chiavi, objParametriSuperServer);
        }

        public async Task<DataTable> LeggiConfigurazioneSitiAsync(List<string> chiavi, AgronicaCoreParametriServer objParametriServer)
        {
            return await LeggiConfigurazioneSitiInternalAsync(chiavi, objParametriServer);
        }

        public async Task<bool> AggiornaValoriAsync(List<string> chiavi, List<string> valori, AgronicaCoreParametriServer objParametriServer)
        {
            return await AggiornaValoriInternalAsync(chiavi, valori, objParametriServer);
        }

        public async Task<bool> AggiornaValoriAsync(List<string> chiavi, List<string> valori, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            return await AggiornaValoriInternalAsync(chiavi, valori, objParametriSuperServer);
        }

        public async Task<string> LeggiConfigurazioneSitiScalareAsync(string chiave, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {

            string valore = string.Empty;

            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();

            CreateQueryLeggiConfigurazioneSiti(stbQuery, parametriSql, chiave);

            try
            {

                //Leggo prima dal Server
                DataTable dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);

                if (dt.Rows.Count == 0)
                {
                    //Se non trovo nulla leggo dal SuperServer
                    dt = await GetDataProvider(objParametriSuperServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);              
                }

                if (dt.Rows.Count > 0)
                {
                    valore = dt.Rows[0]["Valore"].ToString()!;
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return valore;

        }

        public async Task<(string UrlEngine, string ApiKey)> RecuperaConfigurazioneEngineAsync(
            string ChiaveUrlEngine,
            string ChiaveApiKey,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            (string UrlEngine, string ApiKey, string TenantName) config = await RecuperaConfigurazioneEngineInternalAsync(ChiaveUrlEngine, ChiaveApiKey, string.Empty, objParametriServer, objParametriSuperServer);

            return (config.UrlEngine, config.ApiKey);
        }

        public async Task<(string UrlEngine, string ApiKey,string TenantName)> RecuperaConfigurazioneEngineAsync(
            string ChiaveUrlEngine,
            string ChiaveApiKey,
            string ChiaveTenantName,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            return await RecuperaConfigurazioneEngineInternalAsync(ChiaveUrlEngine, ChiaveApiKey, ChiaveTenantName, objParametriServer, objParametriSuperServer);
        }

        /// <summary>
        /// Recupera la configurazione dell'engine. Andando a scalare leggendo prima dal server e se non trova la Chiave oppure ha un valore vuoto legge dal super server.
        /// </summary>
        /// <param name="ChiaveUrlEngine">La chiave per l'URL dell'engine.</param>
        /// <param name="ChiaveApiKey">La chiave per l'API key.</param>
        /// <param name="ChiaveTenantName">La chiave per il nome del tenant.</param>
        /// <param name="objParametriServer">I parametri del server.</param>
        /// <param name="objParametriSuperServer">I parametri del super server.</param>
        /// <returns>Una tupla contenente l'URL dell'engine, l'API key e il nome del tenant.</returns>
        /// <exception cref="Exception"></exception>

        private async Task<(string UrlEngine, string ApiKey, string TenantName)> RecuperaConfigurazioneEngineInternalAsync(
            string ChiaveUrlEngine,
            string ChiaveApiKey,
            string ChiaveTenantName,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            string urlEngine = string.Empty;

            string apiKey = string.Empty;

            string tenantName = string.Empty;

            List<string> chiavi = new List<string> { };

            if(ChiaveUrlEngine != string.Empty)
            {
                chiavi.Add(ChiaveUrlEngine);
            }
            if(ChiaveApiKey != string.Empty)
            {
                chiavi.Add(ChiaveApiKey);
            }
            if(ChiaveTenantName != string.Empty)
            {
                chiavi.Add(ChiaveTenantName);
            }

            if(chiavi.Count > 0)
            {
                DataTable DT_Server = await LeggiConfigurazioneSitiAsync(chiavi, objParametriServer);

                if (DT_Server?.Rows?.Count > 0)
                {
                    foreach (DataRow row in DT_Server.Rows)
                    {

                        if (urlEngine == string.Empty)
                        {
                            if (row["Chiave"].ToString() == ChiaveUrlEngine && row["Valore"].ToString() != string.Empty)
                            {
                                urlEngine = row["Valore"].ToString() ?? string.Empty;
                            }
                            else
                            {
                                DataTable data = await LeggiConfigurazioneSitiAsync(ChiaveUrlEngine, objParametriSuperServer);
                                if (data.Rows.Count > 0)
                                {
                                    urlEngine = data.Rows[0]["Valore"].ToString() ?? string.Empty;
                                }
                            }
                        }

                        if (apiKey == string.Empty)
                        {
                            if (row["Chiave"].ToString() == ChiaveApiKey && row["Valore"].ToString() != string.Empty)
                            {
                                apiKey = row["Valore"].ToString() ?? string.Empty;
                            }
                            else
                            {
                                DataTable data = await LeggiConfigurazioneSitiAsync(ChiaveApiKey, objParametriSuperServer);
                                if (data.Rows.Count > 0)
                                {
                                    apiKey = data.Rows[0]["Valore"].ToString() ?? string.Empty;
                                }
                            }
                        }

                        if (tenantName == string.Empty)
                        {
                            if (row["Chiave"].ToString() == ChiaveTenantName && row["Valore"].ToString() != string.Empty)
                            {
                                tenantName = row["Valore"].ToString() ?? string.Empty;
                            }
                            else
                            {
                                DataTable data = await LeggiConfigurazioneSitiAsync(ChiaveTenantName, objParametriSuperServer);
                                if (data.Rows.Count > 0)
                                {
                                    tenantName = data.Rows[0]["Valore"].ToString() ?? string.Empty;
                                }
                            }
                        }
                    }
                }
                else
                {
                    DataTable DT_Super_Server = await LeggiConfigurazioneSitiAsync(new List<string> { ChiaveUrlEngine, ChiaveApiKey, ChiaveTenantName }, objParametriSuperServer);

                    if (DT_Super_Server?.Rows?.Count > 0)
                    {
                        urlEngine = DT_Super_Server.Rows[0][ChiaveUrlEngine].ToString() ?? string.Empty;
                        apiKey = DT_Super_Server.Rows[0][ChiaveApiKey].ToString() ?? string.Empty;
                        tenantName = DT_Super_Server.Rows[0][ChiaveTenantName].ToString() ?? string.Empty;
                    }
                }
            }


            if (string.IsNullOrWhiteSpace(urlEngine) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(tenantName))
                throw new Exception($"La chiave di configurazione '{ChiaveUrlEngine}' o '{ChiaveApiKey}' o '{tenantName}' non è stata trovata nel sistema di configurazione.");

            if (!Uri.TryCreate(urlEngine, UriKind.Absolute, out Uri? uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new Exception($"La chiave di configurazione '{ChiaveUrlEngine}' contiene un valore non valido: il valore non è un URL HTTP/HTTPS valido.");
            }

            return (urlEngine, apiKey, tenantName);
        }



        private async Task<DataTable> LeggiConfigurazioneSitiInternalAsync(List<string> chiavi, AgronicaCoreParametri objParametri)
        {
            var stbQuery = new StringBuilder();

            var parametriSql = new Dictionary<string, object>();
            var parametriSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            CreateQueryLeggiConfigurazioneSiti(stbQuery, parametriSqlIn, chiavi);

            try
            {
                return await GetDataProvider(objParametri).ExecuteReadAsync(stbQuery.ToString(), parametriSql,parametriSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                throw;
            }
        }

        private async Task<DataTable> LeggiConfigurazioneSitiInternalAsync(string chiave, AgronicaCoreParametri objParametri)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();

            CreateQueryLeggiConfigurazioneSiti(stbQuery, parametriSql, chiave);

            try
            {
                return await GetDataProvider(objParametri).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                throw;
            }
        }

        private async Task<bool> AggiornaValoriInternalAsync(List<string> chiavi, List<string> valori, AgronicaCoreParametri objParametri)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new ExpandoObject();

            stbQuery.AppendLine(" UPDATE ");
            stbQuery.AppendLine("        Configurazione_Siti");
            stbQuery.AppendLine(" SET ");
            stbQuery.AppendLine("        Valore = CASE");

            for (int i = 0; i < chiavi.Count; i++)
            {
                var chiave = chiavi[i];
                var valore = valori[i];

                stbQuery.AppendLine($"        WHEN Chiave = @chiave_{i} THEN @valore_{i}");
                parametriSql.TryAdd($"@chiave_{i}", chiave);
                parametriSql.TryAdd($"@valore_{i}", valore);
            }

            stbQuery.AppendLine("        ELSE Valore END");
            stbQuery.AppendLine(" WHERE ");
            stbQuery.AppendLine("        Chiave IN (");

            for (int i = 0; i < chiavi.Count; i++)
            {
                if (i != (chiavi.Count - 1))
                    stbQuery.AppendLine($"@chiave_{i},");
                else
                    stbQuery.AppendLine($"@chiave_{i})");
            }

            try
            {
                return await GetDataProvider(objParametri).Execute_WriteAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                throw;
            }
        }

        private void CreateQueryLeggiConfigurazioneSiti(StringBuilder stbQuery, Dictionary<string, object> parametriSql, string chiave)
        {
            stbQuery.AppendLine("SELECT * FROM Configurazione_Siti (NOLOCK) ");

            if (!string.IsNullOrEmpty(chiave))
            {
                stbQuery.AppendLine("WHERE Chiave=@Chiave");
                parametriSql.TryAdd("@Chiave", chiave);
            }
        }

        private void CreateQueryLeggiConfigurazioneSiti(StringBuilder stbQuery, Dictionary<string, Dictionary<Type, List<object>>> parametriSqlIn, List<string> chiavi)
        {
            stbQuery.AppendLine("SELECT * FROM Configurazione_Siti (NOLOCK) ");
            if (chiavi.Any())
            {
                stbQuery.AppendLine("WHERE Chiave IN (@chiave)");
                parametriSqlIn.TryAdd("@chiave", FormatClauseIn(chiavi));
            }
        }
    }
}
