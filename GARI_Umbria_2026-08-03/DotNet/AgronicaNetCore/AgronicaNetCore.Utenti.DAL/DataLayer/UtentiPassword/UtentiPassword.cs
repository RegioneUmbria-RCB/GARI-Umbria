using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.Exceptions;
using AgronicaNetCore.Utenti.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.UtentiPassword
{
    /// <summary>
    /// Implementazione DAL per la lettura dei dati password utente sulla tabella Utenti.
    /// </summary>
    public class UtentiPassword : BaseDALUtenti, IUtentiPassword
    {
        private const int UsernameMaxLength = 255;

        public UtentiPassword(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
        }

        public async Task<(DateTime? DataUltimaModificaPassword, bool UtenteTrovato)> LeggiDataUltimaModificaPasswordAsync(
            string username,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            ArgumentNullException.ThrowIfNull(username);
            ArgumentNullException.ThrowIfNull(objParametriUtenti);

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Il parametro username non puo essere vuoto.", nameof(username));

            if (username.Length > UsernameMaxLength)
                throw new ArgumentException($"Il parametro username supera la lunghezza massima di {UsernameMaxLength} caratteri.", nameof(username));

            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();

            stbQuery.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
            stbQuery.AppendLine(" SELECT TOP 1 DataUltimaModificaPassword");
            stbQuery.AppendLine(" FROM   Utenti (NOLOCK)");
            stbQuery.AppendLine(" WHERE  UserName = @username");

            parametriSql.Add("@username", username);

            try
            {
                DataTable result = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parametriSql);

                if (result.Rows.Count == 0)
                    return (null, false);

                DataRow row = result.Rows[0];

                if (row["DataUltimaModificaPassword"] == DBNull.Value)
                {
                    LogWarning(SanitizeLogMessage($"L'utente '{username}' esiste ma DataUltimaModificaPassword e NULL (possibile corruzione dati)."), objParametriUtenti);
                    return (null, true);
                }

                return (Convert.ToDateTime(row["DataUltimaModificaPassword"]), true);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriUtenti, ex);
                throw new UtentiDALException($"Errore nel recupero di DataUltimaModificaPassword per l'utente '{username}'.", ex);
            }
        }
    }
}
