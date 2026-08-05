using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.UnitaMisura
{
    public class UnitaMisura : BaseDALMetaschema, IUnitaMisura
    {
        public UnitaMisura(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, localizer, securityBypass)
        {
        }

        public async Task<DataTable> LeggiAsync(int udmCod, int udmCodAux, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            stbQuery.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ");
            stbQuery.AppendLine(" SELECT UnitaMisura.*, ");
            stbQuery.AppendLine(" ISNULL(Tipo_Controlli.TipoControllo_Des, '') as TipoControllo_Des ");
            stbQuery.AppendLine(" FROM      UnitaMisura ");
            stbQuery.AppendLine(" LEFT JOIN (SELECT TipoControllo_Cod, TipoControllo_Des ");
            stbQuery.AppendLine("            FROM Tipo_Controlli) as Tipo_Controlli ");
            stbQuery.AppendLine("        ON UnitaMisura.TipoControllo_Cod = Tipo_Controlli.TipoControllo_Cod ");
            stbQuery.AppendLine(" WHERE   Validita_Inizio <= @dtFine ");
            stbQuery.AppendLine(" AND     Validita_Fine >= @dtInizio ");

            sqlParams.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            sqlParams.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            if (udmCod != 0)
            {
                stbQuery.AppendLine(" AND Udm_Cod = @udmCod");
                sqlParams.Add("@udmCod", udmCod);
            }

            if (udmCodAux != 0)
            {
                stbQuery.AppendLine(" AND Udm_Cod_Aux = @udmCodAux");
                sqlParams.Add("@udmCodAux", udmCodAux);
            }


            switch (objParametriServer.FlagVisibilita)
            {
                case AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati:
                    stbQuery.AppendLine(" AND Inviato = 0 ");
                    break;
                case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati:
                    stbQuery.AppendLine(" AND Inviato =-1 ");
                    break;
                case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti:
                    break;
                default:
                    throw new Exception("Parametro non corretto nella query (FlagVisibilita)");
            }

            stbQuery.AppendLine(" ORDER BY UDM_DES ");

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiAsyncxProtocolli(AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            var parametriSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            List<int> udmxProtocolli = new List<int>() { 0, 2152, 2153, 2154, 2155 };

            DataTable result;

            stbQuery.AppendLine(" SELECT * ");
            stbQuery.AppendLine(" FROM UnitaMisura ");
            stbQuery.AppendLine(" WHERE 1 = 1");
            stbQuery.AppendLine(" AND Udm_Cod IN (@udm_list)");
            parametriSqlIn.Add("@udm_list", FormatClauseIn(udmxProtocolli));

            try
            {
                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql, parametriSqlIn);
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
