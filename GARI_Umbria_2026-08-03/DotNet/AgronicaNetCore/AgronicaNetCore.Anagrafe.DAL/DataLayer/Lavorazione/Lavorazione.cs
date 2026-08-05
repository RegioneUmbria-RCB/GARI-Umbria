using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Lavorazione
{
    public class Lavorazione : BaseDALAnagrafe, ILavorazione
    {
        public Lavorazione(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, localizer, securityBypass)
        {
        }

        public async Task<string> GetLavDes(int lavCod, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT Lav_Des FROM Operazioni");
            stbQuery.AppendLine("WHERE Lav_Cod = @lavCod");
            parametriSql.Add("@lavCod", lavCod);

            try
            {
                DataTable result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
                if (result.Rows.Count > 0)
                {
                    return result.Rows[0]["Lav_Des"].ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return "";
        }
    }
}
