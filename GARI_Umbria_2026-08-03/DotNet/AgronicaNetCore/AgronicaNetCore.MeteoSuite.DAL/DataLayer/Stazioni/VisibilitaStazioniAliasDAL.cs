using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSDifesa.DAL.DataLayer;
using AgronicaNetCore.MeteoSuite.DAL.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.MeteoSuite.DAL.DataLayer.Stazioni
{
    public class VisibilitaStazioniAliasDAL : BaseMeteoSuiteDAL, IVisibilitaStazioniAliasDAL
    {
        public VisibilitaStazioniAliasDAL(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, localizer, securityBypass)
        {
        }

        public async Task<DataTable?> LeggiElencoAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            
            try
            {
                stbQuery.AppendLine(" SELECT * FROM MeteoSuite_VisibilitaStazioniAlias ");
                stbQuery.AppendLine(" WHERE PIVA = @piva ");

                sqlParams.TryAdd("@piva", piva);

                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                return null;
            }
        }

        public async Task<DataTable?> LeggiPerStazioneAsync(string piva, int idStazione, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            try
            {
                stbQuery.AppendLine(" SELECT * FROM MeteoSuite_VisibilitaStazioniAlias ");
                stbQuery.AppendLine(" WHERE PIVA = @piva ");
                stbQuery.AppendLine(" AND Id_Stazione = @id_stazione ");

                sqlParams.TryAdd("@piva", piva);
                sqlParams.TryAdd("@id_stazione", idStazione);

                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                return null;
            }
        }

        public async Task<DataTable?> UpsertAsync(string piva, int idStazione, string strAlias, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            var exp = new ExpandoObject();

            try
            {
                stbQuery.AppendLine(" MERGE INTO MeteoSuite_VisibilitaStazioniAlias AS t");
                stbQuery.AppendLine(" USING");
                stbQuery.AppendLine("    (SELECT PIVA = @piva, Id_Stazione = @id_stazione, Alias = @alias) AS s");
                stbQuery.AppendLine(" ON t.PIVA = s.PIVA AND t.Id_Stazione = s.Id_Stazione");
                stbQuery.AppendLine(" WHEN MATCHED THEN");
                stbQuery.AppendLine("    UPDATE SET t.Alias = s.Alias, Username_Modifica = @usernameModifica, Data_Modifica = @dataModifica");
                stbQuery.AppendLine(" WHEN NOT MATCHED THEN");
                stbQuery.AppendLine("    INSERT (PIVA, Id_Stazione, Alias, Username_Creazione, Username_Modifica)");
                stbQuery.AppendLine("    VALUES (s.PIVA, s.Id_Stazione, s.Alias, @usernameCreazione, @usernameModifica);");

                exp.TryAdd("@piva", piva);
                exp.TryAdd("@id_stazione", idStazione);
                exp.TryAdd("@alias", strAlias);
                exp.TryAdd("@usernameCreazione", (object?)objParametriServer.UtenteUsername ?? DBNull.Value);
                exp.TryAdd("@usernameModifica", (object?)objParametriServer.UtenteUsername ?? DBNull.Value);
                exp.TryAdd("@dataModifica", DateTime.Now);

                bool upsertResult = await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), exp);
                if (!upsertResult)
                {
                    return null;
                }

                stbQuery = new StringBuilder();
                stbQuery.AppendLine();
                stbQuery.AppendLine(" SELECT * FROM MeteoSuite_VisibilitaStazioniAlias ");
                stbQuery.AppendLine(" WHERE PIVA = @piva ");
                stbQuery.AppendLine(" AND Id_Stazione = @id_stazione ");

                sqlParams.TryAdd("@piva", piva);
                sqlParams.TryAdd("@id_stazione", idStazione);

                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(string piva, int idStazione, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var exp = new ExpandoObject();

            try
            {
                stbQuery.AppendLine(" DELETE FROM MeteoSuite_VisibilitaStazioniAlias ");
                stbQuery.AppendLine(" WHERE PIVA = @piva ");
                stbQuery.AppendLine(" AND Id_Stazione = @id_stazione ");

                exp.TryAdd("@piva", piva);
                exp.TryAdd("@id_stazione", idStazione);

                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), exp);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                return false;
            }
        }
    }
}
