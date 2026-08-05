namespace AgronicaNetCore.APP.BIZ.Exceptions
{
    public class TableNotFoundException : Exception
    {
        public string? TableName { get; }

        public TableNotFoundException(string tableName)
            : base($"Tabella ''{tableName}'' non trovata nel database sorgente")
        {
            TableName = tableName;
        }

        public TableNotFoundException(string tableName, Exception innerException)
            : base($"Tabella ''{tableName}'' non trovata nel database sorgente", innerException)
        {
            TableName = tableName;
        }
    }
}
