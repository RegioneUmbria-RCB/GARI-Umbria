using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Codifiche
{
    /// <summary>
    /// Implementazione dell'accesso combinato su <c>Agenda</c>, <c>Codifica_Operazioni_SistemiEsterni</c>
    /// e <c>Imprese_Progetti</c> per recuperare tipo operazione, data operazione e resa prevista.
    /// Riferimento spec: DS04-BL AssemblyPayloadM4AppezzamentoImpianto — DS04.2-BL Mappature.
    /// </summary>
    public class CodificheDAL : BaseDALSostenibilitaCO2, ICodificheDAL
    {
        /// <summary>
        /// Codice sistema esterno registrato in <c>Codifica_Operazioni_SistemiEsterni</c> per il CO2 engine.
        /// Da valorizzare a pre-deployment come da DS04.2-BL "sarà anche da codificare".
        /// </summary>
        private const int SistemaCodCO2 = (int)Enum_CACSistemaCodEnum.Engine_Sostenibilita;

        public CodificheDAL(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyDictionary<int, string>> GetTipoOperazioneByLavCodAsync(
            IReadOnlyList<int> lavCods,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (lavCods == null || lavCods.Count == 0)
                return new Dictionary<int, string>();

            var parametriSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();
            DataTable dt;
          

            // JOIN INNER su Codifica_Operazioni_SistemiEsterni filtra automaticamente
            // le operazioni prive di mapping per il CO2 engine (DS04.2-BL "join secco").
            var strSql = new System.Text.StringBuilder();
            strSql.AppendLine(" SELECT Lav_Cod, lav_cod_esterno AS TipoOperazione ");
            strSql.AppendLine(" FROM Codifica_Operazioni_SistemiEsterni ");
            strSql.AppendLine(" WHERE Lav_Cod IN (@lavCods)");
            strSql.AppendLine(" AND Sistema_Cod = @sistemaCod");

            parametriSql.Add("@sistemaCod", SistemaCodCO2);
            parSqlIn.Add("@lavCods", FormatClauseIn(lavCods.ToList()));

            try
            {
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql.ToString(), parametriSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            // Nessun mapping trovato: dizionario vuoto (il chiamante decide come gestirlo)
            if (dt.Rows.Count == 0)
                return new Dictionary<int, string>();

            var result = new Dictionary<int, string>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                var lavCod = Convert.ToInt32(row["Lav_Cod"]);
                var tipoOp = row["TipoOperazione"]?.ToString() ?? string.Empty;
                result[lavCod] = tipoOp;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyDictionary<int, string>> GetUdmCodEsternoByUdmCodAsync(
            IReadOnlyList<int> udmCods,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (udmCods == null || udmCods.Count == 0)
                return new Dictionary<int, string>();

            var parametriSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            var strSql = new System.Text.StringBuilder();
            strSql.AppendLine(" SELECT Udm_Cod, Udm_Cod_Esterno ");
            strSql.AppendLine(" FROM Codifica_UnitaMisura_SistemiEsterni ");
            strSql.AppendLine(" WHERE Udm_Cod IN (@udmCods)");
            strSql.AppendLine(" AND Sistema_Cod = @sistemaCod");

            parametriSql.Add("@sistemaCod", SistemaCodCO2);
            parSqlIn.Add("@udmCods", FormatClauseIn(udmCods.ToList()));

            DataTable dt;
            try
            {
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql.ToString(), parametriSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            if (dt.Rows.Count == 0)
                return new Dictionary<int, string>();

            var result2 = new Dictionary<int, string>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                var udmCod = Convert.ToInt32(row["Udm_Cod"]);
                var udmEst = row["Udm_Cod_Esterno"]?.ToString() ?? string.Empty;
                result2[udmCod] = udmEst;
            }

            return result2;
        }

    }
}
