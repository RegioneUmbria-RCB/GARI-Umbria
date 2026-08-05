using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.AvversitaInfestanti
{
    public class GruppiAvversitaAttiveApp : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public GruppiAvversitaAttiveApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT DISTINCT GruppoAvversitaAttive.Av_Gru AS codice");
            stbQuery.AppendLine("     , GruppoAvversita.Av_Gru_Des AS descrizione");
            stbQuery.AppendLine("FROM GruppoAvversitaAttive");
            stbQuery.AppendLine("INNER JOIN GruppoAvversita ON GruppoAvversitaAttive.Av_Gru = GruppoAvversita.Av_Gru");
            stbQuery.AppendLine("WHERE GruppoAvversita.Av_Gru_Des NOT LIKE '%non usare%'");
            stbQuery.AppendLine("  AND GruppoAvversita.Av_Gru_Des NOT LIKE '%(#)%'");

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
