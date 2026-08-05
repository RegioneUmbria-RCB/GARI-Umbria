using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.AvversitaInfestanti
{
    public class GruppiAvversitaApp : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public GruppiAvversitaApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT Av_Gru AS codice");
            stbQuery.AppendLine("     , Av_Gru_Des AS descrizione");
            stbQuery.AppendLine("FROM GruppoAvversita");
            stbQuery.AppendLine("WHERE Av_Gru_Des NOT LIKE '%non usare%'");
            stbQuery.AppendLine("  AND Av_Gru_Des NOT LIKE '%(#)%'");
            stbQuery.AppendLine("  AND Av_Gru_Des_Lat NOT LIKE '%non usare%'");
            stbQuery.AppendLine("  AND Av_Gru_Des_Lat NOT LIKE '%(#)%'");

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
