using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Impostazioni
{
    public class Impostazioni_APP : BaseDALAnagrafe, IImpostazioni_APP
    {
        public Impostazioni_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrEmpty(piva))
                throw new ArgumentException("Specificare la partita iva.");

            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT")
                .AppendLine("    *")
                .AppendLine("FROM")
                .AppendLine("    Imprese_Impostazioni")
                .AppendLine("WHERE")
                .AppendLine("    Imprese_Impostazioni.Piva = @piva")
                .AppendLine("    AND Impostazione_Cod IN (827,828,829,830,831,835,183,850,851,852,853,854,181,860,861,862,863,864,819,820,821,822,832, 833,208,1065,82)");    

            parSql.Add("@piva", piva);

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
