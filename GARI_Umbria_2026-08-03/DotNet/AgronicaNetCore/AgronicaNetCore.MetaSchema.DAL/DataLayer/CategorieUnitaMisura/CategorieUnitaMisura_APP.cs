using System.Text;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.CategorieUnitaMisura
{
    public class CategorieUnitaMisuraAPP : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public CategorieUnitaMisuraAPP(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT Udm_Des AS descrizione, ");
            stbQuery.AppendLine("Udm_Sim AS simbolo, ");
            stbQuery.AppendLine("Udm_Cod AS codice, ");
            stbQuery.AppendLine("ISNULL(tipoControllo_cod,0) as tipoControllo ");
            stbQuery.AppendLine("FROM UnitaMisura ");
            stbQuery.AppendLine("ORDER BY UDM_DES ");

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
