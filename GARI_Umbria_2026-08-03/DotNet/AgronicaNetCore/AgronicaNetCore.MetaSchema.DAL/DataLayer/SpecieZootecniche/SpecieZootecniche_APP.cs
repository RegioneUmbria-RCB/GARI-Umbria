using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.SpecieZootecniche
{
    public class SpecieZootecnicheApp : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public SpecieZootecnicheApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT Lista_Specie_Animali.GEN_COD AS genereCod");
            stbQuery.AppendLine("     , Lista_Specie_Animali.SPE_COD AS specieCod");
            stbQuery.AppendLine("     , ISNULL(Lista_IndirizziProd_Animali.IPRO_COD, 0) AS indirizzoProdCod");
            stbQuery.AppendLine("     , CONCAT(GEN_DES, ' - ', CASE WHEN Lista_IndirizziProd_Animali.IPRO_COD > 0");
            stbQuery.AppendLine("           THEN CONCAT(SPE_DES, ' - ', Lista_IndirizziProd_Animali.IPRO_DES)");
            stbQuery.AppendLine("           ELSE SPE_DES END) AS descrizione");
            stbQuery.AppendLine("FROM Lista_Specie_Animali");
            stbQuery.AppendLine("INNER JOIN Lista_Generi_Animali ON Lista_Specie_Animali.GEN_COD = Lista_Generi_Animali.GEN_COD");
            stbQuery.AppendLine("LEFT JOIN Lista_IndirizziProd_Animali");
            stbQuery.AppendLine("    ON Lista_Specie_Animali.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD");
            stbQuery.AppendLine("   AND Lista_Specie_Animali.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD");
            stbQuery.AppendLine("WHERE Lista_Specie_Animali.GEN_COD <> 0");

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
