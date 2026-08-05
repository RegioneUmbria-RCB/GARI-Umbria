using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Operazione.DAL.Resources;
using InData.Agenda;
using Microsoft.Extensions.Localization;
using System.Dynamic;
using System.Text;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Agronica_Log_Agenda
{
    public class Agronica_Log_Agenda : BaseDALOperazione, IAgronica_Log_Agenda
    {
        public Agronica_Log_Agenda(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider,localizer, securityBypass)
        {
        }

        private WriteLogAgenda Valorizza(WriteLogAgenda dtoLogAgenda)
        {
            dtoLogAgenda.Sa_Cod ??= 0;
            dtoLogAgenda.Object_Data ??= "";
            dtoLogAgenda.Origine ??= -1;
            dtoLogAgenda.Raccoglitore_Cod ??= 0;

            return dtoLogAgenda;
        }

        public async Task<bool> CreateAsync(WriteLogAgenda dtoLogAgenda, AgronicaCoreParametriServer objParametriServer)
        {
            dtoLogAgenda = Valorizza(dtoLogAgenda);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Agronica_Log_Agenda (Piva, Sa_Cod, Id_Agenda, Lav_Cod, Tipo_Operazione, Des_Lib, ")
                .AppendLine("    SuperUser, Utente, Id_Servizio, Data_Ora_Lavorazione, Data_Ora_RegistrazioneLog, object_data, Origine, Raccoglitore_Cod) ");
            stbQuery.AppendLine("VALUES (@piva, @saCod, @idAgenda, @lavCod, @tipoOp, @desLib, ")
                .AppendLine("    @superUser, @user, @idServ, @dataLav, @dataLog, @objData, @orig, @raccCod) ");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", dtoLogAgenda.Piva);
            expandoObj.TryAdd("@saCod", dtoLogAgenda.Sa_Cod);
            expandoObj.TryAdd("@idAgenda", dtoLogAgenda.Id_Agenda);
            expandoObj.TryAdd("@lavCod", dtoLogAgenda.Lav_Cod);
            expandoObj.TryAdd("@tipoOp", dtoLogAgenda.Tipo_Operazione);
            expandoObj.TryAdd("@desLib", dtoLogAgenda.Des_Lib);
            expandoObj.TryAdd("@superUser", dtoLogAgenda.SuperUser);
            expandoObj.TryAdd("@user", dtoLogAgenda.Utente);
            expandoObj.TryAdd("@idServ", dtoLogAgenda.Id_Servizio);
            expandoObj.TryAdd("@dataLav", dtoLogAgenda.Data_Ora_Lavorazione);
            expandoObj.TryAdd("@dataLog", dtoLogAgenda.Data_Ora_RegistrazioneLog);
            expandoObj.TryAdd("@objData", dtoLogAgenda.Object_Data);
            expandoObj.TryAdd("@orig", dtoLogAgenda.Origine);
            expandoObj.TryAdd("@raccCod", dtoLogAgenda.Raccoglitore_Cod);

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(WriteLogAgenda dtoLogAgenda, AgronicaCoreParametriServer objParametriServer)
        {
            if (dtoLogAgenda.ID == 0)
                throw new Exception("ID non valorizzato.");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@id", dtoLogAgenda.ID);
            expandoObj.TryAdd("@piva", dtoLogAgenda.Piva);
            expandoObj.TryAdd("@saCod", dtoLogAgenda.Sa_Cod);
            expandoObj.TryAdd("@idAgenda", dtoLogAgenda.Id_Agenda);

            var propertyMappings = new Dictionary<string, string>
            {
                {"Lav_Cod", "@lavCod"},
                {"Tipo_Operazione", "@tipoOp"},
                {"Des_Lib", "@desLib"},
                {"SuperUser", "@superUser"},
                {"Utente", "@user"},
                {"Id_Servizio", "@idServ"},
                {"Data_Ora_Lavorazione", "@dataLav"},
                {"Data_Ora_RegistrazioneLog", "@dataLog"},
                {"object_data", "@objData"},
                {"Origine", "@orig"},
                {"Raccoglitore_Cod", "@raccCod"},
            };

            var excludedProperties = new HashSet<string>
            {
                "ID",
                "Id_Agenda",
                "Piva",
                "Sa_Cod",
            };

            var setClauses = new List<string>();
            foreach (var property in typeof(WriteLogAgenda).GetProperties())
            {
                if (excludedProperties.Contains(property.Name)) continue;

                var value = property.GetValue(dtoLogAgenda);
                if (value != null)
                {
                    string paramName = "@" + property.Name;
                    setClauses.Add($"{property.Name} = {paramName}");
                    expandoObj.TryAdd(paramName, value);
                }
            }
            if (setClauses.Count == 0) return false;

            string updateQuery =
                $@"UPDATE Agronica_Log_Agenda SET
                    {string.Join(", ", setClauses)}
                WHERE ID = @id ADN Piva = @piva AND Sa_Cod = @saCod AND Id_Agenda = @idAgenda ";

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(updateQuery, expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int Id, string Piva, int Id_Agenda, AgronicaCoreParametriServer objParametriServer)
        {
            if (Id == 0)
                throw new Exception("ID non valorizzato.");

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@id", Id);
            expandoObj.TryAdd("@piva", Piva);
            expandoObj.TryAdd("@idAgenda", Id_Agenda);

            stbQuery.AppendLine("DELETE FROM Agronica_Log_Agenda ")
                .AppendLine("WHERE 1=1 ")
                .AppendLine("    AND ID = @id ")
                .AppendLine("    AND Piva = @piva ")
                .AppendLine("    AND Id_Agenda = @idAgenda ");

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
