namespace AgronicaNetCore.Base.Exceptions
{
    public class WebServiceTimeoutException : Exception
    {
        public WebServiceTimeoutException(string message) : base(message) { }

        public WebServiceTimeoutException(string message, Exception innerException) : base(message, innerException) { }
    }
}
