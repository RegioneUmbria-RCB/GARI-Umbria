using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.ISTAT
{
    public class ProvinceApp : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public ProvinceApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT Lista_Province.PROV AS codice");
            stbQuery.AppendLine("     , Lista_Province.PROVINCIA AS descrizione");
            stbQuery.AppendLine("     , Lista_Province.SIGLA AS sigla");
            stbQuery.AppendLine("     , Lista_Province.reg AS regioneCod");
            stbQuery.AppendLine("     , Lista_Regioni.Regione_Des AS regioneDes");
            stbQuery.AppendLine("     , Lista_Province.Stato_Country AS statoCod");
            stbQuery.AppendLine("FROM Lista_Province");
            stbQuery.AppendLine("INNER JOIN Lista_Regioni");
            stbQuery.AppendLine("    ON Lista_Regioni.REG = Lista_Province.REG");
            stbQuery.AppendLine("   AND Lista_Regioni.Stato_Country = Lista_Province.Stato_Country");

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
