using System.Text;
using System.Data;
using AgronicaNetCore.Base.Models;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaCoreDTOStd.Identity;
using AgronicaNetCore.Base.Constants;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Impresa
{
    public class Impresa : BaseDALAnagrafe, IImpresa
    {
        public Impresa(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
        }

        public async Task<DataTable?> LeggiClausolaInAsync(List<string> elencoPiva,List<int> elencoVegCod,int varieta, AgronicaCoreParametriServer objParametriServer)
        {
            var strSql = "";
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type,List<object>>>();
            DataTable? result = null;

            try
            {
                if (elencoPiva ==null || elencoPiva.Count<=0)
                    throw new Exception("Specificare l'elenco delle partita iva");

                strSql = @$"
                    Select
                        rag_soc,
                        a.piva,
                        sa_cod,
                        appezza,
                        id_reg,
                        a.validita_inizio,
                        a.validita_fine
                    from reg_impianti a inner join imprese b on (a.piva=b.piva)
                    where
                        a.piva in (@in_1)
                        and (cul_cod in (select cul_cod from cultivar where veg_cod in (@in_2) or cul_cod=@cod_1))
                        and b.piva in (@in_1)
                    ";


                parSqlIn.Add("@in_1", FormatClauseIn(elencoPiva));
                parSqlIn.Add("@in_2", FormatClauseIn(elencoVegCod));

                parSql.Add("@cod_1", varieta);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql, parSql,parSqlIn);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }

        public async Task<DataTable?> LeggiAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {

            var strSql = "";
            var parSql = new Dictionary<string, object>();
            DataTable? result = null;

            try
            {
                if (string.IsNullOrEmpty(piva))
                    throw new Exception("Specificare la partita iva");

                strSql = @$"
                    Select
                        piva,
                        rag_soc,
                        validita_inizio,
                        validita_fine
                    from Imprese
                    where
                        piva=@p1
                    ";
                parSql.Add("@p1", piva);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql, parSql);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }

        public async Task<DataTable> LeggiImpreseAsync(string piva, DataTable dtImpreseVisibili, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            var visibilitaLimitata = dtImpreseVisibili is not null && dtImpreseVisibili.Rows.Count > 0;

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("               i.Piva, i.Rag_Soc, i.partitaIvaReale");
            stbQuery.AppendLine(" FROM ");
            stbQuery.AppendLine("               Imprese i");
            if (visibilitaLimitata)
            {
                stbQuery.AppendLine(" JOIN ");
                stbQuery.AppendLine("               Utenti_Visibilita_Appoggio uvap");
                stbQuery.AppendLine("               ON i.Piva = uvap.Piva");
            }

            stbQuery.AppendLine(" WHERE");
            stbQuery.AppendLine("               1 = 1");

            if (!string.IsNullOrEmpty(piva))
            {
                stbQuery.AppendLine("               and i.Piva = @piva");
                parametriSql.Add("@piva", piva);
            }

            if (visibilitaLimitata)
            {
                stbQuery.AppendLine("               and uvap.Entita_Cod = @entitaCod");
                stbQuery.AppendLine("               and uvap.Username = @username");
                stbQuery.AppendLine("               and uvap.PivaSuperUser = @pivaSuperUser");

                parametriSql.Add("@entitaCod", (int)Enum_TipoEntita.Impresa);
                parametriSql.Add("@username", objParametriServer.UtenteUsername);
                parametriSql.Add("@pivaSuperUser", objParametriServer.PivaSuperUser);
            }

            try
            {
                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        public async Task<DataTable> LeggiPadriAsync(DataTable dtImpreseVisibili, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            var visibilitaLimitata = dtImpreseVisibili is not null && dtImpreseVisibili.Rows.Count > 0;

            stbQuery.AppendLine(" SELECT       Imprese.Piva, Imprese.Rag_Soc");
            stbQuery.AppendLine(" FROM         Imprese (NOLOCK)");
            stbQuery.AppendLine(" JOIN         UtentiXImprese (NOLOCK)");
            stbQuery.AppendLine(" ON           Imprese.Piva = UtentiXImprese.Piva");
            
            if (visibilitaLimitata)
            {
                stbQuery.AppendLine(" JOIN         Utenti_Visibilita_Appoggio  (NOLOCK) ");
                stbQuery.AppendLine(" ON           Imprese.Piva = Utenti_Visibilita_Appoggio.Piva ");
                stbQuery.AppendLine(" AND          Utenti_Visibilita_Appoggio.Entita_Cod = @entitaCod");
            }
            
            stbQuery.AppendLine(" JOIN         ImpresexIndirizzi  (NOLOCK) ");
            stbQuery.AppendLine(" ON           Imprese.PIVA = ImpresexIndirizzi.PIVA");
            stbQuery.AppendLine(" WHERE        Imprese.Validita_Inizio <= @dtFine");
            stbQuery.AppendLine(" AND          Imprese.Validita_Fine >= @dtInizio");
            stbQuery.AppendLine(" AND          UtentiXImprese.[User] = @pivaSuperUser");
            stbQuery.AppendLine(" AND          Imprese.TipoImpresaGerarchia <> 1");
            stbQuery.AppendLine(" AND          ImpresexIndirizzi.Tipo_Indirizzo = 1");
            
            if (visibilitaLimitata)
            {
                stbQuery.AppendLine(" AND          Utenti_Visibilita_Appoggio.Username = @username");
            }

            stbQuery.AppendLine(" ORDER BY     Imprese.Rag_Soc");

            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);
            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriSql.Add("@pivaSuperUser", objParametriServer.PivaSuperUser);
            
            if (visibilitaLimitata)
            {
                parametriSql.Add("@entitaCod", (int)Enum_TipoEntita.Impresa);
                parametriSql.Add("@username", objParametriServer.UtenteUsername);
            }

            try
            {
                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        public async Task<string> PivaFromCuaaAsync(string cuaa, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT PIVA FROM Imprese_Codici");
            stbQuery.AppendLine("WHERE id_cod = @idCod AND val_cod = @cuaa");
            parametriSql.Add("@idCod", IMPRESE_CODICI.CUAA);
            parametriSql.Add("@cuaa", cuaa);

            try
            {
                DataTable result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
                if (result.Rows.Count > 0)
                {
                    return result.Rows[0]["PIVA"].ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return "";
        }

        public async Task<string> CuaaFromPivaAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT val_cod FROM Imprese_Codici");
            stbQuery.AppendLine("WHERE id_cod = @idCod AND piva = @piva");
            parametriSql.Add("@idCod", IMPRESE_CODICI.CUAA);
            parametriSql.Add("@piva", piva);

            try
            {
                DataTable result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
                if (result.Rows.Count > 0)
                {
                    return result.Rows[0]["val_cod"].ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return "";
        }

        public async Task<int> ContaTotaleImpreseAsync(AgronicaCoreParametriServer objParametriServer)
        {
            const string sql = "SELECT COUNT(*) FROM Imprese (NOLOCK) " +
                "WHERE Validita_Inizio < GETDATE() AND Validita_Fine > GETDATE()";

            try
            {
                var result = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, new Dictionary<string, object>());
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

        public async Task<DataTable> LeggiMetadatiImpresaBlockchainAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(piva))
                throw new ArgumentException("Specificare la partita IVA.", nameof(piva));

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT Imprese.rag_soc AS RagioneSociale,");
            stbQuery.AppendLine("   Indirizzi.com_des AS Citta,");
            stbQuery.AppendLine("   lista_reg_prov.Regione_Des AS Regione,");
            stbQuery.AppendLine("   lista_reg_prov.Stato_ISO3");
            stbQuery.AppendLine("FROM Imprese WITH(NOLOCK)");
            stbQuery.AppendLine("JOIN ImpresexIndirizzi WITH(NOLOCK)");
            stbQuery.AppendLine("ON Imprese.PIVA = ImpresexIndirizzi.PIVA");
            stbQuery.AppendLine("JOIN Indirizzi");
            stbQuery.AppendLine("ON Indirizzi.cod_indirizzo = ImpresexIndirizzi.cod_indirizzo");
            stbQuery.AppendLine("JOIN (");
            stbQuery.AppendLine("   SELECT Lista_Regioni.REG,");
            stbQuery.AppendLine("       Lista_Regioni.Regione_Des,");
            stbQuery.AppendLine("       Lista_Province.PROV,");
            stbQuery.AppendLine("       Lista_Province.PROVINCIA,");
            stbQuery.AppendLine("       Lista_Regioni.Stato_Country AS Stato_ISO3");
            stbQuery.AppendLine("   FROM Lista_Regioni WITH(NOLOCK)");
            stbQuery.AppendLine("   JOIN Lista_Province WITH(NOLOCK)");
            stbQuery.AppendLine("   ON Lista_Regioni.reg = Lista_Province.REG");
            stbQuery.AppendLine(") AS lista_reg_prov");
            stbQuery.AppendLine("ON lista_reg_prov.PROV = Indirizzi.pro_cod_istat");
            stbQuery.AppendLine("WHERE Imprese.PIVA = @piva");

            var sqlParams = new Dictionary<string, object> { ["@piva"] = piva };

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

        public async Task<(decimal Latitudine, decimal Longitudine)> GetCoordinateCentroideAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(piva))
                throw new ArgumentException("Specificare la partita IVA.", nameof(piva));

            const string sql = @"
                SELECT
                    COALESCE(MAX(CASE WHEN id_cod = @idCodLat THEN CAST(val_cod AS DECIMAL(10,7)) ELSE NULL END), 0) AS latitudine,
                    COALESCE(MAX(CASE WHEN id_cod = @idCodLon THEN CAST(val_cod AS DECIMAL(10,7)) ELSE NULL END), 0) AS longitudine
                FROM Imprese_Codici
                WHERE piva = @piva
                  AND id_cod IN (@idCodLat, @idCodLon)";

            var sqlParams = new Dictionary<string, object>
            {
                ["@piva"]      = piva,
                ["@idCodLat"]  = IMPRESE_CODICI.LATITUDINE,
                ["@idCodLon"]  = IMPRESE_CODICI.LONGITUDINE
            };

            try
            {
                var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, sqlParams);

                if (dt.Rows.Count == 0)
                    return (0m, 0m);

                var row = dt.Rows[0];
                var lat = row["latitudine"] != DBNull.Value ? Convert.ToDecimal(row["latitudine"]) : 0m;
                var lon = row["longitudine"] != DBNull.Value ? Convert.ToDecimal(row["longitudine"]) : 0m;

                return (lat, lon);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<string> GetNazioneAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(piva))
                throw new ArgumentException("Specificare la partita IVA.", nameof(piva));

            const string sql = @"
                SELECT TOP 1
                    LTRIM(RTRIM(COALESCE(Indirizzi.stato, ''))) AS nazione
                FROM Imprese WITH(NOLOCK)
                INNER JOIN ImpresexIndirizzi WITH(NOLOCK)
                    ON Imprese.PIVA = ImpresexIndirizzi.PIVA
                INNER JOIN Indirizzi WITH(NOLOCK)
                    ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo
                WHERE Imprese.PIVA = @piva
                  AND ImpresexIndirizzi.Tipo_Indirizzo = @tipoIndirizzo
                ORDER BY ImpresexIndirizzi.Validita_Fine DESC, ImpresexIndirizzi.Validita_Inizio DESC";

            var sqlParams = new Dictionary<string, object>
            {
                ["@piva"] = piva,
                ["@tipoIndirizzo"] = (int)Enum_IndirizzoTipo.SedeOperativa
            };

            try
            {
                var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, sqlParams);
                if (dt.Rows.Count == 0)
                    return string.Empty;

                return dt.Rows[0]["nazione"]?.ToString()?.Trim() ?? string.Empty;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
