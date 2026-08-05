using AgronicaCoreAPI.Messaging.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AgronicaCoreAPI.Messaging.Handlers
{
    /// <summary>Classe base per handler: fornisce il metodo di deserializzazione robusta del payload.</summary>
    public abstract class SyncMessageHandlerBase
    {
        protected static T DeserializePayload<T>(SyncEnvelope envelope, ILogger logger)
        {
            try
            {
                if (envelope.Payload is T typed)
                    return typed;

                if (envelope.Payload is JObject jObject)
                    return jObject.ToObject<T>();

                if (envelope.Payload is string jsonString)
                    return JsonConvert.DeserializeObject<T>(jsonString);

                // Re-serializzazione come fallback finale
                var json = JsonConvert.SerializeObject(envelope.Payload);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Errore deserializzazione payload tipo={Type}", typeof(T).Name);
                throw;
            }
        }
    }
}
