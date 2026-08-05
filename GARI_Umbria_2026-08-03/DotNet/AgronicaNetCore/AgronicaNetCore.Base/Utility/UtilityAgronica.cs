using AgronicaDataProvider6.Models;
using AgronicaDataProvider6.Settings;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.Json;

namespace AgronicaNetCore.Base.Utility
{
    public class UtilityAgronica
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static AgronicaCoreParametri convertStringtoOBJparametri(string s, IOptions<SecuritySettings> options)
        {
            var sE = Security.Stringa_Decodifica_LANCompatibile(s, CostantiPersonalizzate.AgroKey_EncoderDecoder, options);
            return (JsonConvert.DeserializeObject<AgronicaCoreParametri>(sE))!;
        }

        public static AgronicaCoreParametriServer ConvertStringToObjParametriServer(string s, IOptions<SecuritySettings> options)
        {
            var sE = Security.Stringa_Decodifica_LANCompatibile(s, CostantiPersonalizzate.AgroKey_EncoderDecoder, options);
            return (JsonConvert.DeserializeObject<AgronicaCoreParametriServer>(sE))!;
        }

        public static AgronicaCoreParametriUtenti ConvertStringToObjParametriUtenti(string s, IOptions<SecuritySettings> options)
        {
            var sE = Security.Stringa_Decodifica_LANCompatibile(s, CostantiPersonalizzate.AgroKey_EncoderDecoder, options);
            return (JsonConvert.DeserializeObject<AgronicaCoreParametriUtenti>(sE))!;
        }

        public static AgronicaCoreParametriSuperServer ConvertStringToObjParametriSuperServer(string s, IOptions<SecuritySettings> options)
        {
            var sE = Security.Stringa_Decodifica_LANCompatibile(s, CostantiPersonalizzate.AgroKey_EncoderDecoder, options);
            return (JsonConvert.DeserializeObject<AgronicaCoreParametriSuperServer>(sE))!;
        }

        public static SqlProviderConnection getSqlConnectionFromObjParametri(AgronicaCoreParametri obj)
        {
            var ret = new SqlProviderConnection();
            var connPars = obj.StringaConnessione.Split(";");
            foreach (var par in connPars)
            {
                var p = par.Split("=");
                switch (p[0].Trim())
                {
                    case "Server":
                        ret.DataSource = p[1];
                        break;
                    case "Initial Catalog":
                    case "Database":
                        ret.InitialCatalog = p[1];
                        break;
                    case "User Id":
                        ret.UserId = p[1];
                        break;
                    case "Password":
                        ret.Password = p[1];
                        break;
                    default:
                        //opzione non mappata
                        break;
                }
            }
            return ret;
        }

        [Obsolete("Method is deprecated, please use DAL_Base.FormatClauseIn instead.", false)]
        public static string GenerateParameterizedStringForInClause(string customParamsName, List<string> stringList, Dictionary<string, object> parSql)
        {
            List<string>? paramStringList = new();
            string strParameterized = "";

            foreach (string _string in stringList)
            {
                string parId = "@" + customParamsName;
                parId += stringList.IndexOf(_string).ToString();

                paramStringList.Add(parId);
                parSql.Add(parId, _string);
            }

            strParameterized = string.Join(", \n", paramStringList);

            paramStringList.Clear();
            paramStringList = null;

            return strParameterized;
        }

        /// <summary>
        /// In caso di url assoluto, provvederà a rimuovere l'ultimo '/'
        /// Es. http://test/ => http://test
        /// In caso di url relativo, estrae il valore di 'LanToWebSiteBasePath' e rimuove l'ultimo '/'
        /// Es. /test/ => {LanToWebSiteBasePath}/test
        /// </summary>
        /// <param name="_securityLayer"></param>
        /// <param name="url"></param>
        /// <param name="objParametriServer"></param>
        /// <param name="objParametriSuperServer"></param>
        /// <returns></returns>
        public static string AggiustaUrl(ISecurityLayerDAL _securityLayer, string url, AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            string result = null!;
            Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri)
                result = uri.ToString().TrimEnd(Path.AltDirectorySeparatorChar);
            else
            {
                string baseUrl = _securityLayer.LeggiConfigurazioneSitiScalareAsync("LanToWebSiteBasePath",
                    objParametriServer!, objParametriSuperServer).GetAwaiter().GetResult();
                result = new Uri(new Uri(baseUrl), url).ToString().TrimEnd(Path.AltDirectorySeparatorChar);
            }
            return result;
        }

        public static string ConvertObjParametriToString(AgronicaCoreParametri objParametri, IOptions<SecuritySettings> options)
        {
            var str = JsonConvert.SerializeObject(objParametri);
            return Security.Stringa_Codifica_LANCompatibile(str, CostantiPersonalizzate.AgroKey_EncoderDecoder, options);
        }

        //// <summary>
        /// JsonConvert.SerializeObject di Newtonsoft.Json usa il tipo a runtime dell'oggetto, non il tipo dichiarato del parametro. Quindi se passi un FmisContextDataNutrizione o altri oggetti derivanti da base, il JSON includerà anche RaccoglitoreCod — non solo i campi del base.
        /// </summary>
        public static string CreateFmisContextHeader(IConfiguration _config, FmisContextDataBase fmisContextData)
        {
            try
            {
                var json = JsonConvert.SerializeObject(fmisContextData);
                var cr2 = _config.GetValue<string>("cr2");
                return Security.EncryptString(json, cr2);
            }
            catch
            (Exception ex)
            {
                // Log dell'eccezione se necessario
                throw new InvalidOperationException("Errore durante la creazione dell'header fmis-context", ex);
            }

        }

        public static string? ToJson(object value)
        {
            string? result = null;
            if (value != null)
                result = System.Text.Json.JsonSerializer.Serialize(value, JsonOptions);
            return result;
        }

        public static T? FromJsonStringToType<T>(string value)
        {
            T? result = default;
            if (!string.IsNullOrEmpty(value.ToString()))
                result = System.Text.Json.JsonSerializer.Deserialize<T>(value!, JsonOptions);
            return result;
        }
    }
}
