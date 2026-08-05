using System.Text;
using InData.Agenda;
using System.Dynamic;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Constants;
using static AgronicaNetCore.Base.Models.AgronicaCoreParametri;
using System.Data;
using System.Transactions;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Operazione.DAL.Resources;
using AgronicaDataProvider6.Extensions;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Destinazioni
{
    public class Mov_Destinazioni : BaseDALOperazione, IMov_Destinazioni
    {
        public Mov_Destinazioni(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, localizer, securityBypass)
        {
        }

        private DateTime SetIfNotInAgroDateRange(DateTime? toCheck, DateTime elseValue)
        {
            return toCheck != null && ((DateTime)toCheck).IsInRange(
                    CostantiPersonalizzate.AGRODATAINIZIO_DATE, CostantiPersonalizzate.AGRODATAFINE_DATE)
                ? (DateTime)toCheck
                : elseValue;
        }
        private WriteMovDestinazioni Valorizza(WriteMovDestinazioni dtoMovDestinazioni)
        {
            DateTime adInizio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
            DateTime adFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);

            dtoMovDestinazioni.Sa_Cod ??= 0;
            dtoMovDestinazioni.Appezza ??= 0;
            dtoMovDestinazioni.Tipo_Destinazione ??= 0;
            dtoMovDestinazioni.Qta ??= 0;
            dtoMovDestinazioni.Qta2 ??= 0;
            dtoMovDestinazioni.Tipo_Scorta ??= 0;
            dtoMovDestinazioni.Scorta_Min ??= 0;
            dtoMovDestinazioni.Mov_Destinazioni_GraphicKey ??= "";
            dtoMovDestinazioni.Qta_Dest1 ??= 0;
            dtoMovDestinazioni.Qta_Dest2 ??= 0;
            dtoMovDestinazioni.Sup_Riduzione_BufferZone ??= 0;
            dtoMovDestinazioni.Perc_Riduzione_Deriva ??= 0;
            dtoMovDestinazioni.QuotaDistribuzione ??= 0;
            dtoMovDestinazioni.Sa_Cod_Riferimento ??= 0;
            dtoMovDestinazioni.Id_Destinazione_Riferimento ??= 0;
            dtoMovDestinazioni.Tipo_Destinazione_Riferimento ??= 0;
            dtoMovDestinazioni.Extra_Str ??= "";
            
            dtoMovDestinazioni.Inviato ??= 0;

            dtoMovDestinazioni.Validita_Inizio = SetIfNotInAgroDateRange(dtoMovDestinazioni.Validita_Inizio, adInizio);
            dtoMovDestinazioni.Validita_Fine = SetIfNotInAgroDateRange(dtoMovDestinazioni.Validita_Fine, adFine);

            return dtoMovDestinazioni;
        }

        public async Task<DataTable> ReadAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Dest, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT * FROM Mov_Destinazioni ")
                .AppendLine("WHERE 1=1 ");

            if (!string.IsNullOrEmpty(Piva))
            {
                sqlParams.TryAdd("@piva", Piva);
                stbQuery.AppendLine("    AND PIVA = @piva ");
            }
            if (Id_Agenda != 0)
            {
                sqlParams.TryAdd("@idAgenda", Id_Agenda);
                stbQuery.AppendLine("    AND Id_Agenda = @idAgenda ");
            }
            if (Id_Mov != 0)
            {
                sqlParams.TryAdd("@idMov", Id_Mov);
                stbQuery.AppendLine("    AND Id_Mov = @idMov ");
            }
            if (Id_Mov_Det != 0)
            {
                sqlParams.TryAdd("@idMovDet", Id_Mov_Det);
                stbQuery.AppendLine("    AND Id_Mov_Det = @idMovDet ");
            }
            if (Id_Dest != 0)
            {
                sqlParams.TryAdd("@idDest", Id_Dest);
                stbQuery.AppendLine("    AND Id_Destinazione = @idDest ");
            }
            sqlParams.TryAdd("@inizio", objParametriServer.FinestraTemporaleFine);
            sqlParams.TryAdd("@fine", objParametriServer.FinestraTemporaleInizio);
            stbQuery.AppendLine("    AND Validita_Inizio <= @inizio ")
                .AppendLine("    AND Validita_Fine >= @fine ");

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            stbQuery.AppendLine("ORDER BY Id_Agenda, Id_Mov, Id_Mov_Det, Id_Destinazione DESC ");

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

        public async Task<bool> ExistAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Destinazione, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrEmpty(Piva))
                throw new Exception("Piva non valorizzata.");
            if (Id_Agenda == 0)
                throw new Exception("Id_Agenda non valorizzato.");
            if (Id_Mov == 0)
                throw new Exception("Id_Mov non valorizzato.");
            if (Id_Mov_Det == 0)
                throw new Exception("Id_Mov_Det non valorizzato.");
            if (Id_Destinazione == 0)
                throw new Exception("Id_Destinazione non valorizzato.");

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@piva", Piva);
            sqlParams.TryAdd("@idAgenda", Id_Agenda);
            sqlParams.TryAdd("@idMov", Id_Mov);
            sqlParams.TryAdd("@idMovDet", Id_Mov_Det);
            sqlParams.TryAdd("@idDest", Id_Destinazione);

            stbQuery.AppendLine("SELECT TOP(1) * FROM Mov_Destinazioni ")
                .AppendLine("WHERE 1=1 ")
                .AppendLine("    AND Piva = @piva ")
                .AppendLine("    AND Id_Agenda = @idAgenda ")
                .AppendLine("    AND Id_Mov = @idMov ")
                .AppendLine("    AND Id_Mov_Det = @idMovDet ")
                .AppendLine("    AND Id_Destinazione = @idDest ");

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

        public async Task<bool> CreateAsync(WriteMovDestinazioni dtoMovDestinazioni, AgronicaCoreParametriServer objParametriServer)
        {
            dtoMovDestinazioni = Valorizza(dtoMovDestinazioni);

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            HashSet<(string col, string pName, object? value)> sqlFields = new();

            sqlFields.Add(("PIVA", "@piva", dtoMovDestinazioni.Piva));
            sqlFields.Add(("Sa_Cod", "@saCod", dtoMovDestinazioni.Sa_Cod));
            sqlFields.Add(("Id_Agenda", "@idAgenda", dtoMovDestinazioni.Id_Agenda));
            sqlFields.Add(("Id_Mov", "@idMov", dtoMovDestinazioni.Id_Mov));
            sqlFields.Add(("Id_Mov_Det", "@idMovDet", dtoMovDestinazioni.Id_Mov_Det));
            sqlFields.Add(("Id_Destinazione", "@idDest", dtoMovDestinazioni.Id_Destinazione));
            sqlFields.Add(("Appezza", "@appezza", dtoMovDestinazioni.Appezza));
            sqlFields.Add(("Tipo_Destinazione", "@tipoDest", dtoMovDestinazioni.Tipo_Destinazione));
            sqlFields.Add(("Qta", "@qta", dtoMovDestinazioni.Qta));
            sqlFields.Add(("Qta2", "@qta2", dtoMovDestinazioni.Qta2));
            sqlFields.Add(("Tipo_Scorta", "@tipoScorta", dtoMovDestinazioni.Tipo_Scorta));
            sqlFields.Add(("Scorta_Min", "@scortaMin", dtoMovDestinazioni.Scorta_Min));
            sqlFields.Add(("mov_destinazioni_graphickey", "@movDestGK", dtoMovDestinazioni.Mov_Destinazioni_GraphicKey));
            sqlFields.Add(("Qta_Dest1", "@qtaDest1", dtoMovDestinazioni.Qta_Dest1));
            sqlFields.Add(("Qta_Dest2", "@qtaDest2", dtoMovDestinazioni.Qta_Dest2));
            sqlFields.Add(("Sup_Riduzione_BufferZone", "@supRidBZ", dtoMovDestinazioni.Sup_Riduzione_BufferZone));
            sqlFields.Add(("Perc_Riduzione_Deriva", "@percRidD", dtoMovDestinazioni.Perc_Riduzione_Deriva));
            sqlFields.Add(("QuotaDistribuzione", "@quotaDistr", dtoMovDestinazioni.QuotaDistribuzione));
            sqlFields.Add(("Sa_Cod_Riferimento", "@saCodRif", dtoMovDestinazioni.Sa_Cod_Riferimento));
            sqlFields.Add(("Id_Destinazione_Riferimento", "@idDestRif", dtoMovDestinazioni.Id_Destinazione_Riferimento));
            sqlFields.Add(("Tipo_Destinazione_Riferimento", "@tipoDestRif", dtoMovDestinazioni.Tipo_Destinazione_Riferimento));
            sqlFields.Add(("Extra_Str", "@extraStr", dtoMovDestinazioni.Extra_Str));
            sqlFields.Add(("inviato", "@inviato", dtoMovDestinazioni.Inviato));
            sqlFields.Add(("datainvio", "@dtInvio", dtoMovDestinazioni.Data_Invio));
            sqlFields.Add(("Data_Modifica", "@dataModifica", CostantiPersonalizzate.AGRODATAINIZIO_DATE));
            sqlFields.Add(("Username_Creazione", "@userOp", objParametriServer.UsernameOperazione));
            sqlFields.Add(("Username_Modifica", "@userOp", objParametriServer.UsernameOperazione));
            sqlFields.Add(("Validita_Inizio", "@inizio", dtoMovDestinazioni.Validita_Inizio));
            sqlFields.Add(("Validita_Fine", "@fine", dtoMovDestinazioni.Validita_Fine));

            sqlFields = sqlFields.Where(x => x.value != null).ToHashSet();

            stbQuery.AppendLine("INSERT INTO Mov_Destinazioni ( ");
            sqlFields.Select((x, i) => i == sqlFields.Count - 1 ? x.col : x.col + ",")
                .Chunk(5)
                .Select(cols => cols.Aggregate((a, b) => a + " " + b))
                .ToList().ForEach(cols => stbQuery.AppendLine(cols));
            stbQuery.AppendLine(" , Data_Creazione ");
            stbQuery.AppendLine(") ");
            stbQuery.AppendLine("VALUES ( ");
            sqlFields.Select((x, i) => i == sqlFields.Count - 1 ? x.pName : x.pName + ",")
                .Chunk(5)
                .Select(cols => cols.Aggregate((a, b) => a + " " + b))
                .ToList().ForEach(cols => stbQuery.AppendLine(cols));
            stbQuery.AppendLine(" , GETDATE() ");
            stbQuery.AppendLine(") ");

            sqlFields.ToList().ForEach(x => expandoObj.TryAdd(x.pName, x.value));

            //stbQuery.AppendLine("INSERT INTO Mov_Destinazioni (PIVA, Sa_Cod, Id_Agenda, Id_Mov, Id_Mov_Det, Id_Destinazione, Appezza, Tipo_Destinazione, Qta, ")
            //    .AppendLine("    Qta2, Tipo_Scorta, Scorta_Min, mov_destinazioni_graphickey, Qta_Dest1, Qta_Dest2, Sup_Riduzione_BufferZone, Perc_Riduzione_Deriva, ")
            //    .Append("    QuotaDistribuzione, Sa_Cod_Riferimento, Id_Destinazione_Riferimento, Tipo_Destinazione_Riferimento, Extra_Str, ")
            //    .AppendLine("    inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ");
            //stbQuery.AppendLine("VALUES (@piva, @saCod, @idAgenda, @idMov, @idMovDet, @idDest, @appezza, @tipoDest, @qta, ")
            //    .AppendLine("    @qta2, @tipoScorta, @scortaMin, @movDestGK, @qtaDest1, @qtaDest2, @supRidBZ, @percRidD, ")
            //    .Append("    @quotaDistr, @saCodRif, @idDestRif, @tipoDestRif, @extraStr, ")
            //    .AppendLine("    @inviato, @dtInvio, GETDATE(), GETDATE(), @userOp, @userOp, @inizio, @fine) ");

            //var expandoObj = new ExpandoObject();
            //expandoObj.TryAdd("@piva", dtoMovDestinazioni.Piva);
            //expandoObj.TryAdd("@saCod", dtoMovDestinazioni.Sa_Cod);
            //expandoObj.TryAdd("@idAgenda", dtoMovDestinazioni.Id_Agenda);
            //expandoObj.TryAdd("@idMov", dtoMovDestinazioni.Id_Mov);
            //expandoObj.TryAdd("@idMovDet", dtoMovDestinazioni.Id_Mov_Det);
            //expandoObj.TryAdd("@idDest", dtoMovDestinazioni.Id_Destinazione);
            //expandoObj.TryAdd("@appezza", dtoMovDestinazioni.Appezza);
            //expandoObj.TryAdd("@tipoDest", dtoMovDestinazioni.Tipo_Destinazione);
            //expandoObj.TryAdd("@qta", dtoMovDestinazioni.Qta);

            //expandoObj.TryAdd("@qta2", dtoMovDestinazioni.Qta2);
            //expandoObj.TryAdd("@tipoScorta", dtoMovDestinazioni.Tipo_Scorta);
            //expandoObj.TryAdd("@scortaMin", dtoMovDestinazioni.Scorta_Min);
            //expandoObj.TryAdd("@movDestGK", dtoMovDestinazioni.Mov_Destinazioni_GraphicKey);
            //expandoObj.TryAdd("@qtaDest1", dtoMovDestinazioni.Qta_Dest1);
            //expandoObj.TryAdd("@qtaDest2", dtoMovDestinazioni.Qta_Dest2);
            //expandoObj.TryAdd("@supRidBZ", dtoMovDestinazioni.Sup_Riduzione_BufferZone);
            //expandoObj.TryAdd("@percRidD", dtoMovDestinazioni.Perc_Riduzione_Deriva);
            //expandoObj.TryAdd("@quotaDistr", dtoMovDestinazioni.QuotaDistribuzione);
            //expandoObj.TryAdd("@saCodRif", dtoMovDestinazioni.Sa_Cod_Riferimento);
            //expandoObj.TryAdd("@idDestRif", dtoMovDestinazioni.Id_Destinazione_Riferimento);
            //expandoObj.TryAdd("@tipoDestRif", dtoMovDestinazioni.Tipo_Destinazione_Riferimento);
            //expandoObj.TryAdd("@extraStr", dtoMovDestinazioni.Extra_Str);

            //expandoObj.TryAdd("@inviato", dtoMovDestinazioni.Inviato);
            //expandoObj.TryAdd("@dtInvio", dtoMovDestinazioni.Data_Invio);
            //expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            //expandoObj.TryAdd("@inizio", dtoMovDestinazioni.Validita_Inizio);
            //expandoObj.TryAdd("@fine", dtoMovDestinazioni.Validita_Fine);

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

        public async Task<bool> UpdateAsync(WriteMovDestinazioni dtoMovDestinazioni, AgronicaCoreParametriServer objParametriServer)
        {
            if (dtoMovDestinazioni.Id_Destinazione == 0)
                throw new Exception("Id_Destinazione non valorizzato.");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", dtoMovDestinazioni.Piva);
            expandoObj.TryAdd("@idAgenda", dtoMovDestinazioni.Id_Agenda);
            expandoObj.TryAdd("@idMov", dtoMovDestinazioni.Id_Mov);
            expandoObj.TryAdd("@idMovDet", dtoMovDestinazioni.Id_Mov_Det);
            expandoObj.TryAdd("@idDest", dtoMovDestinazioni.Id_Destinazione);
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);

            var excludedProperties = new HashSet<string>
            {
                "Piva",
                "Sa_Cod",
                "Id_Agenda",
                "Id_Mov",
                "Id_Mov_Det",
                "Id_Destinazione",
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
            foreach (var property in typeof(WriteMovDestinazioni).GetProperties())
            {
                if (excludedProperties.Contains(property.Name)) continue;

                var value = property.GetValue(dtoMovDestinazioni);
                if (value != null)
                {
                    string paramName = "@" + property.Name;
                    setClauses.Add($"{property.Name} = {paramName}");
                    expandoObj.TryAdd(paramName, value);
                }
            }
            if (setClauses.Count == 0) return false;

            string updateQuery =
                $@"UPDATE Mov_Destinazioni SET
                    {string.Join(", ", setClauses)}
                    , Username_Modifica = @userOp
                    , Data_Modifica = GETDATE()
                WHERE Piva = @piva AND Id_Agenda = @idAgenda AND Id_Mov = @idMov AND Id_Mov_Det = @idMovDet AND Id_Destinazione = @idDest ";

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

        public async Task<bool> DeleteAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Dest, AgronicaCoreParametriServer objParametriServer)
        {
            if (Id_Dest == 0)
                throw new Exception("Id_Destinazione non valorizzato.");

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", Piva);
            expandoObj.TryAdd("@idAgenda", Id_Agenda);
            expandoObj.TryAdd("@idMov", Id_Mov);
            expandoObj.TryAdd("@idMovDet", Id_Mov_Det);
            expandoObj.TryAdd("@idDest", Id_Dest);

            if (objParametriServer.FlagCancellazioneLogica == enumCancellazioneLogica.CancellazioneLogica)
            {
                stbQuery.AppendLine("UPDATE Mov_Destinazioni SET ")
                    .AppendLine("    Inviato = -1, ")
                    .AppendLine("    Username_Modifica = @userOp ")
                    .AppendLine("WHERE Inviato >= 0 ");
                expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            }
            else
            {
                stbQuery.AppendLine("DELETE FROM Mov_Destinazioni ")
                    .AppendLine("WHERE 1=1 ");
            }

            stbQuery.AppendLine("    AND Piva = @piva ")
                .AppendLine("    AND Id_Agenda = @idAgenda ")
                .AppendLine("    AND Id_Mov = @idMov ")
                .AppendLine("    AND Id_Mov_Det = @idMovDet ")
                .AppendLine("    AND Id_Destinazione = @idDest ");

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

        public async Task<int> ScriviModificaAsync(WriteMovDestinazioni dtoMovDest, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                bool isNew =
                    !await ExistAsync(dtoMovDest.Piva, dtoMovDest.Id_Agenda, dtoMovDest.Id_Mov, dtoMovDest.Id_Mov_Det, dtoMovDest.Id_Destinazione, objParametriServer);

                if (isNew)
                    await CreateAsync(dtoMovDest, objParametriServer);
                else
                    await UpdateAsync(dtoMovDest, objParametriServer);

                return dtoMovDest.Id_Destinazione;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(WriteMovDestinazioni dtoMovDest, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await DeleteAsync(dtoMovDest.Piva, dtoMovDest.Id_Agenda, dtoMovDest.Id_Mov, dtoMovDest.Id_Mov_Det, dtoMovDest.Id_Destinazione, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(List<WriteMovDestinazioni> MovDestinazioni, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(objParametriServer.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var dto in MovDestinazioni)
                        await DeleteAsync(dto.Piva, dto.Id_Agenda, dto.Id_Mov, dto.Id_Mov_Det, dto.Id_Destinazione, objParametriServer);

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
