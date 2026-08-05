using System.Text;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Varieta
{
    public class VarietaApp : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public VarietaApp(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT Cul_Cod AS codice");
            stbQuery.AppendLine("     , Cul_Des AS descrizione");
            stbQuery.AppendLine("     , Veg_Cod AS specieCod");
            stbQuery.AppendLine("FROM Cultivar");

            try
            {
                return await GetDataProvider(parameters.ObjParametriTriple.ObjParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parameters.ObjParametriTriple.ObjParametriServer, ex);
                throw;
            }
        }
    }
}
