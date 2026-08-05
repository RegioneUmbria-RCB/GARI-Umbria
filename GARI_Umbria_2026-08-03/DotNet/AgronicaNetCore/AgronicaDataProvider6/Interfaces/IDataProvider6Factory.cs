using AgronicaDataProvider6.Models;
using AgronicaDataProvider6.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaDataProvider6.Interfaces
{
    public interface IDataProvider6Factory
    {
        IDataProvider GetDataProvider<T>();
        IDataProvider GetDataProvider<T>(IServiceProvider provider, DataProvider6Settings settings);
        IDataProvider GetDataProvider( SqlProviderConnection conn, int languageCode, DbConnection? connection = null, DbTransaction? transaction = null);
        string AdjustConnectionString(string connestionString);
    }
}
