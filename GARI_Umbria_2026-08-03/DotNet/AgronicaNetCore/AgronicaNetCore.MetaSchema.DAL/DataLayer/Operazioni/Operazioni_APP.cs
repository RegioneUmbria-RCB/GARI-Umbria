using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazioni
{
    public class OperazioniApp : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public OperazioniApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT Operazioni.LAV_COD AS codice");
            stbQuery.AppendLine("     , Operazioni.LAV_DES AS descrizione");
            stbQuery.AppendLine("FROM Operazioni");
            stbQuery.AppendLine("INNER JOIN GruppoOperazioni ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD");
            stbQuery.AppendLine("WHERE GruppoOperazioni.Tipo IN ('V','C')");
            stbQuery.AppendLine("AND " + CostantiPersonalizzate.STR_OP_NON_GESTITE);

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
