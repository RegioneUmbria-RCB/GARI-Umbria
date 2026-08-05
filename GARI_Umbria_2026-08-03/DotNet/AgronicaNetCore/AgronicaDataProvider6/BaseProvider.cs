using AgronicaDataProvider6.Extensions;
using AgronicaDataProvider6.Models;
using AgronicaDataProvider6.Providers;
using Newtonsoft.Json;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;

namespace AgronicaDataProvider6
{

    public class BaseProvider
    {
        protected readonly DataProvider6Settings _providerSetting;
        private DbConnection? _connection;
        private DbTransaction? _transaction;

        public DataProvider6Settings Settings 
        {
            get { return _providerSetting;  } 
        }

        public DbConnection? Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        public DbTransaction? Transaction
        {
            get { return _transaction; }
            set { _transaction = value; }
        }

        public string ConnectionString
        {
            get 
            { 
                if(_providerSetting == null)
                    return string.Empty;

                return _providerSetting.ConnectionString; 
            }
        }

        public int LanguageCode
        {
            get
            {
                if (_providerSetting == null)
                    return 1;

                return _providerSetting.LanguageCode;
            }
        }

        public BaseProvider() { }
        public BaseProvider(DataProvider6Settings providerSetting): this()
        {
            _providerSetting = providerSetting;
            _connection = providerSetting.Connection;
            _connection=providerSetting.Connection;
            _transaction = providerSetting.Transaciton;
        }

        protected T ChangeType<T>(DataTable obj)
        {
            switch (typeof(T))
            {
                case Type dt when dt == typeof(DataTable):
                    return(T)Convert.ChangeType(obj, typeof(DataTable));
                case Type eo when eo == typeof(List<IDictionary<string, object>>):
                    var dic = obj.ToDictionaryList();
                    return (T)Convert.ChangeType(dic, typeof(List<IDictionary<string,object>>));
                case Type ex when ex == typeof(List<ExpandoObject>):
                    var expandoObj = obj.ToExpandoObjectList();
                    return (T)Convert.ChangeType(expandoObj, typeof(List<ExpandoObject>));
                default:
                    return ConvertToTypedObject<T>(obj);
            }

        }

        protected T ConvertToTypedObject<T>(DataTable obj)
        {

            var json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);

        }

        protected bool ApplyReadUncommited(string querySql)
        {
            if (!querySql.Contains("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED") && _providerSetting.UseReadUncommited && _transaction == null)
                return true;
            else
                return false;
        }
    }
}
