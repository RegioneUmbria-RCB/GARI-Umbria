using System;

namespace AgronicaCoreModelsSTD.Utility
{
    public interface IApiResponse
    {
        string message { get; set; }
        string errore { get; set; }
    }

    public class Api_Response : IApiResponse
    {
        public string message { get; set; }
        public string errore { get; set; }
        public object dati { get; set; }
        public Api_Response()
        {
            message = "";
            errore = "";
            dati = null;
        }
    }

    public class GenericApiResponse<T> : IApiResponse
    {
        public string message { get; set; } = "";
        public string errore { get; set; } = "";
        public T dati { get; set; }
    }

    public class Api_Response_Message_Type
    {
        public const String Ok = "OK";
    }

}
