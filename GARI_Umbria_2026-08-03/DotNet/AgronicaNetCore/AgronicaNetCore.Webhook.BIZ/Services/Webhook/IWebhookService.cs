using AgronicaNetCore.Base.Models;
using CloudNative.CloudEvents;

namespace AgronicaNetCore.Webhook.BIZ.Services.Webhook
{
    public interface IWebhookService
    {
        Task<bool> ProcessWebhookAsync(int idTestata, CloudEvent cloudEvent, AgronicaCoreParametriServer objParametriServer);
        Task<bool> CreateTestRowAsync(short tipo, string requestId, AgronicaCoreParametriServer objParametriServer);
    }
}
