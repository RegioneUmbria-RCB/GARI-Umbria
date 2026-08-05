using AgronicaDataProvider6.Interfaces;
using AgronicaDataProvider6.Models;
using AgronicaDataProvider6.Providers;
using AgronicaDataProvider6.Settings;
using Microsoft.VisualBasic;
using Serilog;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Data;
using Microsoft.Extensions.Options;

namespace AgronicaDataProvider6
{
    public sealed class DataProvider6Factory : IDataProvider6Factory
    {
        private static DataProvider6Factory? instance = null;
        private static readonly object lockObject = new object();
        private readonly IServiceProvider _serviceProvider;
        private readonly SqlSettings _sqlSettings;

        public DataProvider6Factory(IServiceProvider provider,IOptions<SqlSettings> sqlOpt)
        {
            _serviceProvider = provider;
            _sqlSettings = sqlOpt.Value;
        }

        public static DataProvider6Factory Instance
        {
            get
            {
                lock (lockObject)
                {
                    instance ??= new DataProvider6Factory(null,null);
                    return instance;
                }
            }
        }

        public IDataProvider GetDataProvider<T>()
        {
            return new DataProvider6<T>();
        }

        public IDataProvider GetDataProvider<T>(IServiceProvider provider, DataProvider6Settings settings)
        {
            return new DataProvider6<T>(provider, settings);
        }

        public IDataProvider GetDataProvider(SqlProviderConnection sqlConn, int languageCode, DbConnection? connection=null,DbTransaction? transaction=null)
        {
            SqlConnectionStringBuilder builder = new()
            {
                DataSource = sqlConn.DataSource,
                InitialCatalog = sqlConn.InitialCatalog,
                UserID = sqlConn.UserId,
                Password = sqlConn.Password
            };
            DataProvider6Settings settings = new()
            {
                ConnectionString = AdjustConnectionString(builder.ConnectionString),
                LanguageCode = languageCode,
                Connection = connection,
                Transaciton = transaction,
                UseReadUncommited = _sqlSettings.UseReadUnCommitted
            };

            return GetDataProvider<SqlDataProvider>(_serviceProvider, settings);
        }

        public string AdjustConnectionString(string connestionString)
        {
            return GetConnectionStringFromOleToSql(connestionString);
        }

        private string GetConnectionStringFromOleToSql(string oleDBConnectionString)
        {
            char[] separator = new char[2] { ';', '=' };
            string rval = "";
            string DaTenere = "server,data source,initial catalog,user id,password";

            string[] arrayFromString = oleDBConnectionString.Split(separator);
            for (int i = 0; i <= arrayFromString.Length - 2; i += 2)
            {
                if (DaTenere.IndexOf(Strings.Trim(arrayFromString[i]).ToLower(), 0) > -1)
                    rval += arrayFromString[i] + "=" + arrayFromString[i + 1] + ";";
            }

            return rval;
        }
    }

}
