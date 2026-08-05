using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.AvversitaInfestanti
{
    public class InfestantiAttiveApp : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public InfestantiAttiveApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT InfestantiAttive.Av_Cod AS codice");
            stbQuery.AppendLine("     , InfestantiAttive.Av_Gru AS gruppo");
            stbQuery.AppendLine("     , Avversita.Av_Des_Vol AS descrizione");
            stbQuery.AppendLine("FROM InfestantiAttive");
            stbQuery.AppendLine("INNER JOIN Avversita ON InfestantiAttive.AV_COD = Avversita.Av_Cod");

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
