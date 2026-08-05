using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.MisureAvversitaAnagrafiche
{
    public class MisureAvversitaAnagrafiche_APP : BaseDALAnagrafe, IMisureAvversitaAnagrafiche_APP
    {
        public MisureAvversitaAnagrafiche_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine("SELECT mav.Anag_valore AS anag_valore, mav.Anag_des AS anag_des, mav.MxAV_Cod, mav.Anag_cod AS anag_cod")
                .AppendLine("FROM MisuraXAvversita_Anagrafiche mav")
                .AppendLine("INNER JOIN MisuraXAvversita mv")
                .AppendLine("    ON mav.MxAV_Cod = mv.cod")
                .AppendLine("WHERE 1 = 1");

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString());
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}