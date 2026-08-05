using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.ImpiantiIrrigazioni
{
    public class ImpiantiIrrigazioniApp : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public ImpiantiIrrigazioniApp(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT Imp_Cod AS codice");
            stbQuery.AppendLine("     , Imp_Des AS descrizione");
            stbQuery.AppendLine("FROM ImpiantiIrrigazioni");

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
