using AgronicaCoreDTOStd.Identity;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Services.Security;
using AgronicaNetCore.UtilityDB.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.UtilityDB.DAL.DataLayer.UtilityDB
{
    public class UtilityDB : BaseDALUtilityDB, IUtilityDB
    {
        protected readonly ISecurityService? _securityService;
        public UtilityDB(IServiceProvider provider, ISecurityService? securityService, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _securityService = securityService;
        }

        public async Task<int> ReadSQLVersionMajorAsync(AgronicaCoreParametri objParametri)
        {
            //sostituisce il metodo Read_SQL_Version_YearAsync, che non funzionava bene in condizioni di server molto carico https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            int major = 0;
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine("SELECT SERVERPROPERTY('ProductVersion') as Version");

            try
            {
                var result = await GetDataProvider(objParametri).ExecuteReadAsync(stbQuery.ToString());
                
                //non faccio nessun null check e nessun try parse perché la query dovrebbe sempre funzionare e dovrebbe sempre restituire la versione con 4 numeri, se qualcosa va male è giusto che lanci l'exception piuttosto che restituire 0
                string version = result.Rows[0]["Version"].ToString();
                var versionNumbers = version.Split(".");
                major = int.Parse(versionNumbers[0]);

                return major;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                throw;
            }
        }

        [Obsolete("Usare il metodo ReadSQLVersionMajorAsync")]
        public async Task<int> Read_SQL_Version_YearAsync(AgronicaCoreParametri objParametri)
        {

            int version = 0;

            var stbQuery = new StringBuilder();
            DataTable result = new DataTable();

            stbQuery.AppendLine(" SELECT @@Version AS Version ");

            try
            {
                result = await GetDataProvider(objParametri).ExecuteReadAsync(stbQuery.ToString());

                string versionStr = result.Rows[0]["Version"].ToString()!;

                string[] s1 = versionStr.Split(new char[0]);

                int l = s1.Length - 1;

                for (int i = 0; i <= l; i++)
                {
                    int _int;
                    if (s1[i].Length == 4 && int.TryParse(s1[i], out _int))
                    {
                        version = _int;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                throw;
            }
            return version;

        }

        public async Task<int> Read_SQL_Compatibility_LevelAsync(AgronicaCoreParametri objParametri)
        {

            int compatibilityLevel = 0;

            Dictionary<string, object> parSql = new();
            StringBuilder stbQuery = new();
            DataTable result = new();

            stbQuery.AppendLine(" SELECT compatibility_level ");
            stbQuery.AppendLine(" FROM sys.databases ");
            stbQuery.AppendLine(" WHERE name = @name ");

            parSql.Add("@name", GetDBName(objParametri));

            try
            {
                result = await GetDataProvider(objParametri).ExecuteReadAsync(stbQuery.ToString(), parSql);

                compatibilityLevel = (int)(byte)result.Rows[0]["compatibility_level"]!;

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                throw;
            }
            return compatibilityLevel;

        }

        public string GetDBName(AgronicaCoreParametri objParametri)
        {
            _securityService?.SetConnectionString(objParametri);
            string UserDBName = "";

            string[] dummy = objParametri.StringaConnessione.Split(';');

            //[0] Provider = SQLOLEDB;
            //[1] Server = ;
            //[2] Initial Catalog = DB_Name;
            //[3] User Id = ;
            //[4] Password = ;

            if (dummy.Length > 0)
            {
                UserDBName = dummy[2].Split("=")[1];
            }

            return UserDBName;

        }

        public async Task<string> GetPivaRealeAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            string pivaReale = piva;
            Dictionary<string, object> parSql = new();
            StringBuilder stbQuery = new();
            DataTable result = new();

            stbQuery.AppendLine(" SELECT CASE WHEN ISNULL(partitaIvaReale, '') = '' THEN PIVA ELSE partitaIvaReale END PivaReale ");
            stbQuery.AppendLine(" FROM Imprese ");
            stbQuery.AppendLine(" WHERE Piva = @piva ");

            parSql.Add("@piva", piva);

            try
            {
                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);
                if (result != null && result.Rows.Count == 1) 
                    pivaReale = result.Rows[0]["PivaReale"].ToString();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return pivaReale;
        }
    }
}
