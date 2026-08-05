using System.Data;
using System.Dynamic;

namespace AgronicaDataProvider6.Interfaces
{
    public interface IReadDataProvider
    {
        Task<DataTable> ExecuteReadAsync(string SqlString);

        /// <summary>
        /// DA NON USARE ASSOLUTAMENTE PERCHé SPECIFICO PER LE CORE API
        /// </summary>
        /// <param name="SqlString"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("Usare solo per core api, negli altri casi usare il metodo asincrono")]
        DataTable ExecuteRead(string SqlString, Dictionary<string, object> parameters, Dictionary<string, Dictionary<Type, List<object>>>? clause_in_params = null);
        Task<DataTable> ExecuteReadAsync(string SqlString, Dictionary<string, object> parameters, Dictionary<string, Dictionary<Type, List<object>>>? clause_in_params = null);
        Task<T> ExecuteReadAsync<T>(string SqlString) where T : class;

        Task<T> ExecuteReadAsync<T>(string SqlString, Dictionary<string, object> parameters, Dictionary<string, Dictionary<Type, List<object>>>? clause_in_params = null) where T : class;

        Task<DataSet> ExecuteMultipleReadAsync(string sqlString, string dataSetName);

        Task<DataSet> ExecuteMultipleReadAsync(string sqlString, ExpandoObject parameters, string dataSetName);

        Task<DataSet> ExecuteMultipleReadAsync(string sqlString, string dataSetName, Dictionary<string, object> parameters, Dictionary<string, Dictionary<Type, List<object>>>? clause_in_params = null);
    }
}
