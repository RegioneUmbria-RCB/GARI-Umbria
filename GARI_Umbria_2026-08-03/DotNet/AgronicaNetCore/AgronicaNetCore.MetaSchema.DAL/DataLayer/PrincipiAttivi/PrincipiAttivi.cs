using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.PrincipiAttivi
{
    public class PrincipiAttivi : BaseDALMetaschema, IPrincipiAttivi
    {
        public PrincipiAttivi(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
        }

        public async Task<DataTable> LeggiPrincipiAttiviAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            stbQuery.AppendLine(" SELECT      Pa_Des, Pa_Cod");
            stbQuery.AppendLine(" FROM        PrincipiAttivi");
            stbQuery.AppendLine(" ORDER BY    Pa_Des ASC");

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
    }
}
