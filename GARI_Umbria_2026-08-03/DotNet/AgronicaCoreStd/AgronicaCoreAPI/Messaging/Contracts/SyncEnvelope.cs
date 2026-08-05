using System;

namespace AgronicaCoreAPI.Messaging.Contracts
{
    /// <summary>
    /// Envelope che viaggia su RabbitMQ per ogni richiesta di sincronizzazione.
    /// Contiene metadati di tracciatura, auth info e il payload del dato da salvare.
    /// </summary>
    public class SyncEnvelope
    {
        /// <summary>UUID univoco del messaggio (usato come MessageId AMQP)</summary>
        public string MessageId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>ID di correlazione ritornato al client (202 Accepted)</summary>
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Routing key / tipo entità (es. "sync.requests.attivita")</summary>
        public string Type { get; set; }

        /// <summary>Credenziali e parametri necessari per chiamare CoreWS</summary>
        public AuthInfo Auth { get; set; }

        /// <summary>
        /// Dato da sincronizzare. Tipo object per evitare dipendenze da generics.
        /// Ogni handler deserializza esplicitamente nel tipo atteso.
        /// </summary>
        public object Payload { get; set; }

        /// <summary>Metadati di tracciatura</summary>
        public MetaInfo Meta { get; set; }
    }

    public class AuthInfo
    {
        public string Bearer { get; set; }
        public string User { get; set; }
        public string Username { get; set; }
        public string ObjP_super_server { get; set; }
        public string ObjP_server { get; set; }
        public string ObjP_utenti { get; set; }
        public string CoreWSBaseURL { get; set; }
        public string UserAgent { get; set; }
    }

    public class MetaInfo
    {
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public string AppVersion { get; set; }
        /// <summary>Identificativo cliente (pivaSuperUser) — usato per audit e log strutturato.</summary>
        public string ClienteId { get; set; }
    }
}
