using System.Text;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.ParcoMacchine
{
    public class TipiMacchineApp : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public TipiMacchineApp(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT CLASS_CODE AS codice");
            stbQuery.AppendLine("     , CLASS_DESC AS descrizione");
            stbQuery.AppendLine("FROM Macchine");

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
