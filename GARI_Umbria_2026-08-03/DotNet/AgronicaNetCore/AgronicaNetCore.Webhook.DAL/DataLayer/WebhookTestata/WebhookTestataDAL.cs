using System.Data;
using System.Dynamic;
using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Webhook.DAL.DataLayer.WebhookTestata
{
    public class WebhookTestataDAL : DAL_Base, IWebhookTestataDAL
    {

        public WebhookTestataDAL(IServiceProvider provider) : base(provider)
        {
        }

        public async Task<bool> InsertTestataAsync(int idTestata,short tipo, string requestId,int raccoglitoreCod, string payload, AgronicaCoreParametriServer objParametriServer)
        {

            const string sql = @"
                INSERT INTO WebHook_Testata
                    (Id_Testata, Tipo, Request_Id, Status,Raccoglitore_Cod, Payload,
                     Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica,
                     Validita_Inizio, Validita_Fine)
                VALUES
                    (@idTestata, @tipo, @requestId, 'TODO', @raccoglitoreCod, @payload,
                     GETDATE(), GETDATE(), @usernameCreazione, @usernameCreazione,
                     @validitaInizio, @validitaFine);";

            var expando = new ExpandoObject();
            expando.TryAdd("@idTestata",         idTestata);
            expando.TryAdd("@tipo",              tipo);
            expando.TryAdd("@requestId",         requestId);
            expando.TryAdd("@raccoglitoreCod",   raccoglitoreCod);
            expando.TryAdd("@payload",           payload ?? string.Empty);
            expando.TryAdd("@usernameCreazione", objParametriServer.UtenteUsername);
            expando.TryAdd("@validitaInizio",    CostantiPersonalizzate.AGRODATAINIZIO_DATE);
            expando.TryAdd("@validitaFine",      CostantiPersonalizzate.AGRODATAFINE_DATE);

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(sql, expando);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> UpdateTestataAsync(int idTestata, string status, string response, AgronicaCoreParametriServer objParametriServer)
        {
            const string sql = @"
                UPDATE WebHook_Testata
                SET    Status        = @status,
                       Response      = @response,
                       Data_Modifica = GETDATE(),
                       Username_Modifica = @usernameModifica
                WHERE  Id_Testata = @idTestata;";

            var expando = new ExpandoObject();
            expando.TryAdd("@idTestata",        idTestata);
            expando.TryAdd("@status",           status);
            expando.TryAdd("@response",         response);
            expando.TryAdd("@usernameModifica", objParametriServer.UtenteUsername);

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(sql, expando);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable?> GetWebHookTestataByRequestIdAsync(
            string requestId,
            int testataId,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(requestId))
                throw new ArgumentException("requestId obbligatorio.", nameof(requestId));

            if (testataId <= 0)
                throw new ArgumentException("testataId obbligatorio.", nameof(testataId));

            // DS15-BL: in caso di RequestId duplicati restituisce il record più recente per Data_Creazione.
            // IsolationLevel ReadUncommitted per non bloccare le scritture dell'engine in parallelo.
            const string sql = @"
                SELECT TOP 1
                    Id_Testata,
                    Request_Id,
                    Status,
                    Response
                FROM WebHook_Testata WITH (NOLOCK)
                WHERE Request_Id = @requestId
                  AND Id_Testata = @testataId
                ORDER BY Data_Creazione DESC;";

            var parameters = new Dictionary<string, object>
            {
                ["@requestId"] = requestId,
                ["@testataId"] = testataId
            };

            try
            {
                var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, parameters);
                return dt == null || dt.Rows.Count == 0 ? null : dt;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
