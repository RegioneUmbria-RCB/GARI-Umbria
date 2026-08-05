using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.ISTAT
{
    public class ComuniApp : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public ComuniApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT COM AS codice");
            stbQuery.AppendLine("     , LOCALITA AS descrizione");
            stbQuery.AppendLine("     , CAP AS cap");
            stbQuery.AppendLine("     , PROV AS provinciaCod");
            stbQuery.AppendLine("     , Stato_Country AS statoCod");
            stbQuery.AppendLine("FROM Istat");

            try
            {
                return await GetDataProvider(parameters.ObjParametriTriple.ObjParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parameters.ObjParametriTriple.ObjParametriServer, ex);
                throw;
            }
        }
    }
}
