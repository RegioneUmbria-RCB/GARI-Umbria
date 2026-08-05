using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.RapportiContabili
{
    public class RapportiContabiliApp : BaseDALMetaschema, ILetturaTabellaComuneApp
    {
        public RapportiContabiliApp(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            return await ReadAsync(parameters.ObjParametriTriple.ObjParametriServer);
        }

        public async Task<object> ReadAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT Cod_Rapporto AS codice");
            stbQuery.AppendLine("     , Rapporto_Des AS descrizione");
            stbQuery.AppendLine("     , Cliente AS cliente");
            stbQuery.AppendLine("     , Fornitore AS fornitore");
            stbQuery.AppendLine("     , Dipendente AS dipendente");
            stbQuery.AppendLine("     , Terzista AS terzista");
            stbQuery.AppendLine("     , Legale AS legale");
            stbQuery.AppendLine("     , Agente AS agente");
            stbQuery.AppendLine("     , Consulente AS consulente");
            stbQuery.AppendLine("FROM Rapporti_Contabili");

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
