using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Text;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio
{
    public class UtentiVisibilitaAppoggio : BaseDALUtenti, IUtentiVisibilitaAppoggio
    {
        public UtentiVisibilitaAppoggio(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public Task<DataTable?> ReadAsync(int entity, AgronicaCoreParametriServer objParametriServer, string piva = "")
        {
            return ReadInternalAsync(entity, objParametriServer.UtenteUsername, objParametriServer, piva);
        }

        public Task<DataTable?> ReadForUsernameAsync(int entity, string username, AgronicaCoreParametriServer objParametriServer, string piva = "")
        {
            return ReadInternalAsync(entity, username, objParametriServer, piva);
        }

        private async Task<DataTable?> ReadInternalAsync(int entity, string username, AgronicaCoreParametriServer objParametriServer, string piva)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            DataTable? result;

            try
            {
                stbQuery.AppendLine(" SELECT * ");
                stbQuery.AppendLine(" FROM Utenti_Visibilita_Appoggio WITH (NOLOCK) ");
                stbQuery.AppendLine(" WHERE PivaSuperUser = @Piva_SuperUser ");
                stbQuery.AppendLine(" AND Username = @Username ");

                if (entity > 0)
                    stbQuery.AppendLine(" AND Entita_Cod = @entity ");

                if (piva != string.Empty)
                {
                    stbQuery.AppendLine(" AND Piva = @Piva ");
                    parSql.Add("@Piva", piva);
                }

                parSql.Add("@entity", entity);
                parSql.Add("@Piva_SuperUser", objParametriServer.PivaSuperUser);
                parSql.Add("@Username", username);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }

            return result;
        }

        public async Task<DataTable?> ReadRowExistsForUsernameAsync(int entity, string username, AgronicaCoreParametriServer parametriServer)
        {
            var sb = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            DataTable? result;

            sb.AppendLine(" SELECT ");
            sb.AppendLine("    CASE WHEN EXISTS (SELECT 1 ");
            sb.AppendLine("                      FROM Utenti_Visibilita_Appoggio WITH (NOLOCK) ");
            sb.AppendLine("                      WHERE PivaSuperUser = @Piva_SuperUser ");
            sb.AppendLine("                      AND Username = @Username ");
            if (entity > 0)
            {
                sb.AppendLine("                      AND Entita_Cod = @entity ");
                parSql.Add("@entity", entity);
            }
            sb.AppendLine("                     ) THEN 1 ");
            sb.AppendLine(" ELSE 0 ");
            sb.AppendLine(" END AS RowExists; ");

            parSql.Add("@Piva_SuperUser", parametriServer.PivaSuperUser);
            parSql.Add("@Username", username);

            try
            {
                result = await GetDataProvider(parametriServer).ExecuteReadAsync(sb.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriServer, ex);
                result = null;
            }

            return result;
        }

        public async Task<DataTable?> ReadMinimumDataAsync(int entity, AgronicaCoreParametriServer parametriServer, bool readSaCod = true)
        {
            var sb = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            DataTable? result;

            try
            {
                sb.AppendLine(" SELECT Piva ");
                if (readSaCod)
                {
                    sb.AppendLine("     , Sa_Cod ");
                }
                sb.AppendLine(" FROM Utenti_Visibilita_Appoggio WITH (NOLOCK) ");
                sb.AppendLine(" WHERE PivaSuperUser = @Piva_SuperUser ");
                sb.AppendLine(" AND Username = @Username ");

                if (entity > 0)
                {
                    sb.AppendLine(" AND Entita_Cod = @entity ");
                    parSql.Add("@entity", entity);
                }

                parSql.Add("@Piva_SuperUser", parametriServer.PivaSuperUser);
                parSql.Add("@Username", parametriServer.UtenteUsername);

                result = await GetDataProvider(parametriServer).ExecuteReadAsync(sb.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriServer, ex);
                result = null;
            }

            return result;
        }

        public async Task<DataTable?> ReadVisibilitaCentriAsync(string? piva, AgronicaCoreParametriServer objParametriServer)

        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            DataTable? result;

            try
            {
                stbQuery.AppendLine(" SELECT * ");
                stbQuery.AppendLine(" FROM Utenti_Visibilita_Appoggio WITH (NOLOCK) ");
                stbQuery.AppendLine(" WHERE PivaSuperUser = @Piva_SuperUser ");
                stbQuery.AppendLine(" AND Username = @Username ");
                stbQuery.AppendLine(" AND Entita_Cod = @Entity ");

                if (!string.IsNullOrEmpty(piva))
                {
                    stbQuery.AppendLine("AND Piva = @Piva");
                }

                parSql.Add("@Entity", TipiEnumerativi.Enum_TipoEntita.Centro);
                parSql.Add("@Piva_SuperUser", objParametriServer.PivaSuperUser);
                parSql.Add("@Username", objParametriServer.UtenteUsername);
                if (!string.IsNullOrEmpty(piva))
                {
                    parSql.Add("@Piva", piva);
                }

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }

            return result;
        }

        public async Task<bool> CancellaPerUtenteAsync(string username, AgronicaCoreParametriServer objParametriServer)
        {
            var sql = new StringBuilder()
                .AppendLine(" DELETE FROM Utenti_Visibilita_Appoggio ")
                .AppendLine(" WHERE PivaSuperUser = @PivaSuperUser ")
                .AppendLine("   AND Username = @Username ")
                .ToString();

            dynamic parameters = new ExpandoObject();
            var dict = (IDictionary<string, object>)parameters;
            dict["@PivaSuperUser"] = objParametriServer.PivaSuperUser;
            dict["@Username"] = username;

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task BulkInsertAsync(DataTable dati, AgronicaCoreParametriServer objParametriServer)
        {
            if (dati == null || dati.Rows.Count == 0)
                return;

            if (objParametriServer.objConnessione is not SqlConnection sqlConn)
                throw new InvalidOperationException("BulkInsertAsync richiede una SqlConnection aperta in objParametriServer.objConnessione.");

            var sqlTx = objParametriServer.objTransazione as SqlTransaction;

            using var bulk = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.Default, sqlTx)
            {
                DestinationTableName = "dbo.Utenti_Visibilita_Appoggio"
            };

            foreach (DataColumn col in dati.Columns)
                bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);

            try
            {
                await bulk.WriteToServerAsync(dati);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<int> ContaAziendeVisibiliAsync(string username, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine(" SELECT COUNT(DISTINCT Piva) ");
            stbQuery.AppendLine(" FROM Utenti_Visibilita_Appoggio WITH (NOLOCK) ");
            stbQuery.AppendLine(" WHERE PivaSuperUser = @PivaSuperUser ");
            stbQuery.AppendLine(" AND Username = @Username ");
            stbQuery.AppendLine(" AND Entita_Cod = @EntitaCod ");

            parSql.Add("@PivaSuperUser", objParametriServer.PivaSuperUser);
            parSql.Add("@Username", username);
            parSql.Add("@EntitaCod", (int)TipiEnumerativi.Enum_TipoEntita.Impresa);

            try
            {
                var result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);
                if (result == null || result.Rows.Count == 0)
                    return 0;
                return Convert.ToInt32(result.Rows[0][0]);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
