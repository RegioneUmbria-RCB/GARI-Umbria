using AgronicaCoreDTOStd.InData.Pratiche;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Text;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.UtentiProfiliPratiche
{
    /// <summary>
    /// Implementazione DAL per la tabella Utenti_Profili_Pratiche.
    /// DS02-BL – GestioneProfiloUtenteFiltriPratiche (§ Persistenze – Utenti_Profili_Pratiche).
    /// </summary>
    public class UtentiProfiliPratiche : BaseDALUtenti, IUtentiProfiliPratiche
    {
        public UtentiProfiliPratiche(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiAsync(string idUtente, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT Servizio_Cod, Considera_Validita_Temporale");
            stbQuery.AppendLine("FROM   Utenti_Profili_Pratiche (NOLOCK)");
            stbQuery.AppendLine("WHERE  IdUtente = @idUtente");

            parSql.Add("@idUtente", idUtente);

            try
            {
                return await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriUtenti, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> EliminaTutteAsync(string idUtente, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine("DELETE FROM Utenti_Profili_Pratiche");
            stbQuery.AppendLine("WHERE  IdUtente = @idUtente");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@idUtente", (object)idUtente);

            try
            {
                return await GetDataProvider(objParametriUtenti).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriUtenti, ex);
                throw;
            }
        }

        public async Task<bool> EliminaTutteAsync(IEnumerable<string> idUtente, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine("DELETE FROM Utenti_Profili_Pratiche");
            stbQuery.AppendLine("WHERE  IdUtente IN (@inUtente)");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@inUtente", FormatClauseIn(idUtente.ToList()));

            try
            {
                return await GetDataProvider(objParametriUtenti).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriUtenti, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> InserisciAsync(string idUtente, List<PraticaFiltrata_IN> pratiche, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            if (pratiche == null || pratiche.Count == 0)
                return true;

            var stbQuery = new StringBuilder();

            stbQuery.AppendLine("INSERT INTO Utenti_Profili_Pratiche");
            stbQuery.AppendLine("       (IdUtente, Servizio_Cod, Considera_Validita_Temporale, Data_Creazione, Data_Modifica)");
            stbQuery.AppendLine("VALUES");

            var expandoObj = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expandoObj;

            for (int i = 0; i < pratiche.Count; i++)
            {
                var sep = i < pratiche.Count - 1 ? "," : "";
                stbQuery.AppendLine($"       (@idUtente_{i}, @servizioCod_{i}, @consideraValidita_{i}, GETDATE(), GETDATE()){sep}");
                expandoDict[$"@idUtente_{i}"] = idUtente;
                expandoDict[$"@servizioCod_{i}"] = pratiche[i].Servizio_Cod;
                expandoDict[$"@consideraValidita_{i}"] = pratiche[i].ConsideraValiditaTemporale ? 1 : 0;
            }

            try
            {
                return await GetDataProvider(objParametriUtenti).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriUtenti, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiServiziDettaglioAsync(List<int> serviziCod, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            if (serviziCod == null || serviziCod.Count == 0)
                return new DataTable();

            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT Servizio_Cod, Servizio_Des, Validita_Inizio, Validita_Fine,");
            stbQuery.AppendLine("       CASE WHEN Validita_Fine >= CAST(GETDATE() AS DATE) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsValida");
            stbQuery.AppendLine("FROM   Servizi_Pratiche (NOLOCK)");
            stbQuery.Append("WHERE  Servizio_Cod IN (");

            for (int i = 0; i < serviziCod.Count; i++)
            {
                var sep = i < serviziCod.Count - 1 ? "," : "";
                stbQuery.Append($"@cod_{i}{sep}");
                parSql.Add($"@cod_{i}", serviziCod[i]);
            }

            stbQuery.AppendLine(")");

            try
            {
                return await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriUtenti, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ServiziSonoValidiAsync(List<int> serviziCod, AgronicaCoreParametriServer objParametriServer)
        {
            if (serviziCod == null || serviziCod.Count == 0)
                return true;

            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT COUNT(*) AS Trovati");
            stbQuery.AppendLine("FROM   Servizi (NOLOCK)");
            stbQuery.Append("WHERE  Inviato = 0 AND Servizio_Cod IN (");

            for (int i = 0; i < serviziCod.Count; i++)
            {
                var sep = i < serviziCod.Count - 1 ? "," : "";
                stbQuery.Append($"@cod_{i}{sep}");
                parSql.Add($"@cod_{i}", serviziCod[i]);
            }

            stbQuery.AppendLine(")");

            try
            {
                DataTable dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);
                if (dt == null || dt.Rows.Count == 0) return false;
                int trovati = Convert.ToInt32(dt.Rows[0]["Trovati"]);
                return trovati == serviziCod.Count;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> CopiaAsync(string template, IEnumerable<string> targets, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            await EliminaTutteAsync(targets, objParametriUtenti);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Utenti_Profili_Pratiche");
            stbQuery.AppendLine("  (IdUtente, Servizio_Cod, Considera_Validita_Temporale, Data_Creazione, Data_Modifica)");
            stbQuery.AppendLine("SELECT");
            stbQuery.AppendLine("  Utenti.Username, TEMPLATE.Servizio_Cod, TEMPLATE.Considera_Validita_Temporale, GETDATE(), GETDATE()");
            stbQuery.AppendLine("FROM Utenti_Profili_Pratiche TEMPLATE, Utenti");
            stbQuery.AppendLine("WHERE  Utenti_Profili_Pratiche.IdUtente = @template");
            stbQuery.AppendLine("  AND Utenti.Username IN (@inUtente)");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@inUtente", FormatClauseIn(targets.ToList()));
            expandoObj.TryAdd("@template", template);

            try
            {
                return await GetDataProvider(objParametriUtenti).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriUtenti, ex);
                throw;
            }
        }
    }
}
