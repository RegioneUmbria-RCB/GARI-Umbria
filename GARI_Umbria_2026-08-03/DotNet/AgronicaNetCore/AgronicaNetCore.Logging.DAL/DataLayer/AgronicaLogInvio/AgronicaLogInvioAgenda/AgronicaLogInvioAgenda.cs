using AgronicaCoreDTOStd.Identity;
using AgronicaCoreModelsSTD.anagrafiche;
using System.Dynamic;
using System.Text;
using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogInvio.AgronicaLogInvioAgenda;
using InData.Log.AgronicaLogInvio;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogInvio.AgronicaLogInvioAgenda
{
    public class AgronicaLogInvioAgenda : DAL_Base, IAgronicaLogInvioAgenda
    {
        public AgronicaLogInvioAgenda(IServiceProvider provider) : base(provider)
        {
        }

        public async Task<bool> WriteAsync(WriteAgronicaLogInvioAgenda writeAgronicaLogInvioAgenda, AgronicaCoreParametriServer objParametriServer)
        {

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Agronica_Log_Invio_Agenda (Tipo_Esportazione, ID_Log_Invio, ID_Agenda,")
                .AppendLine("    ID_Operazione_Esterna, inviato, DataInvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine,")
                .AppendLine("    Id_Mov, Id_Mov_Det, Id_Mov_Dest, Causale_Cod, Piva, Chiave_Esterna, Chiave) ");
            stbQuery.AppendLine("VALUES (@Tipo_Esportazione, @ID_Log_Invio, @ID_Agenda, @ID_Operazione_Esterna, @inviato, @DataInvio,  ")
                .AppendLine("    @Data_Creazione, @Data_Modifica, @Username_Creazione, @Username_Modifica, @Validita_Inizio, @Validita_Fine, ")
                .AppendLine("    @Id_Mov, @Id_Mov_Det, @Id_Mov_Dest, @Causale_Cod, @Piva, @Chiave_Esterna, @Chiave) ");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@Tipo_Esportazione", writeAgronicaLogInvioAgenda.Tipo_Esportazione);
            expandoObj.TryAdd("@ID_Log_Invio", writeAgronicaLogInvioAgenda.ID_Log_Invio);
            expandoObj.TryAdd("@ID_Agenda", writeAgronicaLogInvioAgenda.ID_Agenda);
            expandoObj.TryAdd("@ID_Operazione_Esterna", writeAgronicaLogInvioAgenda.ID_Operazione_Esterna == null ? DBNull.Value : writeAgronicaLogInvioAgenda.ID_Operazione_Esterna);
            expandoObj.TryAdd("@inviato", 0);
            expandoObj.TryAdd("@DataInvio", DBNull.Value);

            expandoObj.TryAdd("@Data_Creazione", DateTime.Now);
            expandoObj.TryAdd("@Data_Modifica", DateTime.Now);
            expandoObj.TryAdd("@Username_Creazione", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@Username_Modifica", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@Validita_Inizio", writeAgronicaLogInvioAgenda.Validita_Inizio == null ? CostantiPersonalizzate.AGRODATAINIZIO : writeAgronicaLogInvioAgenda.Validita_Inizio);
            expandoObj.TryAdd("@Validita_Fine", writeAgronicaLogInvioAgenda.Validita_Fine == null ? CostantiPersonalizzate.AGRODATAFINE : writeAgronicaLogInvioAgenda.Validita_Fine);
            expandoObj.TryAdd("@Id_Mov", writeAgronicaLogInvioAgenda.Id_Mov);
            expandoObj.TryAdd("@Id_Mov_Det", writeAgronicaLogInvioAgenda.Id_Mov_Det);
            expandoObj.TryAdd("@Id_Mov_Dest", writeAgronicaLogInvioAgenda.Id_Mov_Dest);
            expandoObj.TryAdd("@Causale_Cod", writeAgronicaLogInvioAgenda.Causale_Cod == null ? 0 : writeAgronicaLogInvioAgenda.Causale_Cod);
            expandoObj.TryAdd("@Piva", writeAgronicaLogInvioAgenda.Piva);
            expandoObj.TryAdd("@Chiave_Esterna", writeAgronicaLogInvioAgenda.Chiave_Esterna);
            expandoObj.TryAdd("@Chiave", writeAgronicaLogInvioAgenda.Chiave);

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

    public async Task<DataTable> Estrai_Agende_PerInvioSistemiEsterniAsync(
           AgronicaCoreParametriServer objParametriServer,
           int tipoEsportazione, string? tipo = null,
           IEnumerable<string>? pivaToFilter = null, IEnumerable<int>? lavcodToFilter = null, IEnumerable<int>? vegcodToFilter = null,
           IEnumerable<int>? typeOperationToFilter = null, IEnumerable<string>? filtroEsiti = null, IntervalloTemporale? dateInterval = null
        )
    {
        const int allValues = 0;
        const string allValuesStr = "";
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        stbQuery.Length = 0;

        stbQuery.AppendLine(" SELECT Piva, Id_Agenda, Data_Ora_RegistrazioneLog, UltimaOperazione, Lav_Cod, Raccoglitore_Cod  ");
        stbQuery.AppendLine(" INTO #log_agenda  ");
        stbQuery.AppendLine(" FROM Agronica_Log_Agenda_UltimaOperazione (NOLOCK)");
        stbQuery.AppendLine(" WHERE 1 = 1 ");
        if (pivaToFilter.Except(new[] { allValuesStr }).Any())
        {
            stbQuery.AppendLine("  AND piva IN (@piva_cod) ");
            parSqlIn.Add("@piva_cod", FormatClauseIn(pivaToFilter.ToList()));
        }
        if (lavcodToFilter.Except(new[] { allValues }).Any())
        {
            stbQuery.AppendLine("  AND lav_cod IN (@lav_cod) ");
            parSqlIn.Add("@lav_cod", FormatClauseIn(lavcodToFilter.ToList()));
        }

        if (!string.IsNullOrEmpty(tipo))
        {
            parSql.Add("@tipo", tipo);

            stbQuery.AppendLine(" SELECT Piva, Codice, SUBSTRING(riferimento,0,CHARINDEX('|',riferimento,0)) ID_Agenda ");
            stbQuery.AppendLine(" INTO #agendeSistemiEsterniCheInvianoDatiAGias ");
            stbQuery.AppendLine(" FROM APP_Dati (NOLOCK)");
            stbQuery.AppendLine(" WHERE tipo = @tipo ");
            stbQuery.AppendLine(" AND SUBSTRING(riferimento, 0, CHARINDEX('|',riferimento,0)) > 0 ");
        }

        parSql.Add("@tipoEsportazione", tipoEsportazione);
        stbQuery.AppendLine(" SELECT ID_Agenda, Max(Id_Log_Invio) id_log_invio ");
        stbQuery.AppendLine(" INTO #log_invio_agenda ");
        stbQuery.AppendLine(" FROM Agronica_Log_Invio_Agenda (NOLOCK)");
        stbQuery.AppendLine(" WHERE Tipo_Esportazione = @tipoEsportazione ");
        stbQuery.AppendLine(" GROUP BY ID_Agenda ");

        stbQuery.AppendLine(" SELECT ID, Esito, Data_Invio ");
        stbQuery.AppendLine(" INTO #log_invio_chiamate ");
        stbQuery.AppendLine(" FROM Agronica_Log_Invio_Chiamate (NOLOCK)");
        stbQuery.AppendLine(" WHERE Tipo_Esportazione = @tipoEsportazione ");

        stbQuery.AppendLine(" Select #log_agenda.Piva, #log_agenda.Id_Agenda, #log_agenda.Lav_Cod, UltimaOperazione, ISNULL(#log_invio_chiamate.Esito, '') AS Esito, ISNULL(#log_invio_chiamate.ID, 0) AS ID_Chiamata  ");
        if (!string.IsNullOrEmpty(tipo))
        {
            stbQuery.AppendLine(",  ISNULL(#agendeSistemiEsterniCheInvianoDatiAGias.Codice,'') As CodiceEsterno ");
        }
        stbQuery.AppendLine(", #log_agenda.Raccoglitore_Cod  ");
        stbQuery.AppendLine(" FROM #log_agenda ");
        stbQuery.AppendLine(" LEFT OUTER JOIN #log_invio_agenda ");
        stbQuery.AppendLine(" ON (#log_agenda.Id_Agenda = #log_invio_agenda.ID_Agenda) ");
        stbQuery.AppendLine(" LEFT OUTER JOIN #log_invio_chiamate ");
        stbQuery.AppendLine(" ON (#log_invio_agenda.id_log_invio = #log_invio_chiamate.ID) ");
        if (!string.IsNullOrEmpty(tipo))
        {
            stbQuery.AppendLine(" LEFT OUTER JOIN #agendeDemetra ");
            stbQuery.AppendLine(" ON (#log_agenda.Id_Agenda = #agendeDemetra.ID_Agenda) ");
        }

        stbQuery.AppendLine(" WHERE 1 = 1 ");
        stbQuery.AppendLine(" AND (#log_invio_chiamate.Data_Invio Is NULL OR (#log_agenda.Data_Ora_RegistrazioneLog >= #log_invio_chiamate.Data_Invio And #log_invio_chiamate.Esito IN ('OK','BLK')) OR (#log_invio_chiamate.Esito = 'KO')) ");

        if (filtroEsiti != null && filtroEsiti.Except(new[] { allValuesStr }).Any())
        {
            stbQuery.AppendLine("  AND ISNULL(#log_invio_chiamate.Esito, '') IN (@esiti) ");
            parSqlIn.Add("@esiti", FormatClauseIn(filtroEsiti.ToList()));
        }

        stbQuery.AppendLine(" DROP TABLE #log_agenda ");
        stbQuery.AppendLine(" DROP TABLE #agendeSistemiEsterniCheInvianoDatiAGias ");
        stbQuery.AppendLine(" DROP TABLE #log_invio_agenda ");
        stbQuery.AppendLine(" DROP TABLE #log_invio_chiamate ");

        try
        {
            return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

    }

        public async Task<bool> CheckAgendaInviataAsync(int idAgenda, int tipoEsportazione, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT TOP 1 1 ");
            stbQuery.AppendLine("FROM Agronica_Log_Invio_Agenda (NOLOCK) a ");
            stbQuery.AppendLine("JOIN Agronica_Log_Invio_Chiamate (NOLOCK) c ON a.ID_Log_Invio = c.ID ");
            stbQuery.AppendLine("WHERE a.ID_Agenda = @idAgenda ");
            stbQuery.AppendLine("  AND a.Tipo_Esportazione = @tipoEsportazione ");

            stbQuery.AppendLine("  AND c.Esito = @esitoSuccesso ");

            var parSql = new Dictionary<string, object>();
            parSql.Add("@idAgenda", idAgenda);
            parSql.Add("@tipoEsportazione", tipoEsportazione);
            parSql.Add("@esitoSuccesso", TipiEsito.Successo);

            try
            {
                var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, null);
                return dt != null && dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                return false;
            }
        }



    }
}

