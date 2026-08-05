using System.Text;
using InData.Agenda;
using System.Dynamic;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Constants;
using static AgronicaNetCore.Base.Models.AgronicaCoreParametri;
using System.Data;
using System.Transactions;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Operazione.DAL.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Dettaglio_Tecnico
{
    public class Mov_Dettaglio_Tecnico : BaseDALOperazione, IMov_Dettaglio_Tecnico
    {
        private readonly IAgro_Sequence _sequenceDal;

    public Mov_Dettaglio_Tecnico(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider,localizer, securityBypass)
        {
            _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
        }

        private WriteMovDettTecnico Valorizza(WriteMovDettTecnico dtoMovDettTec)
        {
            DateTime adInizio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
            DateTime adFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);

            dtoMovDettTec.Sa_Cod ??= 0;
            dtoMovDettTec.Qta_Ril ??= 0;
            dtoMovDettTec.Data_Ril ??= adInizio;
            dtoMovDettTec.Ditta_Cod ??= 0;
            dtoMovDettTec.Dett_Cod ??= 0;
            dtoMovDettTec.Id_Insetto ??= 0;
            dtoMovDettTec.FF_Classe ??= 0;
            dtoMovDettTec.Dose ??= 0;
            dtoMovDettTec.Mg ??= 0;
            dtoMovDettTec.N ??= 0;
            dtoMovDettTec.K ??= 0;
            dtoMovDettTec.P ??= 0;
            dtoMovDettTec.Parziale ??= 0;
            dtoMovDettTec.Nitrati ??= 0;
            dtoMovDettTec.Freatimetro ??= 0;
            dtoMovDettTec.Piezo1 ??= 0;
            dtoMovDettTec.Piezo2 ??= 0;
            dtoMovDettTec.Piezo3 ??= 0;
            dtoMovDettTec.Piezo4 ??= 0;
            dtoMovDettTec.Sigla_AV ??= "";
            dtoMovDettTec.Trap_Num ??= 0;
            dtoMovDettTec.Inn1_Data ??= adInizio;
            dtoMovDettTec.Inn2_Data ??= adInizio;
            dtoMovDettTec.Inn3_Data ??= adInizio;
            dtoMovDettTec.Inn4_Data ??= adInizio;
            dtoMovDettTec.Av_Cod ??= 0;
            dtoMovDettTec.Av_Gru ??= 0;
            dtoMovDettTec.Lotto ??= "";
            dtoMovDettTec.Extra_Str ??= "";
            dtoMovDettTec.Extra_Date ??= adInizio;
            dtoMovDettTec.Extra_Int ??= 0;
            dtoMovDettTec.Soglia_Cod ??= 0;
            dtoMovDettTec.Soglia_Des ??= "";
            dtoMovDettTec.Soglia_Quantita ??= 0;
            dtoMovDettTec.Efficienza ??= 0;
            dtoMovDettTec.Cu ??= 0;

            dtoMovDettTec.Inviato ??= 0;

            dtoMovDettTec.Validita_Fine = adFine;

            if (dtoMovDettTec.Validita_Inizio < adInizio)
                dtoMovDettTec.Validita_Inizio = adInizio;


            return dtoMovDettTec;
        }

        public async Task<DataTable> ReadAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Reg_Det, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT * FROM Mov_Dettaglio_Tecnico ")
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
            if (Id_Reg_Det != 0)
            {
                sqlParams.TryAdd("@idRegDet", Id_Reg_Det);
                stbQuery.AppendLine("    AND Id_Reg_Dettaglio = @idRegDet ");
            }

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            sqlParams.TryAdd("@inizio", objParametriServer.FinestraTemporaleFine);
            sqlParams.TryAdd("@fine", objParametriServer.FinestraTemporaleInizio);
            stbQuery.AppendLine("    AND Validita_Inizio <= @inizio ")
                .AppendLine("    AND Validita_Fine >= @fine ");

            stbQuery.AppendLine("ORDER BY Id_Agenda, Id_Mov, Id_Mov_Det, Id_Reg_Dettaglio DESC ");

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

        public async Task<bool> ExistAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Reg_Det, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrEmpty(Piva))
                throw new Exception("Piva non valorizzata.");
            if (Id_Agenda == 0)
                throw new Exception("Id_Agenda non valorizzato.");
            if (Id_Mov == 0)
                throw new Exception("Id_Mov non valorizzato.");
            if (Id_Mov_Det == 0)
                throw new Exception("Id_Mov_Det non valorizzato.");
            if (Id_Reg_Det == 0)
                throw new Exception("Id_Reg_Dettaglio non valorizzato.");

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@piva", Piva);
            sqlParams.TryAdd("@idAgenda", Id_Agenda);
            sqlParams.TryAdd("@idMov", Id_Mov);
            sqlParams.TryAdd("@idMovDet", Id_Mov_Det);
            sqlParams.TryAdd("@idRegDet", Id_Reg_Det);

            stbQuery.AppendLine("SELECT TOP(1) * FROM Mov_Dettaglio_Tecnico ")
                .AppendLine("WHERE 1=1 ")
                .AppendLine("    AND Piva = @piva ")
                .AppendLine("    AND Id_Agenda = @idAgenda ")
                .AppendLine("    AND Id_Mov = @idMov ")
                .AppendLine("    AND Id_Mov_Det = @idMovDet ")
                .AppendLine("    AND Id_Reg_Dettaglio = @idRegDet ");

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

        public async Task<bool> CreateAsync(WriteMovDettTecnico dtoMovDettTec, AgronicaCoreParametriServer objParametriServer)
        {
            dtoMovDettTec = Valorizza(dtoMovDettTec);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Mov_Dettaglio_Tecnico (Piva, Sa_Cod, Id_Agenda, Id_Mov, Id_Mov_Det, Id_Reg_Dettaglio, ")
                .AppendLine("    Qta_Ril, Data_Ril, Ditta_Cod, Dett_Cod, Id_Insetto, FF_Classe, Dose, Mg, N, K, P, Parziale, Nitrati, Freatimetro, ")
                .AppendLine("    Piezo1, Piezo2, Piezo3, Piezo4, Sigla_AV, Trap_Num, Inn1_Data, Inn2_Data, Inn3_Data, Inn4_Data, Av_Cod, Av_Gru, ")
                .AppendLine("    Lotto, Extra_Str, Extra_Int, Extra_Date, Soglia_Cod, Soglia_Des, Soglia_Quantita, Efficienza, Cu, ")
                .AppendLine("    inviato, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ");
            stbQuery.AppendLine("VALUES (@piva, @saCod, @idAgenda, @idMov, @idMovDet, @idRegDet, ")
                .AppendLine("    @qtaRil, @dataRil, @dittaCod, @dettCod, @idIns, @ffClasse, @dose, @mg, @n, @k, @p, @parz, @nitr, @freat, ")
                .AppendLine("    @pie1, @pie2, @pie3, @pie4, @sigAv, @trapN, @inn1D, @inn2D, @inn3D, @inn4D, @avCod, @avGru, ")
                .AppendLine("    @lotto, @exStr, @exInt, @exDate, @sogliaCod, @sogliaDes, @sogliaQta, @effic, @cu, ")
                .AppendLine("    @inviato, GETDATE(), GETDATE(), @userOp, @userOp, @inizio, @fine) ");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", dtoMovDettTec.Piva);
            expandoObj.TryAdd("@saCod", dtoMovDettTec.Sa_Cod);
            expandoObj.TryAdd("@idAgenda", dtoMovDettTec.Id_Agenda);
            expandoObj.TryAdd("@idMov", dtoMovDettTec.Id_Mov);
            expandoObj.TryAdd("@idMovDet", dtoMovDettTec.Id_Mov_Det);
            expandoObj.TryAdd("@idRegDet", dtoMovDettTec.Id_Reg_Dettaglio);
            
            expandoObj.TryAdd("@qtaRil", dtoMovDettTec.Qta_Ril);
            expandoObj.TryAdd("@dataRil", dtoMovDettTec.Data_Ril);
            expandoObj.TryAdd("@dittaCod", dtoMovDettTec.Ditta_Cod);
            expandoObj.TryAdd("@dettCod", dtoMovDettTec.Dett_Cod);
            expandoObj.TryAdd("@idIns", dtoMovDettTec.Id_Insetto);
            expandoObj.TryAdd("@ffClasse", dtoMovDettTec.FF_Classe);
            expandoObj.TryAdd("@dose", dtoMovDettTec.Dose);
            expandoObj.TryAdd("@mg", dtoMovDettTec.Mg);
            expandoObj.TryAdd("@n", dtoMovDettTec.N);
            expandoObj.TryAdd("@k", dtoMovDettTec.K);
            expandoObj.TryAdd("@p", dtoMovDettTec.P);
            expandoObj.TryAdd("@parz", dtoMovDettTec.Parziale);
            expandoObj.TryAdd("@nitr", dtoMovDettTec.Nitrati);
            expandoObj.TryAdd("@freat", dtoMovDettTec.Freatimetro);
            expandoObj.TryAdd("@pie1", dtoMovDettTec.Piezo1);
            expandoObj.TryAdd("@pie2", dtoMovDettTec.Piezo2);
            expandoObj.TryAdd("@pie3", dtoMovDettTec.Piezo3);
            expandoObj.TryAdd("@pie4", dtoMovDettTec.Piezo4);
            expandoObj.TryAdd("@sigAv", dtoMovDettTec.Sigla_AV);
            expandoObj.TryAdd("@trapN", dtoMovDettTec.Trap_Num);
            expandoObj.TryAdd("@inn1D", dtoMovDettTec.Inn1_Data);
            expandoObj.TryAdd("@inn2D", dtoMovDettTec.Inn2_Data);
            expandoObj.TryAdd("@inn3D", dtoMovDettTec.Inn3_Data);
            expandoObj.TryAdd("@inn4D", dtoMovDettTec.Inn4_Data);
            expandoObj.TryAdd("@avCod", dtoMovDettTec.Av_Cod);
            expandoObj.TryAdd("@avGru", dtoMovDettTec.Av_Gru);
            expandoObj.TryAdd("@lotto", dtoMovDettTec.Lotto);
            expandoObj.TryAdd("@exStr", dtoMovDettTec.Extra_Str);
            expandoObj.TryAdd("@exInt", dtoMovDettTec.Extra_Int);
            expandoObj.TryAdd("@exDate", dtoMovDettTec.Extra_Date);
            expandoObj.TryAdd("@sogliaCod", dtoMovDettTec.Soglia_Cod);
            expandoObj.TryAdd("@sogliaDes", dtoMovDettTec.Soglia_Des);
            expandoObj.TryAdd("@sogliaQta", dtoMovDettTec.Soglia_Quantita);
            expandoObj.TryAdd("@effic", dtoMovDettTec.Efficienza);
            expandoObj.TryAdd("@cu", dtoMovDettTec.Cu);

            expandoObj.TryAdd("@inviato", dtoMovDettTec.Inviato);
            //expandoObj.TryAdd("@dtInvio", dtoMovDettTec.Data_Invio);
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@inizio", dtoMovDettTec.Validita_Inizio);
            expandoObj.TryAdd("@fine", dtoMovDettTec.Validita_Fine);

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

        public async Task<bool> UpdateAsync(WriteMovDettTecnico dtoMovDettTec, AgronicaCoreParametriServer objParametriServer)
        {
            if (dtoMovDettTec.Id_Reg_Dettaglio == 0)
                throw new Exception("Id_Reg_Dettaglio non valorizzato.");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", dtoMovDettTec.Piva);
            expandoObj.TryAdd("@idAgenda", dtoMovDettTec.Id_Agenda);
            expandoObj.TryAdd("@idMov", dtoMovDettTec.Id_Mov);
            expandoObj.TryAdd("@idMovDet", dtoMovDettTec.Id_Mov_Det);
            expandoObj.TryAdd("@idRegDet", dtoMovDettTec.Id_Reg_Dettaglio);
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);

            var excludedProperties = new HashSet<string>
            {
                "Piva",
                "Sa_Cod",
                "Id_Agenda",
                "Id_Mov",
                "Id_Mov_Det",
                "Id_Reg_Dettaglio",
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
            foreach (var property in typeof(WriteMovDettTecnico).GetProperties())
            {
                if (excludedProperties.Contains(property.Name)) continue;

                var value = property.GetValue(dtoMovDettTec);
                if (value != null)
                {
                    string paramName = "@" + property.Name;
                    setClauses.Add($"{property.Name} = {paramName}");
                    expandoObj.TryAdd(paramName, value);
                }
            }
            if (setClauses.Count == 0) return false;

            string updateQuery =
                $@"UPDATE Mov_Dettaglio_Tecnico SET
                    {string.Join(", ", setClauses)}
                    , Username_Modifica = @userOp
                    , Data_Modifica = GETDATE()
                WHERE Piva = @piva AND Id_Agenda = @idAgenda AND Id_Mov = @idMov AND Id_Mov_Det = @idMovDet AND Id_Reg_Dettaglio = @idRegDet ";

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

        public async Task<bool> DeleteAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Reg_Det, AgronicaCoreParametriServer objParametriServer)
        {
            if (Id_Reg_Det == 0)
                throw new Exception("Id_Reg_Dettaglio non valorizzato.");

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", Piva);
            expandoObj.TryAdd("@idAgenda", Id_Agenda);
            expandoObj.TryAdd("@idMov", Id_Mov);
            expandoObj.TryAdd("@idMovDet", Id_Mov_Det);
            expandoObj.TryAdd("@idRegDet", Id_Reg_Det);

            if (objParametriServer.FlagCancellazioneLogica == enumCancellazioneLogica.CancellazioneLogica)
            {
                stbQuery.AppendLine("UPDATE Mov_Dettaglio_Tecnico SET ")
                    .AppendLine("    Inviato = -1, ")
                    .AppendLine("    Username_Modifica = @userOp ")
                    .AppendLine("WHERE Inviato >= 0 ");
                expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            }
            else
            {
                stbQuery.AppendLine("DELETE FROM Mov_Dettaglio_Tecnico ")
                    .AppendLine("WHERE 1=1 ");
            }

            stbQuery.AppendLine("    AND Piva = @piva ")
                .AppendLine("    AND Id_Agenda = @idAgenda ")
                .AppendLine("    AND Id_Mov = @idMov ")
                .AppendLine("    AND Id_Mov_Det = @idMovDet ")
                .AppendLine("    AND Id_Reg_Dettaglio = @idRegDet ");

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

        public async Task<int> ScriviModificaAsync(WriteMovDettTecnico dtoMovDettTec, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                bool isNew = false;

                if (dtoMovDettTec.Id_Reg_Dettaglio == 0)
                {
                    dtoMovDettTec.Id_Reg_Dettaglio = await _sequenceDal.NuovoId_TabellaAsync("movimenti_dettagli_tecnici", 0, 2000000000, objParametriServer);
                    isNew = true;
                }
                else
                    isNew = !await ExistAsync(dtoMovDettTec.Piva, dtoMovDettTec.Id_Agenda, dtoMovDettTec.Id_Mov, dtoMovDettTec.Id_Mov_Det, dtoMovDettTec.Id_Reg_Dettaglio, objParametriServer);

                if (isNew)
                    await CreateAsync(dtoMovDettTec, objParametriServer);
                else
                    await UpdateAsync(dtoMovDettTec, objParametriServer);

                return dtoMovDettTec.Id_Reg_Dettaglio;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(WriteMovDettTecnico dtoMovDettTec, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await DeleteAsync(dtoMovDettTec.Piva, dtoMovDettTec.Id_Agenda, dtoMovDettTec.Id_Mov, dtoMovDettTec.Id_Mov_Det, dtoMovDettTec.Id_Reg_Dettaglio, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(List<WriteMovDettTecnico> MovDettTec, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(objParametriServer.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var dto in MovDettTec)
                        await DeleteAsync(dto.Piva, dto.Id_Agenda, dto.Id_Mov, dto.Id_Mov_Det, dto.Id_Reg_Dettaglio, objParametriServer);

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
