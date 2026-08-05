using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.LavorazioniAttivitaCDG
{
    public class LavorazioniAttivitaCDG_APP : BaseDALAnagrafe, ILavorazioniAttivitaCDG_APP
    {
        public LavorazioniAttivitaCDG_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine(" SELECT DISTINCT")
                .AppendLine(" ISNULL(A.ID_Attivita,'') AS ID_Attivita,")
                .AppendLine(" A.Attivita_Poliannuale,")
                .AppendLine(" ISNULL(AO.Lav_Cod,0) AS Lav_Cod")
                .AppendLine(" FROM Attivita A")
                .AppendLine(" LEFT OUTER JOIN AttivitaxOperazioni AO ON A.ID_Attivita = AO.ID_Attivita")
                .AppendLine(" WHERE A.Utilizzo_GiasAPP = 1 AND ISNULL(AO.Lav_Cod,0) > 0 AND")
                .AppendLine(" AO.Piva_SuperUser = @pivaSuperUser")
                .AppendLine(" AND A.Inviato >= 0");

            parSql.Add("@pivaSuperUser", objParametriServer.PivaSuperUser);


            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
