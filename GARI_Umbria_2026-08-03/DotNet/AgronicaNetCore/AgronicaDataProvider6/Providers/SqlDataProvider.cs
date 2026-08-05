using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Text;
using AgronicaDataProvider6.Exceptions;
using AgronicaDataProvider6.Interfaces;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AgronicaDataProvider6.Providers
{
    public class SqlDataProvider : BaseProvider, IDataProvider
    {
        private readonly ILogger<SqlDataProvider> _logger;

        private const int MAXIMUM_PARAMETERS_NUMBER = 2100;
        private const int MAXIMUM_PARAMETERS_CLAUSE_IN_BEFORE_USE_TVP = 10;

        public SqlDataProvider(IServiceProvider provider, DataProvider6Settings settings)
            : base(settings)
        {
            _logger = provider.GetRequiredService<ILogger<SqlDataProvider>>();
        }

        public async Task<(DbConnection?, DbTransaction?)> OpenConnectionAsync(
            bool OpenTransaction = true,
            IsolationLevel? level = null
        )
        {
            return await InternalOpenConnectionAsync(OpenTransaction, level);
        }

        public void CloseConnection(ref DbConnection? connection, bool RollbackTransaction = false)
        {
            InternalCloseConnection(ref connection, RollbackTransaction);
        }

        public async Task<DbTransaction?> OpenTransactionAsync(
            DbConnection connection,
            IsolationLevel? level = null
        )
        {
            return await InternalOpenTransactionAsync(connection, level);
        }

        public void CloseTransaction(ref DbTransaction transaction, bool Rollback = false)
        {
            InternalCloseTransaction(ref transaction, Rollback);
        }

        private async Task<(DbConnection?, DbTransaction?)> InternalOpenConnectionAsync(
            bool OpenTransaction,
            IsolationLevel? level
        )
        {
            (DbConnection?, DbTransaction?) ret = (null, null);
            try
            {
                if (Connection == null)
                    Connection = new SqlConnection(ConnectionString);
                ret.Item1 = Connection;

                if (Connection.State != ConnectionState.Open)
                    await Connection.OpenAsync();
                if (OpenTransaction && Transaction == null)
                    Transaction = await InternalOpenTransactionAsync(Connection, level);
                ret.Item2 = Transaction;
            }
            catch (Exception ex)
            {
                ret = (null, null);
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }
            return ret;
        }

        private void InternalCloseConnection(ref DbConnection? connection, bool RollbackTransaction)
        {
            try
            {
                if (connection != null)
                {
                    if (Transaction != null)
                    {
                        if (Transaction.Connection != null)
                        {
                            if (RollbackTransaction)
                                Transaction.Rollback();
                            else
                                Transaction.Commit();
                        }

                        Transaction.Dispose();
                        Transaction = null;
                    }
                    //if (connection.State == ConnectionState.Open)
                    connection.Close();

                    connection.Dispose();
                    connection = null;
                    Connection.Close();
                    Connection.Dispose();
                    Connection = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }
        }

        private async Task<DbTransaction?> InternalOpenTransactionAsync(
            DbConnection connection,
            IsolationLevel? level
        )
        {
            DbTransaction? transaction = null;
            try
            {
                if (connection is null)
                    throw new ArgumentNullException("Connection can't be null");

                if (connection.State != ConnectionState.Open)
                    throw new Exception("No connection open");

                if (Transaction != null)
                    throw new Exception("Transaction already defined");
                else
                {
                    if (level == null)
                        transaction = await Connection.BeginTransactionAsync();
                    else
                        transaction = await Connection.BeginTransactionAsync((IsolationLevel)level);
                    Transaction = transaction;
                }
            }
            catch (Exception ex)
            {
                transaction = null;
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }
            return transaction;
        }

        private void InternalCloseTransaction(ref DbTransaction transaction, bool Rollback)
        {
            try
            {
                if (Connection != null)
                {
                    if (Connection.State != ConnectionState.Open)
                        throw new Exception("No connection open");

                    if (transaction != null)
                    {
                        if (Rollback)
                            transaction.Rollback();
                        else
                            transaction.Commit();

                        transaction.Dispose();
                        transaction = null;
                        Transaction = null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }
        }

        public async Task<DataTable> ExecuteReadAsync(string SqlString)
        {
            return await this.ExecuteReadAsync<DataTable>(SqlString);
        }

        public async Task<T> ExecuteReadAsync<T>(string SqlString)
            where T : class
        {
            var result = await this.ExecuteReadAsync(SqlString, null, null);
            return ChangeType<T>(result);
        }

        /// <summary>
        /// DA NON USARE ASSOLUTAMENTE PERCHé SPECIFICO PER LE CORE API
        /// </summary>
        /// <param name="SqlString"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("Usare solo per core api, negli altri casi usare il metodo asincrono")]
        public DataTable ExecuteRead(
            string SqlString,
            Dictionary<string, object> parameters,
            Dictionary<string, Dictionary<Type, List<object>>> clause_in_params
        )
        {
            DataTable result = new DataTable();

            try
            {
                Translate(ref SqlString, LanguageCode);
                result = InternalExecuteRead(SqlString, parameters, clause_in_params);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }

            return result;
        }

        public async Task<DataTable> ExecuteReadAsync(
            string SqlString,
            Dictionary<string, object> parameters,
            Dictionary<string, Dictionary<Type, List<object>>> clause_in_params
        )
        {
            DataTable result = new DataTable();

            try
            {
                Translate(ref SqlString, LanguageCode);
                result = await InternalExecuteReadAsync(SqlString, parameters, clause_in_params);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }

            return result;
        }

        private DataTable InternalExecuteRead(
            string SqlString,
            Dictionary<string, object> parameters,
            Dictionary<string, Dictionary<Type, List<object>>> clause_in_params
        )
        {
            SqlCommand? command = null;
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable result = new DataTable();
            Dictionary<int, Type> filter_key_list = new Dictionary<int, Type>();

            bool LocalConnection = (Connection == null ? true : false);

            try
            {
                //Lavez - 04/12/2024 - Controllo preliminare numero massimo parametri
                if (!CheckMaximumParameterNumber(parameters, clause_in_params))
                    throw new Exception(
                        string.Format(
                            "Superato numero massimo di parametri applicabili ({0}). Operazione interrotta.",
                            MAXIMUM_PARAMETERS_NUMBER
                        )
                    );

                //Lavez - 18/03/2025 - controllo chiavi simili
                if (!CheckSimilarKey(clause_in_params))
                    throw new Exception(
                        string.Format(
                            "Parametri di clausola IN simili. Query non eseguibile, riverificare i nomi delle chiavi. Operazione interrotta."
                        )
                    );

                if (Connection == null)
                {
                    Connection = new SqlConnection(ConnectionString);
                    Connection.Open();
                }
                else
                {
                    if (Connection.State == ConnectionState.Closed)
                    {
                        Connection.ConnectionString = ConnectionString;
                        Connection.Open();
                    }
                }

                command = ((SqlConnection)Connection).CreateCommand();
                command.CommandTimeout = 600; //TODO Parametrize

                //Lavez -  04/12/2024 - clausola in
                if (clause_in_params != null)
                {
                    SqlString = SetQuery_ClauseIN(
                        command,
                        SqlString,
                        parameters,
                        clause_in_params,
                        filter_key_list,
                        null
                    );
                }

                if (ApplyReadUncommited(SqlString))
                    SqlString = " SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; \n" + SqlString;

                command.CommandText = SqlString;

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(new SqlParameter(param.Key, param.Value));
                    }
                }

                adapter.SelectCommand = command;
                if (Transaction != null)
                {
                    adapter.SelectCommand.Transaction = (SqlTransaction)Transaction;
                }
                adapter.Fill(result);

                //Lavez - 20/01/2025 - pulizia tabella filtro
                if (clause_in_params != null)
                {
                    Clear_Tables_Filter(filter_key_list);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }
            finally
            {
                if (command != null)
                    command.Dispose();
                if (adapter != null)
                    adapter.Dispose();
                if (LocalConnection)
                {
                    if (Connection.State != ConnectionState.Closed)
                        Connection.Close();
                    Connection.Dispose();
                    Connection = null;
                }
            }
            return result;
        }

        private async Task<DataTable> InternalExecuteReadAsync(
            string SqlString,
            Dictionary<string, object> parameters,
            Dictionary<string, Dictionary<Type, List<object>>> clause_in_params
        )
        {
            SqlCommand? command = null;
            DataTable result = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            Dictionary<int, Type> filter_key_list = new Dictionary<int, Type>();

            bool LocalConnection = (Connection == null ? true : false);
            var sb = new StringBuilder();
            try
            {
                await PerformChecksAndOpenConnectionAsync(parameters, clause_in_params);

                command = ((SqlConnection)Connection).CreateCommand();

                await PrepareCommandForReadingAsync(SqlString, parameters, clause_in_params, command, adapter, filter_key_list, sb);

                adapter.Fill(result);
                //// --- INIZIO MODIFICA ASINCRONA ---
                //using (var reader = await command.ExecuteReaderAsync())
                //{
                //    // result.Load() è sincrono, ma opera su un SqlDataReader
                //    // che è stato popolato asincronamente. Questa è la pratica comune.
                //    result.Load(reader);
                //}
                //// --- FINE MODIFICA ASINCRONA ---
                
                await CleanInParametersAsync(clause_in_params, filter_key_list, sb);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }
            finally
            {
                await CloseConnectionAndDisposeEverythingAsync(command, adapter, LocalConnection);
            }

            sb.Clear();
            return result;
        }

        private void printParameter(string paramName, object paramValue, StringBuilder sb)
        {
            if (paramValue == null)
                return;

            string declareLine = "";
            string setLine = "";
            var type = paramValue.GetType();

            if (type == typeof(string))
            {
                declareLine = $"DECLARE {paramName} AS VARCHAR({paramValue.ToString().Length})";
                setLine = $"SET {paramName} = '{paramValue}'";
            }
            else if (type == typeof(int) || type == typeof(long) || type == typeof(short))
            {
                declareLine = $"DECLARE {paramName} AS INT";
                setLine = $"SET {paramName} = {paramValue}";
            }
            else if (type == typeof(DateTime))
            {
                declareLine = $"DECLARE {paramName} AS DATETIME";
                setLine =
                    $"SET {paramName} = CONVERT(DATETIME, '{((DateTime)paramValue):yyyy-MM-dd HH:mm:ss.fff}', 120)";
            }
            else if (type == typeof(bool))
            {
                declareLine = $"DECLARE {paramName} AS BIT";
                setLine = $"SET {paramName} = {(Convert.ToBoolean(paramValue) ? 1 : 0)}";
            }
            else if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
            {
                declareLine = $"DECLARE {paramName} AS DECIMAL(18,6)";
                setLine = $"SET {paramName} = {paramValue.ToString().Replace(',', '.')}";
            }
            else
            {
                declareLine = $"DECLARE {paramName} AS VARCHAR({paramValue.ToString().Length})";
                setLine = $"SET {paramName} = '{paramValue}'";
            }

            sb.AppendLine(declareLine);
            sb.AppendLine(setLine + Environment.NewLine);
        }

        public async Task<T> ExecuteReadAsync<T>(
            string SqlString,
            Dictionary<string, object> parameters,
            Dictionary<string, Dictionary<Type, List<object>>>? clause_in_params
        )
            where T : class
        {
            var result = await this.ExecuteReadAsync(SqlString, parameters, clause_in_params);
            return ChangeType<T>(result);
        }

        public async Task<DataSet> ExecuteMultipleReadAsync(string sqlString, string dataSetName)
        {
            return await this.ExecuteMultipleReadAsync(sqlString, null, dataSetName);
        }

        public async Task<DataSet> ExecuteMultipleReadAsync(string sqlString, string dataSetName, Dictionary<string, object> parameters, Dictionary<string, Dictionary<Type, List<object>>>? clause_in_params = null)
        {
            SqlCommand? command = null;
            DataSet result = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            Dictionary<int, Type> filter_key_list = new Dictionary<int, Type>();

            bool LocalConnection = (Connection == null ? true : false);
            var sb = new StringBuilder();

            try
            {
                await PerformChecksAndOpenConnectionAsync(parameters, clause_in_params);

                command = ((SqlConnection)Connection).CreateCommand();

                await PrepareCommandForReadingAsync(sqlString, parameters, clause_in_params, command, adapter, filter_key_list, sb);

                adapter.Fill(result, dataSetName);

                await CleanInParametersAsync(clause_in_params, filter_key_list, sb);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }
            finally
            {
                await CloseConnectionAndDisposeEverythingAsync(command, adapter, LocalConnection);
            }

            sb.Clear();
            return result;
        }

        public async Task<DataSet> ExecuteMultipleReadAsync(
            string sqlString,
            ExpandoObject parameters,
            string dataSetName
        )
        {
            SqlCommand? command = null;
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet result = new DataSet();

            bool LocalConnection = (Connection == null ? true : false);

            try
            {
                await OpenConnectionAsync();

                command = ((SqlConnection)Connection).CreateCommand();
                command.CommandTimeout = 600; //TODO Parametrize

                if (ApplyReadUncommited(sqlString))
                    sqlString = " SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; \n" + sqlString;

                command.CommandText = sqlString;

                if (parameters != null)
                {
                    foreach (KeyValuePair<string, object> kvp in parameters)
                        command.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value));
                }

                adapter.SelectCommand = command;
                adapter.Fill(result, dataSetName);
            }
            catch (Exception ex)
            {
                // TODO LOGGER
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }
            finally
            {
                await CloseConnectionAndDisposeEverythingAsync(command, adapter, LocalConnection);
            }

            return result;
        }

        public string Get_DataBase_Name_From_ConnectionString(string connectionString)
        {
            //esempio: "Provider=SQLOLEDB;Server=*****;Initial Catalog=*****_Utenti;User Id=*****;Password=*****;"

            var r = connectionString.Split(';')[2];
            var s = r.Split('=')[1];
            return s.Trim();
        }

        public string Get_Instance_Name_From_ConnectionString(string connectionString)
        {
            //esempio: "Provider=SQLOLEDB;Server=*****;Initial Catalog=*****_Utenti;User Id=*****;Password=*****;"

            var r = connectionString.Split(';')[1];
            var s = r.Split('=')[1];
            return s.Trim();
        }

        public async Task<bool> Execute_WriteAsync(string sqlString, ExpandoObject parameters)
        {
            SqlCommand? command = null;
            var retVal = false;

            bool LocalConnection = (Connection == null ? true : false);
            bool LocalTransaction = (Transaction == null ? true : false);

            try
            {
                if (Connection == null)
                {
                    Connection = new SqlConnection(ConnectionString);
                    await Connection.OpenAsync();
                }
                else
                {
                    if (Connection.State == ConnectionState.Closed)
                    {
                        Connection.ConnectionString = ConnectionString;
                        await Connection.OpenAsync();
                    }
                }
                if (LocalTransaction)
                {
                    if (Transaction != null)
                    {
                        throw new Exception(
                            "Local transaction = true and Transacation obj not null"
                        );
                    }
                    else
                    {
                        Transaction = await Connection.BeginTransactionAsync();
                    }
                }

                command = ((SqlConnection)Connection).CreateCommand();
                command.Transaction = (SqlTransaction)Transaction!;

                command.CommandTimeout = 600; // TODO PARAMETRIZE
                command.CommandText = sqlString;
                if (parameters != null)
                {
                    foreach (KeyValuePair<string, object> kvp in parameters)
                        command.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value));
                }
                await command.ExecuteNonQueryAsync();
                retVal = true;
            }
            catch (Exception ex)
            {
                if (LocalTransaction)
                {
                    if (Transaction != null)
                        await Transaction.RollbackAsync();
                }
                // TODO LOGGER
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }
            finally
            {
                if (command != null)
                    command.Dispose();
                if (LocalTransaction)
                {
                    if (Transaction != null)
                    {
                        await Transaction.CommitAsync();
                        Transaction.Dispose();
                        Transaction = null;
                    }
                }
                if (LocalConnection)
                {
                    if (Connection.State != ConnectionState.Closed)
                        await Connection.CloseAsync();
                    Connection.Dispose();
                    Connection = null;
                }
            }

            return retVal;
        }

        public async Task<bool> Execute_WriteAsync(string sqlString)
        {
            return await Execute_WriteAsync(sqlString, null);
        }

        public async Task<bool> Execute_WriteVarBinaryAsync(
            string sqlString,
            Dictionary<string, byte[]> cmdParameters,
            Dictionary<string, object>? parameters = null
        )
        {
            SqlCommand? command = null;
            var retVal = false;

            bool localConnection = Connection is null;
            bool localTransaction = Transaction is null;

            try
            {
                if (Connection is null)
                {
                    Connection = new SqlConnection(ConnectionString);
                    await Connection.OpenAsync();
                }
                else
                {
                    if (Connection.State == ConnectionState.Closed)
                    {
                        Connection.ConnectionString = ConnectionString;
                        await Connection.OpenAsync();
                    }
                }

                if (localTransaction)
                {
                    if (Transaction != null)
                        throw new Exception(
                            "Local transaction = true and Transaction obj not null"
                        );

                    Transaction = await Connection.BeginTransactionAsync();
                }

                command = ((SqlConnection)Connection).CreateCommand();
                command.Transaction = (SqlTransaction)Transaction!;
                command.CommandTimeout = 600; // TODO PARAMETRIZE
                command.CommandText = sqlString;

                if (cmdParameters is not null && cmdParameters.Count > 0)
                {
                    foreach (var param in cmdParameters)
                    {
                        var sqlParam = new SqlParameter(param.Key, SqlDbType.VarBinary);
                        sqlParam.Value = param.Value;
                        command.Parameters.Add(sqlParam);
                    }
                }

                if (parameters is not null)
                {
                    foreach (var param in parameters)
                        command.Parameters.Add(new SqlParameter(param.Key, param.Value ?? DBNull.Value));
                }

                await command.ExecuteNonQueryAsync();
                retVal = true;
            }
            catch (Exception ex)
            {
                if (localTransaction && Transaction is not null)
                    await Transaction.RollbackAsync();

                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }
            finally
            {
                if (command is not null)
                    command.Dispose();

                if (localTransaction)
                {
                    if (Transaction is not null)
                    {
                        await Transaction.CommitAsync();
                        Transaction.Dispose();
                        Transaction = null;
                    }
                }

                if (localConnection)
                {
                    if (Connection.State != ConnectionState.Closed)
                        await Connection.CloseAsync();
                    Connection.Dispose();
                    Connection = null;
                }
            }

            return retVal;
        }

        #region "Clause IN"

        private string SetQuery_ClauseIN(
            SqlCommand command,
            string sqlstring,
            Dictionary<string, object> parameters,
            Dictionary<string, Dictionary<Type, List<object>>> clause_in_params,
            Dictionary<int, Type> filter_key_list,
            StringBuilder sb
        )
        {
            //force table use if the number of parameters + the number of parameters for clause_in_parameters is higher than maximum allowed
            bool forceTableUse = MustForceTableUse(parameters, clause_in_params);

            foreach (var dic in clause_in_params)
            {
                foreach (var lst in dic.Value)
                {
                    if (
                        lst.Value.Count > MAXIMUM_PARAMETERS_CLAUSE_IN_BEFORE_USE_TVP
                        || forceTableUse
                    )
                    {
                        var id_filtro = InternalWriteClauseIN_Values(lst);
                        if (id_filtro == -1)
                            throw new Exception(
                                "Filtro clausola IN non valorizzato. Impossibile proseguire"
                            );

                        sqlstring = sqlstring.Replace(
                            dic.Key.ToString(),
                            String.Format(
                                "(select val from {0} (NOLOCK) where id = {1}_flt)",
                                (GetFilterType(lst) == 1 ? "_TMP_FILTRO_STR" : "_TMP_FILTRO_INT"),
                                dic.Key.ToString()
                            )
                        );

                        command.Parameters.Add(
                            new SqlParameter(dic.Key.ToString() + "_flt", id_filtro)
                        );

                        filter_key_list.Add(id_filtro, lst.Key);
                    }
                    else
                    {
                        sqlstring = sqlstring.Replace(
                            dic.Key.ToString(),
                            BuildClauseIN_InLine(lst, dic.Key.ToString(), command.Parameters, sb)
                        );
                    }
                }
            }
            return sqlstring;
        }

        private async Task<string> SetQuery_ClauseINAsync(
            SqlCommand command,
            string sqlstring,
            Dictionary<string, object> parameters,
            Dictionary<string, Dictionary<Type, List<object>>> clause_in_params,
            Dictionary<int, Type> filter_key_list,
            StringBuilder sb
        )
        {
            //force table use if the number of parameters + the number of parameters for clause_in_parameters is higher than maximum allowed
            bool forceTableUse = MustForceTableUse(parameters, clause_in_params);

            foreach (var dic in clause_in_params)
            {
                foreach (var lst in dic.Value)
                {
                    if (
                        lst.Value.Count > MAXIMUM_PARAMETERS_CLAUSE_IN_BEFORE_USE_TVP
                        || forceTableUse
                    )
                    {
                        var id_filtro = await InternalWriteClauseIN_ValuesAsync(lst, sb);
                        if (id_filtro == -1)
                            throw new Exception(
                                "Filtro clausola IN non valorizzato. Impossibile proseguire"
                            );

                        sqlstring = sqlstring.Replace(
                            dic.Key.ToString(),
                            String.Format(
                                "(select val from {0} (NOLOCK) where id = {1}_flt)",
                                (GetFilterType(lst) == 1 ? "_TMP_FILTRO_STR" : "_TMP_FILTRO_INT"),
                                dic.Key.ToString()
                            )
                        );

                        var flt_key = dic.Key.ToString() + "_flt";
                        command.Parameters.Add(new SqlParameter(flt_key, id_filtro));
                        if (Debugger.IsAttached)
                        {
                            sb.AppendLine($"DECLARE {flt_key} AS INT");
                            sb.AppendLine(
                                $"SET {flt_key} = {id_filtro.ToString()} " + Environment.NewLine
                            );
                        }

                        filter_key_list.Add(id_filtro, lst.Key);
                    }
                    else
                    {
                        sqlstring = sqlstring.Replace(
                            dic.Key.ToString(),
                            BuildClauseIN_InLine(lst, dic.Key.ToString(), command.Parameters, sb)
                        );
                    }
                }
            }
            return sqlstring;
        }

        private bool MustForceTableUse(
            Dictionary<string, object> parameters,
            Dictionary<string, Dictionary<Type, List<object>>> clause_in_params
        )
        {
            var totalNumberOfParameters = parameters is null ? 0 : parameters.Count;
            foreach (var param in clause_in_params.Values)
            {
                foreach (var p in param.Values)
                {
                    totalNumberOfParameters += p.Count;
                }
            }

            return totalNumberOfParameters >= MAXIMUM_PARAMETERS_NUMBER;
        }

        private bool CheckMaximumParameterNumber(
            Dictionary<string, object> parameters,
            Dictionary<string, Dictionary<Type, List<object>>> clause_in_params
        )
        {
            var count = 0;
            if (parameters is not null)
            {
                count += parameters.Count;
            }
            if (clause_in_params is not null) //anche quando i clause_in_params vengono aggiunti con tabella servono comunque dei parametri, ne teniamo conto in questo punto
            {
                foreach (var value in clause_in_params.Values)
                {
                    count += value.Count; //serve un parametro per ogni lista di oggetti
                }
            }
            return count < MAXIMUM_PARAMETERS_NUMBER;
        }

        private bool CheckSimilarKey(
            Dictionary<string, Dictionary<Type, List<object>>> clause_in_parameters
        )
        {
            if (clause_in_parameters is null)
                return true;
            foreach (var key in clause_in_parameters.Keys)
            {
                if (clause_in_parameters.Keys.Any(x => x.Contains(key) && !x.Equals(key)))
                    return false;
            }
            return true;
        }

        private int GetFilterType(KeyValuePair<Type, List<object>> lst)
        {
            var tab_type = 1;
            int g;
            if (lst.Value.Count() > 0)
            {
                switch (lst.Key)
                {
                    case Type t when t == typeof(string):
                        tab_type = 1;
                        break;
                    case Type t when t == typeof(int):
                        tab_type = 2;
                        break;
                    default:
                        throw new Exception("Type not mapped");
                }
            }
            return tab_type;
        }

        private string BuildClauseIN_InLine(
            KeyValuePair<Type, List<object>> values,
            string key,
            SqlParameterCollection parameters,
            StringBuilder sb
        )
        {
            var clauseIn_string = "";
            //var tab_type = GetFilterType(values);
            var seq = 1;
            foreach (var val in values.Value)
            {
                var par = $"{key}_{seq}";
                clauseIn_string += $"{par},";
                parameters.Add(new SqlParameter(par, val));
                if (Debugger.IsAttached)
                {
                    var tab_type = GetFilterType(values);
                    if (tab_type == 1)
                    {
                        sb?.AppendLine(
                            $"DECLARE {par} AS VARCHAR({val.ToString().Length.ToString()})"
                        );
                        sb?.AppendLine($"SET {par} = '{val.ToString()}' {Environment.NewLine}");
                    }
                    else
                    {
                        sb?.AppendLine($"DECLARE {par} AS INT;");
                        sb?.AppendLine($"SET {par} = {val.ToString()} {Environment.NewLine}");
                    }
                }
                seq++;
            }

            clauseIn_string = clauseIn_string.Substring(0, clauseIn_string.Length - 1);

            return clauseIn_string;
        }

        private int InternalWriteClauseIN_Values(KeyValuePair<Type, List<object>> values)
        {
            SqlCommand? command = null;

            var ID = -1;

            bool LocalConnection = (Connection == null ? true : false);
            bool LocalTransaction = true; //In questo caso la transazione è sempre dedicata

            var tab_filter = "";
            try
            {
                var tab_type = GetFilterType(values);
                if (tab_type == 1)
                    tab_filter = "_TMP_FILTRO_STR";
                else
                    tab_filter = "_TMP_FILTRO_INT";

                using (var bulkCopy = new SqlBulkCopy(Connection as SqlConnection))
                {
                    ID = GetNewIDTableFilter(tab_filter);

                    var dataTable = new DataTable();

                    dataTable.Columns.Add("ID", typeof(int));
                    dataTable.Columns.Add("Seq", typeof(int));
                    if (tab_type == 1)
                        dataTable.Columns.Add("value", typeof(string));
                    else
                        dataTable.Columns.Add("value", typeof(int));

                    var seq = 1;
                    foreach (var val in values.Value)
                    {
                        dataTable.Rows.Add(ID, seq, val);
                        seq++;
                    }

                    bulkCopy.DestinationTableName = tab_filter;

                    bulkCopy.WriteToServer(dataTable);
                    dataTable.Dispose();
                }
            }
            catch (Exception ex)
            {
                ID = -1;
                if (LocalTransaction)
                {
                    if (Transaction != null)
                        Transaction.Rollback();
                }
                // TODO LOGGER
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }
            finally
            {
                if (command != null)
                    command.Dispose();
                if (LocalTransaction)
                {
                    if (Transaction != null)
                    {
                        Transaction.Commit();
                        Transaction.Dispose();
                        Transaction = null;
                    }
                }
                if (LocalConnection)
                {
                    if (Connection.State != ConnectionState.Closed)
                        Connection.Close();
                    Connection.Dispose();
                    Connection = null;
                }
            }

            return ID;
        }

        private async Task<int> InternalWriteClauseIN_ValuesAsync(
            KeyValuePair<Type, List<object>> values,
            StringBuilder sb
        )
        {
            SqlCommand? command = null;

            var ID = -1;

            bool LocalConnection = (Connection == null ? true : false);
            bool LocalTransaction = true; //In questo caso la transazione è sempre dedicata

            var tab_filter = "";
            try
            {
                var tab_type = GetFilterType(values);
                if (tab_type == 1)
                    tab_filter = "_TMP_FILTRO_STR";
                else
                    tab_filter = "_TMP_FILTRO_INT";

                using (var bulkCopy = new SqlBulkCopy(Connection as SqlConnection))
                {
                    ID = await GetNewIDTableFilterAsync(tab_filter);

                    var dataTable = new DataTable();

                    dataTable.Columns.Add("ID", typeof(int));
                    dataTable.Columns.Add("Seq", typeof(int));
                    if (tab_type == 1)
                        dataTable.Columns.Add("value", typeof(string));
                    else
                        dataTable.Columns.Add("value", typeof(int));

                    var seq = 1;
                    foreach (var val in values.Value)
                    {
                        dataTable.Rows.Add(ID, seq, val);
                        if (Debugger.IsAttached)
                        {
                            sb.AppendLine(
                                $"INSERT INTO {tab_filter} values ({ID},{seq},{(tab_type == 1 ? "'" + val + "'" : val)});"
                            );
                        }
                        seq++;
                    }

                    bulkCopy.DestinationTableName = tab_filter;

                    await bulkCopy.WriteToServerAsync(dataTable);
                    dataTable.Dispose();
                }
            }
            catch (Exception ex)
            {
                ID = -1;
                if (LocalTransaction)
                {
                    if (Transaction != null)
                        await Transaction.RollbackAsync();
                }
                // TODO LOGGER
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }
            finally
            {
                if (command != null)
                    await command.DisposeAsync();
                if (LocalTransaction)
                {
                    if (Transaction != null)
                    {
                        await Transaction.CommitAsync();
                        await Transaction.DisposeAsync();
                        Transaction = null;
                    }
                }
                if (LocalConnection)
                {
                    if (Connection.State != ConnectionState.Closed)
                        await Connection.CloseAsync();
                    await Connection.DisposeAsync();
                    Connection = null;
                }
            }

            return ID;
        }

        /// <summary>
        /// Esegue un inserimento massivo asincrono di dati in una tabella del database
        /// </summary>
        /// <param name="dataTable">DataTable contenente i dati da inserire.</param>
        /// <param name="tableName">Nome della tabella di destinazione nel database.</param>
        /// <param name="columnMappings">Mappature opzionali tra i nomi delle colonne del DataTable (source) e quelli della tabella di destinazione. Se null, si assume che i nomi delle colonne corrispondano.</param>
        /// <param name="batchSize">Dimensione del batch per l'inserimento dei dati.</param>
        /// <returns>Restituisce true se l'inserimento è stato completato con successo, altrimenti false.</returns>
        public async Task<bool> ExecuteBulkInsertAsync(
            DataTable dataTable,
            string tableName,
            Dictionary<string, string> columnMappings = null,
            int batchSize = 1000
        )
        {
            var response = false;

            bool localConnection = Connection is null;
            bool localTransaction = Transaction is null;

            try
            {
                if (Connection is null)
                {
                    Connection = new SqlConnection(ConnectionString);
                    await Connection.OpenAsync();
                }
                else
                {
                    if (Connection.State == ConnectionState.Closed)
                    {
                        Connection.ConnectionString = ConnectionString;
                        await Connection.OpenAsync();
                    }
                }

                if (localTransaction)
                {
                    if (Transaction != null)
                    {
                        throw new Exception(
                            "Local transaction = true and Transacation obj not null"
                        );
                    }
                    else
                    {
                        Transaction = await Connection.BeginTransactionAsync();
                    }
                }

                using (
                    var bulkCopy = new SqlBulkCopy(
                        (SqlConnection)Connection,
                        SqlBulkCopyOptions.Default,
                        (SqlTransaction)Transaction
                    )
                )
                {
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.BatchSize = batchSize;

                    if (columnMappings is not null)
                    {
                        foreach (var mapping in columnMappings)
                        {
                            bulkCopy.ColumnMappings.Add(mapping.Key, mapping.Value);
                        }
                    }

                    await bulkCopy.WriteToServerAsync(dataTable);
                }

                response = true;
            }
            catch (Exception ex)
            {
                if (localTransaction && Transaction is not null)
                {
                    await Transaction.RollbackAsync();
                }

                _logger.LogError(ex, ex.Message);
                throw new DataProvider6GenericException(ex.Message);
            }
            finally
            {
                if (localTransaction)
                {
                    if (Transaction is not null)
                    {
                        await Transaction.CommitAsync();
                        await Transaction.DisposeAsync();
                        Transaction = null;
                    }
                }
                if (localConnection)
                {
                    if (Connection.State != ConnectionState.Closed)
                        await Connection.CloseAsync();
                    await Connection.DisposeAsync();
                    Connection = null;
                }
            }
            return response;
        }

        /// <summary>
        /// Esegue un update massivo asincrono di dati in una tabella del database
        /// </summary>
        /// <param name="dataTable">DataTable contenente i dati da inserire.</param>
        /// <param name="tableName">Nome della tabella di destinazione nel database.</param>
        /// <param name="keyColumn">Nome della colonna utilizzata per identificare il record da aggiornare.</param>
        /// <param name="columnMappings">Mappature opzionali tra i nomi delle colonne del DataTable (source) e quelli della tabella di destinazione. Se null, si assume che i nomi delle colonne corrispondano.</param>
        /// <param name="batchSize">Dimensione del batch per l'aggiornamento dei dati.</param>
        /// <returns>Restituisce true se l'aggiornamento è stato completato con successo, altrimenti false.</returns>
        public async Task<bool> ExecuteBulkUpdateAsync(
            DataTable dataTable,
            string tableName,
            string keyColumn,
            Dictionary<string, string> columnMappings = null,
            string? additionalFilter = null,
            int batchSize = 1000
        )
        {
            bool response = false;

            bool localConnection = Connection is null;
            bool localTransaction = Transaction is null;

            try
            {
                if (Connection is null)
                {
                    Connection = new SqlConnection(ConnectionString);
                    await Connection.OpenAsync();
                }
                else if (Connection.State == ConnectionState.Closed)
                {
                    Connection.ConnectionString = ConnectionString;
                    await Connection.OpenAsync();
                }

                if (localTransaction)
                {
                    if (Transaction != null)
                    {
                        throw new Exception(
                            "Local transaction = true and Transacation obj not null"
                        );
                    }

                    Transaction = await Connection.BeginTransactionAsync();
                }

                string tempTableName = "#TmpBulkUpdate";

                string createTempSql = BuildCreateTempTableSql(tempTableName, dataTable);

                await using (
                    var cmd = new SqlCommand(
                        createTempSql,
                        (SqlConnection)Connection,
                        (SqlTransaction)Transaction
                    )
                )
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                using (
                    var bulk = new SqlBulkCopy(
                        (SqlConnection)Connection,
                        SqlBulkCopyOptions.Default,
                        (SqlTransaction)Transaction
                    )
                )
                {
                    bulk.DestinationTableName = tempTableName;
                    bulk.BulkCopyTimeout = 600;
                    bulk.BatchSize = batchSize;

                    if (columnMappings is not null)
                    {
                        foreach (var map in columnMappings)
                        {
                            bulk.ColumnMappings.Add(map.Key, map.Value);
                        }
                    }
                    else
                    {
                        foreach (DataColumn col in dataTable.Columns)
                        {
                            bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }
                    }

                    await bulk.WriteToServerAsync(dataTable);
                }

                await using (
                    var cmd = new SqlCommand(
                        "select count(*) as count from #TmpBulkUpdate",
                        (SqlConnection)Connection,
                        (SqlTransaction)Transaction
                    )
                )
                {
                    var res = await cmd.ExecuteScalarAsync();
                    var count = res.ToString();
                }
                string mergeSql = BuildMergeSql(
                    tableName,
                    tempTableName,
                    dataTable,
                    keyColumn,
                    columnMappings,
                    additionalFilter
                );

                await using (
                    var cmd = new SqlCommand(
                        mergeSql,
                        (SqlConnection)Connection,
                        (SqlTransaction)Transaction
                    )
                )
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                response = true;
            }
            catch (Exception ex)
            {
                if (localTransaction && Transaction is not null)
                {
                    await Transaction.RollbackAsync();
                }

                _logger.LogError(ex, ex.Message);
                throw new DataProvider6GenericException(ex.Message);
            }
            finally
            {
                if (localTransaction)
                {
                    if (Transaction is not null)
                    {
                        await Transaction.CommitAsync();
                        await Transaction.DisposeAsync();
                        Transaction = null;
                    }
                }
                if (localConnection)
                {
                    if (Connection.State != ConnectionState.Closed)
                        await Connection.CloseAsync();
                    await Connection.DisposeAsync();
                    Connection = null;
                }
            }
            return response;
        }

        private async Task PerformChecksAndOpenConnectionAsync(Dictionary<string, object> parameters, Dictionary<string, Dictionary<Type, List<object>>>? clause_in_params)
        {
            //Lavez - 04/12/2024 - Controllo preliminare numero massimo parametri
            if (!CheckMaximumParameterNumber(parameters, clause_in_params))
                throw new Exception(
                    string.Format(
                        "Superato numero massimo di parametri applicabili ({0}). Operazione interrotta.",
                        MAXIMUM_PARAMETERS_NUMBER
                    )
                );
            //Lavez - 18/03/2025 - controllo chiavi simili
            if (!CheckSimilarKey(clause_in_params))
                throw new Exception(
                    string.Format(
                        "Parametri di clausola IN simili. Query non eseguibile, riverificare i nomi delle chiavi. Operazione interrotta."
                    )
                );

            await OpenConnectionAsync();
        }

        private async Task OpenConnectionAsync()
        {
            if (Connection == null)
            {
                Connection = new SqlConnection(ConnectionString);
                await Connection.OpenAsync();
            }
            else
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.ConnectionString = ConnectionString;
                    await Connection.OpenAsync();
                }
            }
        }

        private async Task PrepareCommandForReadingAsync(string SqlString, Dictionary<string, object> parameters, Dictionary<string, Dictionary<Type, List<object>>>? clause_in_params, SqlCommand? command, SqlDataAdapter adapter, Dictionary<int, Type> filter_key_list, StringBuilder sb)
        {
            command.CommandTimeout = 600; //TODO Parametrize
            if (Debugger.IsAttached)
                sb.AppendLine("--- INIZIO QUERY ---");

            //Lavez -  04/12/2024 - clausola in
            if (clause_in_params != null)
            {
                SqlString = await SetQuery_ClauseINAsync(
                    command,
                    SqlString,
                    parameters,
                    clause_in_params,
                    filter_key_list,
                    sb
                );
            }

            if (ApplyReadUncommited(SqlString))
                SqlString = " SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; \n" + SqlString;

            command.CommandText = SqlString;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(new SqlParameter(param.Key, param.Value));
                }
                if (Debugger.IsAttached)
                {
                    foreach (var param in parameters)
                    {
                        try
                        {
                            printParameter(param.Key, param.Value, sb);
                        }
                        catch (Exception ex)
                        {
                            //won't print the parameter
                        }
                    }
                }
            }

            adapter.SelectCommand = command;
            if (Transaction != null)
            {
                adapter.SelectCommand.Transaction = (SqlTransaction)Transaction;
                //command.Transaction = (SqlTransaction)Transaction;
            }

            if (Debugger.IsAttached)
            {
                sb.AppendLine(command.CommandText + Environment.NewLine);
            }
        }

        private async Task CleanInParametersAsync(Dictionary<string, Dictionary<Type, List<object>>>? clause_in_params, Dictionary<int, Type> filter_key_list, StringBuilder sb)
        {
            //Lavez - 20/01/2025 - pulizia tabella filtro
            if (clause_in_params != null)
            {
                await Clear_Tables_FilterAsync(filter_key_list, sb);
            }

            if (Debugger.IsAttached)
            {
                sb.AppendLine("--- FINE QUERY ---");
                Debug.WriteLine(sb.ToString());
            }
        }

        private async Task CloseConnectionAndDisposeEverythingAsync(SqlCommand? command, SqlDataAdapter adapter, bool LocalConnection)
        {
            if (command != null)
                await command.DisposeAsync();
            if (adapter != null)
                adapter.Dispose();
            if (LocalConnection)
            {
                if (Connection.State != ConnectionState.Closed)
                    await Connection.CloseAsync();
                await Connection.DisposeAsync();
                Connection = null;
            }
        }


        private int GetNewIDTableFilter(string tableName)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable seqId = new DataTable();
            SqlCommand? command = null;
            var ID = -1;
            try
            {
                //0 - Recupero ID per tabella

                command = ((SqlConnection)Connection).CreateCommand();
                command.CommandTimeout = 600; //TODO Parametrize

                var sqlString =
                    @$"IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE [NAME]=@SEQ)
                                BEGIN
                                      create sequence {"Sequence_" + tableName} start with 1 increment by 1
                                END

                                Select next value for {"Sequence_" + tableName}";

                if (ApplyReadUncommited(sqlString))
                    sqlString = " SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; \n" + sqlString;
                command.CommandText = sqlString;

                command.Parameters.Add(new SqlParameter("@SEQ", "Sequence_" + tableName));

                adapter.SelectCommand = command;
                adapter.SelectCommand.Transaction = (SqlTransaction)Connection.BeginTransaction();
                adapter.Fill(seqId);

                if (seqId.Rows.Count <= 0)
                    throw new Exception(
                        String.Format(
                            "Nessun ID per la sequenza {0} recuperato",
                            "Sequence_" + tableName
                        )
                    );
                else
                    ID = int.Parse(seqId.Rows[0][0].ToString());

                adapter.SelectCommand.Transaction.Commit();
                //adapter.SelectCommand.Transaction.Dispose();
            }
            catch (Exception ex)
            {
                if (adapter.SelectCommand.Transaction != null)
                {
                    adapter.SelectCommand.Transaction.Rollback();
                    adapter.SelectCommand.Transaction.Dispose();
                }
            }
            finally
            {
                if (command != null)
                    command.Dispose();
                if (adapter != null)
                    adapter.Dispose();
                if (seqId != null)
                    seqId.Dispose();
            }
            return ID;
        }

        private async Task<int> GetNewIDTableFilterAsync(string tableName)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable seqId = new DataTable();
            SqlCommand? command = null;
            var ID = -1;
            try
            {
                //0 - Recupero ID per tabella

                command = ((SqlConnection)Connection).CreateCommand();
                command.CommandTimeout = 600; //TODO Parametrize

                var sqlString =
                    @$"IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE [NAME]=@SEQ)
                                BEGIN
                                      create sequence {"Sequence_" + tableName} start with 1 increment by 1
                                END

                                Select next value for {"Sequence_" + tableName}";

                if (ApplyReadUncommited(sqlString))
                    sqlString = " SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; \n" + sqlString;
                command.CommandText = sqlString;

                command.Parameters.Add(new SqlParameter("@SEQ", "Sequence_" + tableName));

                adapter.SelectCommand = command;
                adapter.SelectCommand.Transaction = (SqlTransaction)Connection.BeginTransaction();
                adapter.Fill(seqId);

                if (seqId.Rows.Count <= 0)
                    throw new Exception(
                        String.Format(
                            "Nessun ID per la sequenza {0} recuperato",
                            "Sequence_" + tableName
                        )
                    );
                else
                    ID = int.Parse(seqId.Rows[0][0].ToString());

                adapter.SelectCommand.Transaction.Commit();
                //adapter.SelectCommand.Transaction.Dispose();
            }
            catch (Exception ex)
            {
                if (adapter.SelectCommand.Transaction != null)
                {
                    await adapter.SelectCommand.Transaction.RollbackAsync();
                    await adapter.SelectCommand.Transaction.DisposeAsync();
                }
            }
            finally
            {
                if (command != null)
                    await command.DisposeAsync();
                if (adapter != null)
                    adapter.Dispose();
                if (seqId != null)
                    seqId.Dispose();
            }
            return ID;
        }

        private void Clear_Tables_Filter(Dictionary<int, Type> filter_key_list)
        {
            foreach (var key in filter_key_list.Values.Distinct().ToList())
            {
                Internal_DeleteFilter(
                    (key == typeof(string) ? 1 : 2),
                    filter_key_list
                        .Where(x => x.Value.Equals(key))
                        .ToDictionary(x => x.Key, x => x.Value)
                        .Keys.ToList()
                );
            }
        }

        private async Task Clear_Tables_FilterAsync(
            Dictionary<int, Type> filter_key_list,
            StringBuilder sb
        )
        {
            foreach (var key in filter_key_list.Values.Distinct().ToList())
            {
                await Internal_DeleteFilterAsync(
                    (key == typeof(string) ? 1 : 2),
                    filter_key_list
                        .Where(x => x.Value.Equals(key))
                        .ToDictionary(x => x.Key, x => x.Value)
                        .Keys.ToList(),
                    sb
                );
            }
        }

        private bool Internal_DeleteFilter(int tab_type, List<int> ids)
        {
            var retVal = false;
            SqlCommand? command = null;

            bool LocalConnection = (Connection == null ? true : false);
            bool LocalTransaction = true; //In questo caso la transazione è sempre dedicata

            var tab_filter = "";
            try
            {
                if (tab_type == 1)
                    tab_filter = "_TMP_FILTRO_STR";
                else
                    tab_filter = "_TMP_FILTRO_INT";

                var sqlString = @$"DELETE FROM {tab_filter} WITH (ROWLOCK) where ID in (@pID)";

                if (Connection == null)
                {
                    Connection = new SqlConnection(ConnectionString);
                    Connection.Open();
                }
                else
                {
                    if (Connection.State == ConnectionState.Closed)
                    {
                        Connection.ConnectionString = ConnectionString;
                        Connection.Open();
                    }
                }
                if (LocalTransaction)
                {
                    if (Transaction != null)
                    {
                        throw new Exception(
                            "Local transaction = true and Transacation obj not null"
                        );
                    }
                    else
                    {
                        Transaction = Connection.BeginTransaction();
                    }
                }

                command = ((SqlConnection)Connection).CreateCommand();
                command.Transaction = (SqlTransaction)Transaction!;

                command.CommandTimeout = 600; // TODO PARAMETRIZE

                var param_string = "";
                var idx = 1;
                foreach (var id in ids)
                {
                    var idx_str = $"@px{idx}";
                    param_string += $"{idx_str},";
                    command.Parameters.Add(new SqlParameter(idx_str, id));
                    idx++;
                }
                param_string = param_string.Substring(0, param_string.Length - 1);

                sqlString = sqlString.Replace("@pID", param_string);

                command.CommandText = sqlString;

                command.ExecuteNonQuery();
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                if (LocalTransaction)
                {
                    if (Transaction != null)
                        Transaction.Rollback();
                }
                // TODO LOGGER
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }
            finally
            {
                if (command != null)
                    command.Dispose();
                if (LocalTransaction)
                {
                    if (Transaction != null)
                    {
                        Transaction.Commit();
                        Transaction.Dispose();
                        Transaction = null;
                    }
                }
                if (LocalConnection)
                {
                    if (Connection.State != ConnectionState.Closed)
                        Connection.Close();
                    Connection.Dispose();
                    Connection = null;
                }
            }
            return retVal;
        }

        private async Task<bool> Internal_DeleteFilterAsync(
            int tab_type,
            List<int> ids,
            StringBuilder sb
        )
        {
            var retVal = false;
            SqlCommand? command = null;

            bool LocalConnection = (Connection == null ? true : false);
            bool LocalTransaction = true; //In questo caso la transazione è sempre dedicata

            var tab_filter = "";
            try
            {
                if (tab_type == 1)
                    tab_filter = "_TMP_FILTRO_STR";
                else
                    tab_filter = "_TMP_FILTRO_INT";

                var sqlString = @$"DELETE FROM {tab_filter} WITH (ROWLOCK) where ID in (@pID)";

                if (Connection == null)
                {
                    Connection = new SqlConnection(ConnectionString);
                    await Connection.OpenAsync();
                }
                else
                {
                    if (Connection.State == ConnectionState.Closed)
                    {
                        Connection.ConnectionString = ConnectionString;
                        await Connection.OpenAsync();
                    }
                }
                if (LocalTransaction)
                {
                    if (Transaction != null)
                    {
                        throw new Exception(
                            "Local transaction = true and Transacation obj not null"
                        );
                    }
                    else
                    {
                        Transaction = await Connection.BeginTransactionAsync();
                    }
                }

                command = ((SqlConnection)Connection).CreateCommand();
                command.Transaction = (SqlTransaction)Transaction!;

                command.CommandTimeout = 600; // TODO PARAMETRIZE

                var param_string = "";
                var idx = 1;
                foreach (var id in ids)
                {
                    var idx_str = $"@p{(tab_type == 1 ? "_str_" : "_int_")}{idx}";
                    param_string += $"{idx_str},";
                    command.Parameters.Add(new SqlParameter(idx_str, id));
                    if (Debugger.IsAttached)
                    {
                        sb.AppendLine($"DECLARE {idx_str} AS INT;");
                        sb.AppendLine($"SET {idx_str} = {id.ToString()} {Environment.NewLine}");
                    }
                    idx++;
                }
                param_string = param_string.Substring(0, param_string.Length - 1);

                sqlString = sqlString.Replace("@pID", param_string);

                command.CommandText = sqlString;

                if (Debugger.IsAttached)
                {
                    sb.AppendLine($"{sqlString} {Environment.NewLine}");
                }

                await command.ExecuteNonQueryAsync();
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                if (LocalTransaction)
                {
                    if (Transaction != null)
                        await Transaction.RollbackAsync();
                }
                // TODO LOGGER
                _logger.LogError(ex, "");
                throw new DataProvider6GenericException(ex.Message);
            }
            finally
            {
                if (command != null)
                    await command.DisposeAsync();
                if (LocalTransaction)
                {
                    if (Transaction != null)
                    {
                        await Transaction.CommitAsync();
                        await Transaction.DisposeAsync();
                        Transaction = null;
                    }
                }
                if (LocalConnection)
                {
                    if (Connection.State != ConnectionState.Closed)
                        await Connection.CloseAsync();
                    await Connection.DisposeAsync();
                    Connection = null;
                }
            }
            return retVal;
        }

        #endregion

        private bool Translate(ref string sqlString, int languageCode)
        {
            string originalSqlString = sqlString;

            if (languageCode >= 1)
            {
                try
                {
                    var ISOCode = "it";

                    switch (languageCode)
                    {
                        case 2:
                        {
                            ISOCode = "en";
                            break;
                        }

                        case 3:
                        {
                            ISOCode = "fr";
                            break;
                        }

                        case 4:
                        {
                            ISOCode = "IT-ch";
                            break;
                        }

                        case 5:
                        {
                            ISOCode = "pt";
                            break;
                        }
                    }

                    if (ISOCode != "it")
                    {
                        Dictionary<string, string> tables_x_translation =
                            GetOriginalTables_X_TranslationTables(ISOCode);

                        var w_key = "";
                        var w_value = "";
                        foreach (var item in tables_x_translation.OrderBy(x => x.Key).ToList())
                        {
                            w_key = item.Key;
                            w_value = item.Value;
                            // " tabella " --> " tabella_XLingue_xx "
                            sqlString = sqlString.Replace(
                                " " + w_key + " ",
                                " " + w_value + " ",
                                StringComparison.InvariantCultureIgnoreCase
                            );
                            // " tabella\r\n" --> " tabella_XLingue_xx\r\n"
                            sqlString = sqlString.Replace(
                                " " + w_key + "\r\n",
                                " " + w_value + "\r\n",
                                StringComparison.InvariantCultureIgnoreCase
                            );
                            // " dbo.tabella " --> " tabella_XLingue_xx "
                            sqlString = sqlString.Replace(
                                " dbo." + w_key + " ",
                                " " + w_value + " ",
                                StringComparison.InvariantCultureIgnoreCase
                            );
                            // " tabella." --> " tabella_XLingue_xx."
                            sqlString = sqlString.Replace(
                                " " + w_key + ".",
                                " " + w_value + ".",
                                StringComparison.InvariantCultureIgnoreCase
                            );
                            // "tabella." --> "tabella_XLingue_xx."
                            sqlString = sqlString.Replace(
                                "" + w_key + ".",
                                "" + w_value + ".",
                                StringComparison.InvariantCultureIgnoreCase
                            );
                            // " dbo.tabella." --> " tabella_XLingue_xx."
                            sqlString = sqlString.Replace(
                                " dbo." + w_key + ".",
                                " " + w_value + ".",
                                StringComparison.InvariantCultureIgnoreCase
                            );
                            // "(tabella." --> "(tabella_XLingue_xx."
                            sqlString = sqlString.Replace(
                                "(" + w_key + ".",
                                "(" + w_value + ".",
                                StringComparison.InvariantCultureIgnoreCase
                            );
                            // " tabella," --> " tabella_XLingue_xx,"
                            sqlString = sqlString.Replace(
                                " " + w_key + ",",
                                " " + w_value + ",",
                                StringComparison.InvariantCultureIgnoreCase
                            );
                            // "(tabella," --> "(tabella_XLingue_xx,"
                            sqlString = sqlString.Replace(
                                "(" + w_key + ",",
                                "(" + w_value + ",",
                                StringComparison.InvariantCultureIgnoreCase
                            );
                            // ",tabella " --> ",tabella_XLingue_xx "
                            sqlString = sqlString.Replace(
                                "," + w_key + " ",
                                "," + w_value + " ",
                                StringComparison.InvariantCultureIgnoreCase
                            );
                            // ",tabella," --> ",tabella_XLingue_xx,"
                            sqlString = sqlString.Replace(
                                "," + w_key + ",",
                                "," + w_value + ",",
                                StringComparison.InvariantCultureIgnoreCase
                            );
                            // ",tabella." --> ",tabella_XLingue_xx."
                            sqlString = sqlString.Replace(
                                "," + w_key + ".",
                                "," + w_value + ".",
                                StringComparison.InvariantCultureIgnoreCase
                            );
                            // "*tabella." --> "*tabella_XLingue_xx."
                            sqlString = sqlString.Replace(
                                "*" + w_key + ".",
                                "*" + w_value + ".",
                                StringComparison.InvariantCultureIgnoreCase
                            );
                            // "=tabella." --> "=tabella_XLingue_xx."
                            sqlString = sqlString.Replace(
                                "=" + w_key + ".",
                                "=" + w_value + ".",
                                StringComparison.InvariantCultureIgnoreCase
                            );
                        }

                        // Queste vengono fatte per risolvere il caso in cui ci fossero nomi di tabella scritti fra []
                        sqlString = sqlString.Replace("[[", "[");
                        sqlString = sqlString.Replace("]]", "]");

                        return true;
                    }
                    else
                        return false;
                }
                catch (Exception)
                {
                    //reset to orginal
                    sqlString = originalSqlString;
                    return false;
                }
            }
            return false;
        }

        protected Dictionary<string, string> GetOriginalTables_X_TranslationTables(string CodiceISO)
        {
            Dictionary<string, string> dic_originalTables_x_translationTable = new Dictionary<string, string>()
            {
                { "Analisi_Tipi", "[Analisi_Tipi_XLingue_" + CodiceISO + "]" },
                { "Avversita", "[Avversita_XLingue_" + CodiceISO + "]" },
                { "CategorieMagazzino", "[CategorieMagazzino_XLingue_" + CodiceISO + "]" },
                { "ClassificazioniFormulati", "[ClassificazioniFormulati_XLingue_" + CodiceISO + "]"},
                { "Cultivar", "[Cultivar_XLingue_" + CodiceISO + "]" },
                { "Epoche", "[Epoche_XLingue_" + CodiceISO + "]" },
                { "Fabbricati_Tipi", "[Fabbricati_Tipi_XLingue_" + CodiceISO + "]" },
                { "FormeAllevamento", "[FormeAllevamento_XLingue_" + CodiceISO + "]" },
                { "GruppoAvversita", "[GruppoAvversita_XLingue_" + CodiceISO + "]" },
                { "GruppoFinalita", "[GruppoFinalita_XLingue_" + CodiceISO + "]" },
                { "GruppoOperazioni", "[GruppoOperazioni_XLingua_" + CodiceISO + "]" },
                { "GruppoVarietale", "[GruppoVarietale_XLingue_" + CodiceISO + "]" },
                { "GruppoVegetale", "[GruppoVegetale_XLingue_" + CodiceISO + "]" },
                { "ImpiantiIrrigazioni", "[ImpiantiIrrigazioni_XLingue_" + CodiceISO + "]" },
                { "Macchine", "[Macchine_XLingue_" + CodiceISO + "]" },
                { "Materie_Prime", "[Materie_Prime_XLingue_" + CodiceISO + "]" },
                { "MenuBS_2017_Sezioni", "[MenuBS_2017_Sezioni_XLingua_" + CodiceISO + "]" },
                { "Note_Intervento", "[Note_Intervento_XLingue_" + CodiceISO + "]" },
                { "Note_Intervento_Gruppi", "[Note_Intervento_Gruppi_XLingue_" + CodiceISO + "]" },
                { "Note_Intervento_Utilizzo", "[Note_Intervento_Utilizzo_XLingue_" + CodiceISO + "]" },
                { "Operazioni", "[Operazioni_XLingue_" + CodiceISO + "]" },
                { "Portinnesti", "[Portinnesti_XLingue_" + CodiceISO + "]" },
                { "Prenotazione_Piante_Categoria", "[Prenotazione_Piante_Categoria_XLingue_" + CodiceISO + "]" },
                { "Prenotazione_Piante_Certificazione", "[Prenotazione_Piante_Certificazione_XLingue_" + CodiceISO + "]" },
                { "Rapporti_Contabili", "[Rapporti_Contabili_XLingue_" + CodiceISO + "]" },
                { "SpecieVegetali", "[SpecieVegetali_XLingue_" + CodiceISO + "]" },
                { "SpecieVegetaliXStadiCrescita", "[StadiXSpecieVegetali_XLingua_" + CodiceISO + "]" },
                { "StampeReport", "[StampeReport_XLingue_" + CodiceISO + "]" },
                { "Tipologie", "[Tipologie_XLingue_" + CodiceISO + "]" },
                { "TipologieSementi", "[TipologieSementi_XLingue_" + CodiceISO + "]" },
                { "UnitaMisura", "[UnitaMisura_XLingue_" + CodiceISO + "]" },
                { "Codici_Anagrafe", "[Codici_Anagrafe_XLingue_" + CodiceISO + "]" },
                { "Modalita_Applicazione_Globali", "[Modalita_Applicazione_Globali_XLingue_" + CodiceISO + "]" },
                { "Modalita_Applicazione", "[Modalita_Applicazione_XLingue_" + CodiceISO + "]" },
                { "Lista_Categorie_Animali", "[Lista_Categorie_Animali_XLingue_" + CodiceISO + "]" },
                { "Lista_Causali_Morte", "[Lista_Causali_Morte_XLingue_" + CodiceISO + "]" },
                { "Lista_Generi_Animali", "[Lista_Generi_Animali_XLingue_" + CodiceISO + "]" },
                { "Lista_IndirizziProd_Animali", "[Lista_IndirizziProd_Animali_XLingue_" + CodiceISO + "]" },
                { "Lista_Patologie", "[Lista_Patologie_XLingue_" + CodiceISO + "]" },
                { "Lista_Razze_Animali", "[Lista_Razze_Animali_XLingue_" + CodiceISO + "]" },
                { "Lista_Specie_Animali", "[Lista_Specie_Animali_XLingue_" + CodiceISO + "]" },
                { "Lista_Tipi_Raggruppamento_Stalla", "[Lista_Tipi_Raggruppamento_Stalla_XLingue_" + CodiceISO + "]" },
                { "CodiciErroreVerificaConformita", "[CodiciErroreVerificaConformita_XLingue_" + CodiceISO + "]" },
                { "Guida_Impostazioni", "[Guida_Impostazioni_XLingue_" + CodiceISO + "]" },
                { "Guida_Impostazioni_Sezioni", "[Guida_Impostazioni_Sezioni_XLingue_" + CodiceISO + "]" },
                { "Guida_Impostazioni_Valori", "[Guida_Impostazioni_Valori_XLingue_" + CodiceISO + "]" },
            };

            return dic_originalTables_x_translationTable;
        }

        private string BuildCreateTempTableSql(string tempTableName, DataTable dt)
        {
            var sb = new StringBuilder();
            sb.Append($"CREATE TABLE {tempTableName} (");

            foreach (DataColumn col in dt.Columns)
            {
                sb.Append($"[{col.ColumnName}] {GetSqlType(col.DataType)},");
            }

            sb.Length--; // remove last comma
            sb.Append(");");

            return sb.ToString();
        }

        private string BuildMergeSql(
            string targetTable,
            string tempTable,
            DataTable dt,
            string keyColumn,
            Dictionary<string, string>? mappings,
            string? additionalFilter = null
        )
        {
            // Determine actual column names
            Dictionary<string, string> map;
            if (mappings is not null)
            {
                map = mappings;
            }
            else
            {
                map = dt
                    .Columns.Cast<DataColumn>()
                    .ToDictionary(c => c.ColumnName, c => c.ColumnName);
            }

            // Columns to update (excluding key)
            var updateAssignments = map.Where(m => m.Value != keyColumn)
                .Select(m => $"t.[{m.Value}] = s.[{m.Key}]");

            // Additional filter (optional)
            string filterClause = string.IsNullOrWhiteSpace(additionalFilter)
                ? ""
                : $" AND ({additionalFilter})";

            // Build the SQL
            return $@"
                MERGE INTO {targetTable} AS t
                USING {tempTable} AS s
                ON t.[{keyColumn}] = s.[{keyColumn}]
                WHEN MATCHED{filterClause} THEN
                    UPDATE SET
                        {string.Join(",\n", updateAssignments)};
                ";
        }

        private static string GetSqlType(Type type)
        {
            if (type == typeof(int))
                return "INT";
            if (type == typeof(long))
                return "BIGINT";
            if (type == typeof(bool))
                return "BIT";
            if (type == typeof(DateTime))
                return "DATETIME";
            if (type == typeof(decimal))
                return "DECIMAL(18,4)";
            if (type == typeof(double))
                return "FLOAT";
            if (type == typeof(string))
                return "NVARCHAR(MAX)";

            throw new Exception($"Unsupported type: {type}");
        }
    }
}
