using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Finalita
{
    public class FinalitaApp : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public FinalitaApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT GruppoFinalita.Grfi_Cod AS codice");
            stbQuery.AppendLine("     , GruppoFinalita.Grfi_Des AS descrizione");
            stbQuery.AppendLine("     , GruppoFinalitaxSpecieVegetali.Veg_Cod AS specieCod");
            stbQuery.AppendLine("FROM GruppoFinalita");
            stbQuery.AppendLine("INNER JOIN GruppoFinalitaxSpecieVegetali ON");
            stbQuery.AppendLine("   GruppoFinalitaxSpecieVegetali.Grfi_Cod = GruppoFinalita.Grfi_Cod");

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
