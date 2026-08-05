using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.AttivitaCDG
{
    public class AttivitaCDG_APP : BaseDALAnagrafe, IAttivitaCDG_APP
    {
        public AttivitaCDG_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine(" SELECT Id_Attivita, [Desc]")
                .AppendLine(" FROM Attivita A")
                .AppendLine(" WHERE A.Piva_SuperUser = @pivaSuperUser")
                .AppendLine(" AND A.Utilizzo_GiasAPP = 1");

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
