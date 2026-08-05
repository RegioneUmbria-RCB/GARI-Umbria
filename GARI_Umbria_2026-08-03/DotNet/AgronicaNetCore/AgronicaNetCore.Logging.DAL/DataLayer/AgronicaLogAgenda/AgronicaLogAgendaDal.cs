using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogAgenda
{
    public class AgronicaLogAgendaDal : DAL_Base, IAgronicaLogAgendaDal
    {
        public AgronicaLogAgendaDal(IServiceProvider provider, bool securityServiceBypass = false) : base(provider, securityServiceBypass)
        {
        }

        public async Task<bool> EsistonoModificheDopoLaDataPerAttivitaAppAsync(string piva, DateTime dataLavorazioneMin, DateTime dataModificheMin, AgronicaCoreParametriServer parametriServer)
        {
            var sb = new StringBuilder();

            sb.AppendLine(" SELECT CASE ");
            sb.AppendLine("     WHEN EXISTS ( "); 
            sb.AppendLine("         SELECT ");
            sb.AppendLine("             1 ");
            sb.AppendLine("         FROM ");
            sb.AppendLine("             Agronica_Log_Agenda ");
            sb.AppendLine("         WHERE ");
            sb.AppendLine("             Piva = @piva ");
            sb.AppendLine("             AND Data_Ora_Lavorazione >= @dataLavorazioneMin ");
            sb.AppendLine("             AND Lav_Cod IN (@lavCods) ");
            sb.AppendLine("             AND Data_Ora_RegistrazioneLog >= @dataModificheMin ");
            sb.AppendLine(" )");
            sb.AppendLine(" THEN 1 ");
            sb.AppendLine(" ELSE 0 ");
            sb.AppendLine(" END AS EsisteModifica; ");

            var parameters = new Dictionary<string, object>
            {
                { "@piva", piva },
                { "@dataLavorazioneMin", dataLavorazioneMin },
                { "@dataModificheMin", dataModificheMin },
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
