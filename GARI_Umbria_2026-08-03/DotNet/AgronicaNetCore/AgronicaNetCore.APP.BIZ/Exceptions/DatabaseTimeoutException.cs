namespace AgronicaNetCore.APP.BIZ.Exceptions
{
    public class DatabaseTimeoutException : Exception
    {
        public DatabaseTimeoutException(string message) : base(message) { }

        public DatabaseTimeoutException(string message, Exception innerException) : base(message, innerException) { }
    }
}
