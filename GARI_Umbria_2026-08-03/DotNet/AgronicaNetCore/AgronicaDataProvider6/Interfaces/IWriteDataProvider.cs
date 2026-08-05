using System.Data;
using System.Dynamic;

namespace AgronicaDataProvider6.Interfaces
{
    public interface IWriteDataProvider
    {
        string Get_DataBase_Name_From_ConnectionString(string connectionString);

        string Get_Instance_Name_From_ConnectionString(string connectionString);

        Task<bool> Execute_WriteAsync(string sqlString, ExpandoObject parameters);
        Task<bool> Execute_WriteAsync(string sqlString);

        /// <summary>
        /// Esegue una query di scrittura con parametri di tipo VarBinary (byte[]).
        /// </summary>
        /// <param name="sqlString">La query SQL da eseguire.</param>
        /// <param name="cmdParameters">Dizionario di parametri VarBinary (nome parametro → valore byte[]).</param>
        /// <returns>True se l'esecuzione ha avuto successo.</returns>
        Task<bool> Execute_WriteVarBinaryAsync(string sqlString, Dictionary<string, byte[]> cmdParameters, Dictionary<string, object>? parameters = null);

        /// <summary>
        /// Esegue un inserimento massivo asincrono di dati in una tabella del database
        /// </summary>
        /// <param name="dataTable">DataTable contenente i dati da inserire.</param>
        /// <param name="tableName">Nome della tabella di destinazione nel database.</param>
        /// <param name="columnMappings">Mappature opzionali tra i nomi delle colonne del DataTable (source) e quelli della tabella di destinazione. Se null, si assume che i nomi delle colonne corrispondano.</param>
        /// <param name="batchSize">Dimensione del batch per l'inserimento dei dati.</param>
        /// <returns>Restituisce true se l'inserimento è stato completato con successo, altrimenti false.</returns>
        Task<bool> ExecuteBulkInsertAsync(DataTable dataTable, string tableName, Dictionary<string, string> columnMappings = null, int batchSize = 1000);

        /// <summary>
        /// Esegue un update massivo asincrono di dati in una tabella del database
        /// </summary>
        /// <param name="dataTable">DataTable contenente i dati da inserire.</param>
        /// <param name="tableName">Nome della tabella di destinazione nel database.</param>
        /// <param name="keyColumn">Nome della colonna utilizzata per identificare il record da aggiornare.</param>
        /// <param name="columnMappings">Mappature opzionali tra i nomi delle colonne del DataTable (source) e quelli della tabella di destinazione. Se null, si assume che i nomi delle colonne corrispondano.</param>
        /// <param name="batchSize">Dimensione del batch per l'aggiornamento dei dati.</param>
        /// <returns>Restituisce true se l'aggiornamento è stato completato con successo, altrimenti false.</returns>
        Task<bool> ExecuteBulkUpdateAsync(DataTable dataTable, string tableName, string keyColumn, Dictionary<string, string> columnMappings = null, string? additionalFilter = null, int batchSize = 1000);

    }
}
