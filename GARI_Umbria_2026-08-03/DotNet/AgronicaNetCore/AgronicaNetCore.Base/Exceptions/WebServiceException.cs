namespace AgronicaNetCore.Base.Exceptions
{
    public class WebServiceException : Exception
    {
        public int? StatusCode { get; }

        public WebServiceException(string message) : base(message) { }

        public WebServiceException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public WebServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
