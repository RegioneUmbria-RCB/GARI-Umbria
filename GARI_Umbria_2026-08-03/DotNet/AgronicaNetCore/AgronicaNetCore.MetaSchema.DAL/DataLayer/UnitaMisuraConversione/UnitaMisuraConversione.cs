using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.UnitaMisuraConversione
{
    public class UnitaMisuraConversione : BaseDALMetaschema, IUnitaMisuraConversione
    {
        public UnitaMisuraConversione(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(UnitaMisuraConversione_IN leggiUnitaMisuraConversioneIN, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            if (leggiUnitaMisuraConversioneIN.Udm_Cod_Da == 0)
                throw new Exception("Parametro non corretto nella query (Udm_Cod_Da)");

            if (leggiUnitaMisuraConversioneIN.Udm_Cod_A == 0)
                throw new Exception("Parametro non corretto nella query (Udm_Cod_A)");

            stbQuery.AppendLine(" SELECT  ");
            stbQuery.AppendLine("    udm_cod_da, udm_cod_a, fattoreconversione ");
            stbQuery.AppendLine(" FROM UnitaMisura_Conversione ");
            stbQuery.AppendLine(" WHERE Udm_Cod_Da = @udmCodDa");
            stbQuery.AppendLine(" AND Udm_Cod_A = @udmCodA");
            stbQuery.AppendLine(" AND Validita_Inizio <= @dtFine");
            stbQuery.AppendLine(" AND Validita_Fine >= @dtInizio");

            stbQuery.AppendLine(" ORDER BY Udm_Cod_Da ");

            parametriSql.Add("@udmCodDa", leggiUnitaMisuraConversioneIN.Udm_Cod_Da);
            parametriSql.Add("@udmCodA", leggiUnitaMisuraConversioneIN.Udm_Cod_A);
            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            try
            {
                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return result;
        }
    }
}
