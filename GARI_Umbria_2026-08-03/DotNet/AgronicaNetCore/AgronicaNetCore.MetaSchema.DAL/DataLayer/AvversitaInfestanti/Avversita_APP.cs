using System.Text;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.AvversitaInfestanti
{
    public class AvversitaApp : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public AvversitaApp(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT Avversita.Av_Cod AS codice");
            stbQuery.AppendLine("     , Av_Des_Vol AS descrizione");
            stbQuery.AppendLine("     , 0 AS gruppoCod");
            stbQuery.AppendLine("FROM Avversita");
            stbQuery.AppendLine("WHERE Av_Des_Vol NOT LIKE '%non usare%'");
            stbQuery.AppendLine("  AND Av_Des_Vol NOT LIKE '%(#)%'");

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
