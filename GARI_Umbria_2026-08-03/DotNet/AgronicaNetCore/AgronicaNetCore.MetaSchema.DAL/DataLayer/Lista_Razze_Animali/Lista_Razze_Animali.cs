using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Lista_Razze_Animali
{
    public class Lista_Razze_Animali : BaseDALMetaschema, ILista_Razze_Animali
    {
        public Lista_Razze_Animali(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
        }

        public async Task<DataTable> LeggiRazzeAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var stb = new StringBuilder();

            stb.AppendLine("SELECT");
            stb.AppendLine("    CAST(GEN_COD AS NVARCHAR(10)) + '_'");
            stb.AppendLine("        + CAST(SPE_COD AS NVARCHAR(10)) + '_'");
            stb.AppendLine("        + CAST(RAZ_COD AS NVARCHAR(10)) AS razza_key,");
            stb.AppendLine("    RAZ_DES AS razza_des");
            stb.AppendLine("FROM Lista_Razze_Animali (NOLOCK)");
            stb.AppendLine("ORDER BY RAZ_DES ASC");

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stb.ToString(), new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
