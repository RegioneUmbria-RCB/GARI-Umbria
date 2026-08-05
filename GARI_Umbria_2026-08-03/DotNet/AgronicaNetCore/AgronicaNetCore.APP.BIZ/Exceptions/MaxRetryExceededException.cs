namespace AgronicaNetCore.APP.BIZ.Exceptions
{
    public class MaxRetryExceededException : Exception
    {
        public MaxRetryExceededException(string message) : base(message) { }

        public MaxRetryExceededException(string message, Exception innerException) : base(message, innerException) { }
    }
}
