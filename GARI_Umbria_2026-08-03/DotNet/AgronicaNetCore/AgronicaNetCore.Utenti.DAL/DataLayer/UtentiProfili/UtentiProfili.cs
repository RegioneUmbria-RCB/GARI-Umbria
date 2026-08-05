using AgronicaCoreDTOStd.Identity;
using AgronicaCoreModelsSTD.profilazione;
using AgronicaCoreVisibilitaStd;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.UtentiProfili
{
    public class UtentiProfili : BaseDALUtenti, IUtentiProfili
    {
        public const string WildcardPiva = "###########";

        public UtentiProfili(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable?> ReadAsync(
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer,
            int idServizio = 0)
        {
            return await ReadAsync(objParametriUtenti.UtenteUsername, objParametriUtenti, objParametriServer, idServizio);
        }

        public async Task<DataTable?> ReadAsync(
            string username,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer,
            int idServizio = 0
        )
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            DataTable? result;
            try
            {
                stbQuery.AppendLine(" SELECT * ");
                stbQuery.AppendLine(" FROM Utenti_Profili (NOLOCK) ");
                stbQuery.AppendLine(" WHERE Utente_Profilo = @SuperUserUsername ");
                stbQuery.AppendLine(" AND Utente = @Username ");

                if (idServizio != 0)
                {
                    stbQuery.AppendLine(" AND Id_Servizio = @idServizio");
                    parSql.Add("@idServizio", idServizio);
                }

                parSql.Add("@SuperUserUsername", objParametriUtenti.SuperUserUsername);
                parSql.Add("@Username", username);

                result = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }

        private async Task<bool> VisibilitaTotale(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, int idServizio = 0)
        {
            return await VisibilitaTotale(objParametriUtenti.UtenteUsername, objParametriUtenti, objParametriServer, idServizio);
        }

        /// <inheritdoc/>
        public async Task<bool> VisibilitaTotale(string username, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, int idServizio = 0)
        {
            DataTable? dt = await ReadAsync(username, objParametriUtenti, objParametriServer, idServizio);
            if (dt == null || dt.Rows.Count == 0) { return false; }

            var row = dt.Rows[0];
            string sqlFilter = row["Descrizione_2"] as string ?? string.Empty;
            bool praticheFilterEnabled = Convert.ToInt32(row["Filtro_Pratiche_Attivo"]) != 0;
            string operatoreStr = row["Operatore_Filtri"] as string ?? "OR";
            PraticheFilterOperator filtersOperator = operatoreStr.Equals("AND", StringComparison.OrdinalIgnoreCase)
                ? PraticheFilterOperator.And
                : PraticheFilterOperator.Or;

            if (praticheFilterEnabled)
            {
                return string.IsNullOrWhiteSpace(sqlFilter) && filtersOperator == PraticheFilterOperator.Or;
            }
            else
            {
                return string.IsNullOrWhiteSpace(sqlFilter);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> VisibilitaNulla(string username, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, int idServizio = 0)
        {
            DataTable? dt = await ReadAsync(username, objParametriUtenti, objParametriServer, idServizio);
            if (dt == null || dt.Rows.Count == 0) { return false; }

            var row = dt.Rows[0];
            string sqlFilter = row["Descrizione_2"] as string ?? string.Empty;
            bool praticheFilterEnabled = Convert.ToInt32(row["Filtro_Pratiche_Attivo"]) != 0;
            string operatoreStr = row["Operatore_Filtri"] as string ?? "OR";
            PraticheFilterOperator filtersOperator = operatoreStr.Equals("AND", StringComparison.OrdinalIgnoreCase)
                ? PraticheFilterOperator.And
                : PraticheFilterOperator.Or;

            if (praticheFilterEnabled)
            {
                return sqlFilter.Contains(WildcardPiva) && filtersOperator == PraticheFilterOperator.And;
            }
            else
            {
                return sqlFilter.Contains(WildcardPiva);
            }
        }

        public async Task<bool> VisibilitaTotaleGiasOnline(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            return await VisibilitaTotale(objParametriUtenti, objParametriServer, (int)Enum_Id_Servizio.GiasOnline);
        }

        /// <inheritdoc/>
        public async Task<bool> EsisteUtenteAsync(string idUtente, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT TOP(1) 1");
            stbQuery.AppendLine("FROM   Utenti_Profili (NOLOCK)");
            stbQuery.AppendLine("WHERE  Utente = @idUtente");
            stbQuery.AppendLine("AND    Utente_Profilo = @superUserUsername");

            parSql.Add("@idUtente", idUtente);
            parSql.Add("@superUserUsername", objParametriUtenti.SuperUserUsername);

            try
            {
                DataTable dt = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parSql);
                return dt != null && dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriUtenti, ex);
                throw;
            }
        }

        public async Task<bool> EsisteUtenteAsync(IEnumerable<string> idUtente, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT TOP(1) 1");
            stbQuery.AppendLine("FROM   Utenti_Profili (NOLOCK)");
            stbQuery.AppendLine("WHERE  Utente in (@inUtente)");
            stbQuery.AppendLine("AND    Utente_Profilo = @superUserUsername");

            parSql.Add("@inUtente", FormatClauseIn(idUtente));
            parSql.Add("@superUserUsername", objParametriUtenti.SuperUserUsername);

            try
            {
                DataTable dt = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parSql);
                return dt != null && dt.Rows.Count == idUtente.Distinct().Count();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriUtenti, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<DataTable?> LeggiProfiloFiltriAsync(string idUtente, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT Filtro_Pratiche_Attivo, Operatore_Filtri, Descrizione_1, Descrizione_2, Data_Modifica, DataUltimoRiportoUtentiVisibilitaAppoggio");
            stbQuery.AppendLine("FROM   Utenti_Profili (NOLOCK)");
            stbQuery.AppendLine("WHERE  Utente = @idUtente");
            stbQuery.AppendLine("AND    Utente_Profilo = @superUserUsername");

            parSql.Add("@idUtente", idUtente);
            parSql.Add("@superUserUsername", objParametriUtenti.SuperUserUsername);
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
        public async Task<bool> AggiornaFiltroPraticheAsync(string idUtente, bool filtroPraticheAttivo, PraticheFilterOperator operatoreFiltri, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var stbQuery = new StringBuilder();
            string operatoreStr = operatoreFiltri == PraticheFilterOperator.And ? "AND" : "OR";

            stbQuery.AppendLine("UPDATE Utenti_Profili");
            stbQuery.AppendLine("SET    Filtro_Pratiche_Attivo = @filtroPraticheAttivo,");
            stbQuery.AppendLine("       Operatore_Filtri      = @operatoreFiltri,");
            stbQuery.AppendLine("       Data_Modifica        = GETDATE()");
            stbQuery.AppendLine("WHERE  Utente               = @idUtente");
            stbQuery.AppendLine("AND    Utente_Profilo        = @superUserUsername");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@filtroPraticheAttivo", filtroPraticheAttivo);
            expandoObj.TryAdd("@operatoreFiltri", operatoreStr);
            expandoObj.TryAdd("@idUtente", (object)idUtente);
            expandoObj.TryAdd("@superUserUsername", (object)objParametriUtenti.SuperUserUsername);

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

        public async Task<bool> CopyAsync(
            string template,
            IEnumerable<string> targets,
            bool copyHierarchy,
            bool copyPratiche,
            AgronicaCoreParametriUtenti objParametriUtenti
        )
        {
            if (targets == null || !targets.Any())
            {
                throw new ArgumentException("Targets list cannot be null or empty.", nameof(targets));
            }
            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();

            stbQuery.AppendLine("UPDATE Utenti_Profili SET");
            stbQuery.AppendLine("  Data_Modifica = GETDATE()");
            stbQuery.AppendLine("  , Username_Modifica = @utenteModifica");
            if (copyHierarchy)
            {
                stbQuery.AppendLine("  , Descrizione_1 = NEWDATA.Descrizione_1");
                stbQuery.AppendLine("  , Descrizione_2 = NEWDATA.Descrizione_2");
            }
            if (copyPratiche)
            {
                stbQuery.AppendLine("  , filtro_pratiche_attivo = NEWDATA.filtro_pratiche_attivo");
                stbQuery.AppendLine("  , operatore_filtri = NEWDATA.operatore_filtri");
            }
            stbQuery.AppendLine("FROM (");
            stbQuery.AppendLine("  SELECT * FROM Utenti_Profili");
            stbQuery.AppendLine("  WHERE Utente = @template");
            stbQuery.AppendLine("    AND Id_Servizio = @idServizio");
            stbQuery.AppendLine(") NEWDATA");
            stbQuery.AppendLine("WHERE Utenti_Profili.Id_Servizio = @idServizio");
            stbQuery.AppendLine("  AND Utenti_Profili.Utente IN (@inUtenti)");

            expandoObj.TryAdd("@idServizio", Enum_Id_Servizio.GiasOnline);
            expandoObj.TryAdd("@template", template);
            expandoObj.TryAdd("@utenteModifica", objParametriUtenti.UtenteUsername);
            expandoObj.TryAdd("@inUtenti", FormatClauseIn(targets.Take(_batchSize).ToList()));
            expandoObj.TryAdd("@superUserUsername", objParametriUtenti.SuperUserUsername);

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
