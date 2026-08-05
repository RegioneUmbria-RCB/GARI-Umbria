using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazioni
{
    public class OperazioniCombinazioniApp : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public OperazioniCombinazioniApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT ID AS codice");
            stbQuery.AppendLine("     , Lav_Cod1 AS lavCod1");
            stbQuery.AppendLine("     , Lav_Cod2 AS lavCod2");
            stbQuery.AppendLine("     , Lav_Cod3 AS lavCod3");
            stbQuery.AppendLine("     , Lav_Cod4 AS lavCod4");
            stbQuery.AppendLine("     , Lav_Cod5 AS lavCod5");
            stbQuery.AppendLine("FROM Operazioni_Combinazioni");

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
