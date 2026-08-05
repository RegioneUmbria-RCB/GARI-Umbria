using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Webhook.DAL.DataLayer.WebhookTestata
{
    public interface IWebhookTestataDAL
    {
        Task<bool> InsertTestataAsync(int idTestata, short tipo, string requestId,int raccoglitoreCod, string payload, AgronicaCoreParametriServer objParametriServer);
        Task<bool> UpdateTestataAsync(int idTestata, string status, string response, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> GetWebHookTestataByRequestIdAsync(
            string requestId,
            int testataId,
            AgronicaCoreParametriServer objParametriServer);
    }
}
