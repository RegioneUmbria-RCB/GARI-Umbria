using AgronicaCoreDTOStd.InData;
using AgronicaCoreModelsSTD.DataExchange.AntaresTrace;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OutData.FoodMetaverse
{
    /// <summary>
    /// Risposta dell'API <c>POST /v1/sostenibilita-co2/crea-token-blockchain</c>.
    /// Proxa la risposta ricevuta dal servizio Blockchain M5.
    /// Riferimento spec: DS10-BL CreaTokenBlockchain — Output.
    /// </summary>
    public class SubmitBlockchainResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public SubmitBlockchainResponseData data { get; set; }
    }

    public class SubmitBlockchainResponseData
    {
        public string acknowledgmentId { get; set; }
        public string submissionId { get; set; }
        public int pendingNftMints { get; set; }
        public DateTime receivedAt { get; set; }
        public string statusEndpoint { get; set; }
    }

    /// <summary>
    /// Risposta dell'API <c>POST /v1/sostenibilita-co2/crea-token-blockchain</c>.
    /// Proxa la risposta ricevuta dal servizio Blockchain M5.
    /// Riferimento spec: DS10-BL CreaTokenBlockchain — Output.
    /// </summary>
    public class SubmitBlockchainResponseError
    {
        public int statusCode { get; set; }
        public DateTime timestamp { get; set; }
        public string path { get; set; }
        public string method { get; set; }
        public string status { get; set; }
        public SubmitBlockchainResponseErrorMessage error { get; set; }
    }

    public class SubmitBlockchainResponseErrorMessage
    {
        public string code { get; set; }
        public string message { get; set; }
        public SubmitBlockchainResponseErrorMessageDetails details { get; set; }
    }

    public class SubmitBlockchainResponseErrorMessageDetails
    {
        public string field { get; set; }
        public string reason { get; set; }
    }
}
