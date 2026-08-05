using System.Data;
using System.Text;
using InData.Agenda;
using System.Dynamic;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Anagrafe.DAL.Base;
using static AgronicaNetCore.Base.Models.AgronicaCoreParametri;
using System.Transactions;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.RicettexAgenda
{
    public class RicettexAgenda : DAL_Base, IRicettexAgenda
    {
        public RicettexAgenda(IServiceProvider provider, bool securityBypass = false) : base(provider, securityBypass)
        {
        }

        private WriteRicettexAgenda Valorizza(WriteRicettexAgenda dtoRxA)
        {
            DateTime adInizio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
            DateTime adFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);

            dtoRxA.Ricetta_Operazione_Cod ??= 0;
            dtoRxA.Inviato ??= 0;
            dtoRxA.DataLock ??= 0;

            if (dtoRxA.Validita_Inizio < adInizio)
                dtoRxA.Validita_Inizio = adInizio;
            if (dtoRxA.Validita_Fine > adFine)
                dtoRxA.Validita_Fine = adFine;

            return dtoRxA;
        }

        public async Task<DataTable> ReadAsync(int Ricetta_Cod, int Id_Agenda, AgronicaCoreParametriServer objParametriServer, int RicettaOp_Cod = 0)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            stbQuery.AppendLine("SELECT * FROM RicettexAgenda ")
                .AppendLine("WHERE 1=1 ");

            sqlParams.TryAdd("@superUser", objParametriServer.PivaSuperUser);
            stbQuery.AppendLine("    AND Ricetta_SuperUser = @superUser ");

            if (Ricetta_Cod != 0)
            {
                sqlParams.TryAdd("@ricettaCod", Ricetta_Cod);
                stbQuery.AppendLine("    AND Ricetta_Cod = @ricettaCod ");
            }
            if (RicettaOp_Cod != 0)
            {
                sqlParams.TryAdd("@ricOpCod", RicettaOp_Cod);
                stbQuery.AppendLine("    AND Ricetta_Operazione_Cod = @ricOpCod ");
            }
            if (Id_Agenda != 0)
            {
                sqlParams.TryAdd("@idAgenda", Id_Agenda);
                stbQuery.AppendLine("    AND Id_Agenda = @idAgenda ");
            }
            sqlParams.TryAdd("@inizio", objParametriServer.FinestraTemporaleFine);
            sqlParams.TryAdd("@fine", objParametriServer.FinestraTemporaleInizio);
            stbQuery.AppendLine("    AND Validita_Inizio <= @inizio ")
                .AppendLine("    AND Validita_Fine >= @fine ");

            stbQuery.AppendLine("ORDER BY Ricetta_SuperUser, Ricetta_Cod, Id_Agenda DESC ");

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> ExistAsync(int Ricetta_Cod, int Id_Agenda, AgronicaCoreParametriServer objParametriServer, int RicettaOp_Cod = 0)
        {
            if (Ricetta_Cod == 0)
                throw new Exception("Ricetta_Cod non valorizzato.");
            if (Id_Agenda == 0)
                throw new Exception("Id_Agenda non valorizzato.");

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@superUser", objParametriServer.PivaSuperUser);
            sqlParams.TryAdd("@ricettaCod", Ricetta_Cod);
            sqlParams.TryAdd("@idAgenda", Id_Agenda);

            stbQuery.AppendLine("SELECT TOP(1) * FROM RicettexAgenda ")
                .AppendLine("WHERE Ricetta_SuperUser = @superUser AND Ricetta_Cod = @ricettaCod AND Id_Agenda = @idAgenda ");

            try
            {
                var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
                if (dt.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> CreateAsync(WriteRicettexAgenda dtoRxA, AgronicaCoreParametriServer objParametriServer)
        {
            dtoRxA = Valorizza(dtoRxA);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO RicettexAgenda (Ricetta_SuperUser, Ricetta_Cod, Ricetta_Operazione_Cod, Id_Agenda, ")
                .AppendLine("    inviato, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, DataLock) ");
            stbQuery.AppendLine("VALUES (@superUser, @ricettaCod, @ricOpCod, @idAgenda, ")
                .AppendLine("    @inviato, GETDATE(), GETDATE(), @userOp, @userOp, @inizio, @fine, @dtLock) ");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@superUser", objParametriServer.PivaSuperUser);
            expandoObj.TryAdd("@ricettaCod", dtoRxA.Ricetta_Cod);
            expandoObj.TryAdd("@ricOpCod", dtoRxA.Ricetta_Operazione_Cod);
            expandoObj.TryAdd("@idAgenda", dtoRxA.Id_Agenda);

            expandoObj.TryAdd("@inviato", dtoRxA.Inviato);
            //expandoObj.TryAdd("@dtInvio", dtoRxA.Data_Invio);
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@inizio", dtoRxA.Validita_Inizio);
            expandoObj.TryAdd("@fine", dtoRxA.Validita_Fine);
            expandoObj.TryAdd("@dtLock", dtoRxA.DataLock);

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

        public async Task<bool> UpdateAsync(WriteRicettexAgenda dtoRxA, AgronicaCoreParametriServer objParametriServer)
        {
            if (dtoRxA.Ricetta_Cod == 0)
                throw new Exception("Ricetta_Cod non valorizzato.");
            if (dtoRxA.Id_Agenda == 0)
                throw new Exception("Id_Agenda non valorizzato.");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@superUser", objParametriServer.PivaSuperUser);
            expandoObj.TryAdd("@ricettaCod", dtoRxA.Ricetta_Cod);
            expandoObj.TryAdd("@idAgenda", dtoRxA.Id_Agenda);
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);

            var excludedProperties = new HashSet<string>
            {
                "Ricetta_Cod",
                "Ricetta_Operazione_Cod",
                "Id_Agenda",
                "Ricetta_SuperUser",
                "inviato",
                "DataInvio",
                "Validita_Inizio",
                "Validita_Fine",
                "Username_Creazione",
                "Username_Modifica",
                "Data_Creazione",
                "Data_Modifica"
            };

            var setClauses = new List<string>();
            foreach (var property in typeof(WriteRicettexAgenda).GetProperties())
            {
                if (excludedProperties.Contains(property.Name)) continue;

                var value = property.GetValue(dtoRxA);
                if (value != null)
                {
                    string paramName = "@" + property.Name;
                    setClauses.Add($"{property.Name} = {paramName}");
                    expandoObj.TryAdd(paramName, value);
                }
            }
            if (setClauses.Count == 0) return false;

            string updateQuery =
                $@"UPDATE RicettexAgenda SET
                    {string.Join(", ", setClauses)}
                    , Username_Modifica = @userOp
                    , Data_Modifica = GETDATE()
                WHERE Ricetta_SuperUser = @superUser AND Ricetta_Cod = @ricettaCod AND Id_Agenda = @idAgenda ";

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

        public async Task<bool> DeleteAsync(int Ricetta_Cod, int Id_Agenda, AgronicaCoreParametriServer objParametriServer, int RicettaOp_Cod = 0)
        {
            if (Ricetta_Cod == 0)
                throw new Exception("Ricetta_Cod non valorizzato.");            
            if (Id_Agenda == 0)
                throw new Exception("Id_Agenda non valorizzato.");

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@superUser", objParametriServer.PivaSuperUser);
            expandoObj.TryAdd("@ricettaCod", Ricetta_Cod);
            expandoObj.TryAdd("@idAgenda", Id_Agenda);

            if (objParametriServer.FlagCancellazioneLogica == enumCancellazioneLogica.CancellazioneLogica)
            {
                stbQuery.AppendLine("UPDATE RicettexAgenda SET ")
                    .AppendLine("    Inviato = -1, ")
                    .AppendLine("    Username_Modifica = @userOp ")
                    .AppendLine("WHERE Inviato >= 0 ");
                expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            }
            else
            {
                stbQuery.AppendLine("DELETE FROM RicettexAgenda ")
                    .AppendLine("WHERE 1=1 ");
            }

            stbQuery.AppendLine("    AND Ricetta_SuperUser = @superUser ")
                .AppendLine("    AND Ricetta_Cod = @ricettaCod ")
                .AppendLine("    AND Id_Agenda = @idAgenda ");

            if (RicettaOp_Cod != 0)
            {
                stbQuery.AppendLine("    AND Ricetta_Operazione_Cod = @ricOpCod ");
                expandoObj.TryAdd("@ricOpCod", RicettaOp_Cod);
            }

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

        public async Task<bool> ScriviModificaAsync(WriteRicettexAgenda dtoRicxAg, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                bool isNew = !await ExistAsync(dtoRicxAg.Ricetta_Cod, dtoRicxAg.Id_Agenda, objParametriServer);

                if (isNew)
                    await CreateAsync(dtoRicxAg, objParametriServer);
                else
                    await UpdateAsync(dtoRicxAg, objParametriServer);

                return true;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(WriteRicettexAgenda dtoRicxAg, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await DeleteAsync(dtoRicxAg.Ricetta_Cod, dtoRicxAg.Id_Agenda, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(List<WriteRicettexAgenda> RicettexAgenda, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(objParametriServer.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var dto in RicettexAgenda)
                    {
                        await DeleteAsync(dto.Ricetta_Cod, dto.Id_Agenda, objParametriServer);
                    }
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    LogError(ex.Message, objParametriServer, ex);
                    throw;
                }
                finally
                {
                    if (objParametriServer.objTransazione == null)
                        ts.Dispose();
                }
            }

            return true;
        }
    }
}
