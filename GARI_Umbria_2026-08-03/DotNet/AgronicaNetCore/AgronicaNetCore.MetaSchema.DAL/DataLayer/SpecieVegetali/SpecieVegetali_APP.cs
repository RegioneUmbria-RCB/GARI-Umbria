using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.SpecieVegetali
{
    public class SpecieVegetaliAPP : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public SpecieVegetaliAPP(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT Veg_Cod AS codice");
            stbQuery.AppendLine("     , Veg_Des AS descrizione");
            stbQuery.AppendLine("FROM SpecieVegetali");
            stbQuery.AppendLine("WHERE Gru_Cod <> @gruCodExclude");

            parSql.Add("@gruCodExclude", -1);

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
