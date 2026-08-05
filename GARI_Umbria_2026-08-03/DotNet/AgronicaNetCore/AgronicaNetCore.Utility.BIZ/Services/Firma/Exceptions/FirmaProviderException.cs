using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Utility.BIZ.Services.Firma.Exceptions
{
    public class FirmaProviderException : Exception
    {
        public string Provider { get; }
        public HttpStatusCode? StatusCode { get; }

        public FirmaProviderException(
            string provider,
            HttpStatusCode? statusCode,
            string message,
            Exception? inner = null)
            : base(message, inner)
        {
            Provider = provider;
            StatusCode = statusCode;
        }
    }
}
