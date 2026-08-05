using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Models;
using System.Data;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Base.DataLayer.Security
{
    /// <summary>
    /// ATTENZIONE, per letture normali usare ISecurityLayerDAL, questa classe imposta securityServiceBypass uguale a true
    /// </summary>
    public class SecurityLayer : DAL_Base, ISecurityLayer
    {
        public SecurityLayer(IServiceProvider provider) : base(provider, true)
        {
        }

        [Obsolete("ATTENZIONE, per letture normali usare ISecurityLayerDAL, questa classe imposta securityServiceBypass uguale a true")]
        public DataTable LeggiConnessioni(AgronicaCoreParametriSuperServer objParametriSuperServer)
        {            
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT * FROM Connessioni (NOLOCK) ");

            try
            {
                return GetDataProvider(objParametriSuperServer).ExecuteRead(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriSuperServer, ex);
                throw;
            }
        }
        [Obsolete("ATTENZIONE, per letture normali usare ISecurityLayerDAL, questa classe imposta securityServiceBypass uguale a true")]
        public DataTable LeggiConnessioniDbServerEUtenti(int idDbServerOUtenti, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            var sb = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();

            sb.AppendLine(" WITH ");
            sb.AppendLine("       #cteConnessioni AS");
            sb.AppendLine(" (SELECT");
            sb.AppendLine("       PivaSuperUser, Progressivo ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("       Connessioni (NOLOCK)");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("       ID_DB = @idDb)");

            sb.AppendLine(" SELECT");
            sb.AppendLine("       * ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("       Connessioni (NOLOCK)");
            sb.AppendLine(" JOIN ");
            sb.AppendLine("       #cteConnessioni");
            sb.AppendLine("       ON Connessioni.PivaSuperUser = #cteConnessioni.PivaSuperUser");
            sb.AppendLine("       AND Connessioni.Progressivo = #cteConnessioni.Progressivo");
            sb.AppendLine(" WHERE ");
            sb.AppendLine($"       TipoDB IN ({(int)Enum_TipoDB.GiasServer}, {(int)Enum_TipoDB.GiasUtenti})");
            sb.AppendLine(" ORDER BY ");
            sb.AppendLine("       TipoDB");


            parametriSql.Add("@idDb", idDbServerOUtenti);
            try
            {
                return GetDataProvider(objParametriSuperServer).ExecuteRead(sb.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriSuperServer, ex);
                throw;
            }
        }


        [Obsolete("ATTENZIONE, per letture normali usare ISecurityLayerDAL, questa classe imposta securityServiceBypass uguale a true")]
        public DataTable LeggiConfigurazioneSiti(string chiave, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
           var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();

            CreateQueryLeggiConfigurazioneSiti(stbQuery, parametriSql, chiave);

            try
            {
                return GetDataProvider(objParametriSuperServer).ExecuteRead(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriSuperServer, ex);
                throw;
            }
        }

        private void CreateQueryLeggiConfigurazioneSiti(StringBuilder stbQuery, Dictionary<string, object> parametriSql, string chiave)
        {
            stbQuery.AppendLine("SELECT * FROM Configurazione_Siti (NOLOCK) ");

            if (!string.IsNullOrEmpty(chiave))
            {
                stbQuery.AppendLine("WHERE Chiave=@Chiave");
                parametriSql.TryAdd("@Chiave", chiave);
            }
        }
    }
}
