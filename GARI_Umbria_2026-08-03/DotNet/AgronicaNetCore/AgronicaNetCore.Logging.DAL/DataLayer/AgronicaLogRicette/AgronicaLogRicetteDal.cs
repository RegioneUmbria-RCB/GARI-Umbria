using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogRicette
{
    public class AgronicaLogRicetteDal : DAL_Base, IAgronicaLogRicetteDal
    {
        public AgronicaLogRicetteDal(IServiceProvider provider, bool securityServiceBypass = false) : base(provider, securityServiceBypass)
        {
        }

        public async Task<bool> EsistonoModificheDopoLaDataPerBrogliacciAppAsync(string piva, DateTime dataLavorazioneMin, DateTime dataModificheMin, AgronicaCoreParametriServer parametriServer)
        {
            var sb = new StringBuilder();

            sb.AppendLine(" SELECT CASE ");
            sb.AppendLine("     WHEN EXISTS (");
            sb.AppendLine("         SELECT ");
            sb.AppendLine("             1 ");
            sb.AppendLine("         FROM ");
            sb.AppendLine("             Agronica_Log_Ricette");
            sb.AppendLine("         WHERE ");
            sb.AppendLine("             Param3 = @piva ");
            sb.AppendLine("             AND Tipo = @tipo ");
            sb.AppendLine("             AND ISDATE(Param6) = 1");
            sb.AppendLine("             AND CONVERT(datetime, Param6, 103) >= @dataLavorazioneMin ");
            sb.AppendLine("             AND Param5 IN (@lavCods) ");
            sb.AppendLine("             AND Data_Ora_RegistrazioneLog >= @dataModificheMin ");
            sb.AppendLine("     )");
            sb.AppendLine("     THEN 1 ");
            sb.AppendLine("     ELSE 0 ");
            sb.AppendLine(" END AS EsisteModifica;");

            var parameters = new Dictionary<string, object>
            {
                { "@piva", piva },
                { "@dataLavorazioneMin", dataLavorazioneMin },
                { "@dataModificheMin", dataModificheMin },
                { "@tipo", "Ricette_Operazioni"},
            };

            Dictionary<string, Dictionary<Type, List<object>>> parametersIn = new()
            {
                { "@lavCods", FormatClauseIn(CostantiPersonalizzate.OPERAZIONI_GESTITE_APP_DEMETRA_LIST) },
            };

            try
            {
                DataTable dt = await GetDataProvider(parametriServer).ExecuteReadAsync(sb.ToString(), parameters, parametersIn);

                return dt is not null && dt.Rows.Count > 0 && dt.Rows[0].Field<int>("EsisteModifica") == 1;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriServer, ex);
                throw;
            }
        }
    }
}
