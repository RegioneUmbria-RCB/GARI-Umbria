using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ConsiglioNutrizione.Models;
using CloudNative.CloudEvents;

namespace AgronicaNetCore.Webhook.DAL.DataLayer.Models
{
    public static class WebhookRouting
    {

        #region EsitoConsiglioNutrizione

        private static readonly Dictionary<string, CloudEventAttribute> EsitoConsiglioExtensionAttributes = new()
        {
            ["requestid"] = CloudEventAttribute.CreateExtension("requestid", CloudEventAttributeType.String),
            ["dssmodeltype"] = CloudEventAttribute.CreateExtension("dssmodeltype", CloudEventAttributeType.String),
            ["tenant"] = CloudEventAttribute.CreateExtension("tenant", CloudEventAttributeType.String),
        };

        #endregion

        public const string GenericSaveHandler = "GenericSave";

        public static readonly Dictionary<string, WebhookTipo> TypeMapping = new()
        {
            ["dss.engine.ExecutionResult"] = WebhookTipo.EsitoConsiglioNutrizione,
        };

        public static readonly Dictionary<WebhookTipo, string> HandlerMapping = new()
        {
            [WebhookTipo.EsitoConsiglioNutrizione] = GenericSaveHandler,
        };

        public static readonly Dictionary<WebhookTipo, Type> DataTypeMapping = new()
        {
            [WebhookTipo.EsitoConsiglioNutrizione] = typeof(EsitoConsiglioNutrizione),
        };

        public static readonly Dictionary<WebhookTipo, Dictionary<string, CloudEventAttribute>> ExtensionAttributesMapping = new()
        {
            [WebhookTipo.EsitoConsiglioNutrizione] = EsitoConsiglioExtensionAttributes,
        };
    }
}
