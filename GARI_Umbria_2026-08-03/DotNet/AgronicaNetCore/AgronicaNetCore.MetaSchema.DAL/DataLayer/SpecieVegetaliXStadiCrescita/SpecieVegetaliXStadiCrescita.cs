using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.SpecieVegetaliXStadiCrescita
{
    /// <summary>
    /// Implementazione del DAL di lettura per <c>SpecieVegetaliXStadiCrescita</c>.
    /// Traduzione C# della classe VB.NET <c>AgronicaCoreMetaSchemaDAL.SpecieVegetaliXStadiCrescita_R</c>.
    /// Riferimento: DS02-BL ConfrontiFasiFenologicheQDCA — Regole di Business — Confronto per unicità.
    /// </summary>
    public class SpecieVegetaliXStadiCrescita : BaseDALMetaschema, ISpecieVegetaliXStadiCrescita
    {
        public SpecieVegetaliXStadiCrescita(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <inheritdoc />
        public async Task<DataTable> LeggiAsync(
            int vegCod,
            int idBbch,
            int ffCod,
            bool soloVisibili,
            bool soloFioritura,
            bool soloRipresaVegetativa,
            AgronicaCoreParametriServer objParametriServer,
            DateTime? agrodataFine = null,
            DateTime? agrodataInizio = null)
        {
            if (agrodataFine == null)   agrodataFine   = CostantiPersonalizzate.AGRODATAFINE_DATE;
            if (agrodataInizio == null) agrodataInizio = CostantiPersonalizzate.AGRODATAINIZIO_DATE;

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine(" SELECT * ");
            stbQuery.AppendLine(" FROM  SpecieVegetaliXStadiCrescita  ss");
            stbQuery.AppendLine(" INNER JOIN SpecieVegetali s ON ss.VEG_COD = s.VEG_COD");
            stbQuery.AppendLine(" LEFT OUTER JOIN Stadi_Crescita_BBCH sc ON ss.ID_BBCH = sc.ID_BBCH");

            stbQuery.AppendLine(" WHERE ss.Validita_inizio <= @dtFine");
            stbQuery.AppendLine(" AND   ss.Validita_Fine   >= @dtInizio");
            stbQuery.AppendLine(" AND   s.Validita_inizio  <= @dtFine");
            stbQuery.AppendLine(" AND   s.Validita_Fine    >= @dtInizio");

            sqlParams.Add("@dtFine",   agrodataFine);
            sqlParams.Add("@dtInizio", agrodataInizio);

            if (vegCod != 0)
            {
                stbQuery.AppendLine(" AND ss.VEG_COD = @vegCod");
                sqlParams.Add("@vegCod", vegCod);
            }

            if (idBbch != 0)
            {
                stbQuery.AppendLine(" AND sc.ID_BBCH = @idBbch");
                sqlParams.Add("@idBbch", idBbch);
            }

            if (ffCod != 0)
            {
                stbQuery.AppendLine(" AND ss.FF_COD = @ffCod");
                sqlParams.Add("@ffCod", ffCod);
            }

            if (soloVisibili)
                stbQuery.AppendLine(" AND ss.Flag_Visibile = 1");

            if (soloFioritura)
                stbQuery.AppendLine(" AND ss.Flag_Fioritura = 1");

            if (soloRipresaVegetativa)
                stbQuery.AppendLine(" AND ss.Flag_RipresaVegetativa = 1");

            stbQuery.AppendLine(" ORDER BY s.VEG_DES ASC, ss.progressivo ASC");

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<DataTable> LeggiPerCodSsListAsync(
            List<int> codSsList,
            AgronicaCoreParametriServer objParametriServer,
            DateTime? agrodataFine = null,
            DateTime? agrodataInizio = null)
        {
            if (codSsList is null || codSsList.Count == 0)
                return new DataTable();

            if (agrodataFine == null) agrodataFine = CostantiPersonalizzate.AGRODATAFINE_DATE;
            if (agrodataInizio == null) agrodataInizio = CostantiPersonalizzate.AGRODATAINIZIO_DATE;

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            var sqlParamsIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine(" SELECT *");
            stbQuery.AppendLine(" FROM  SpecieVegetaliXStadiCrescita  ss");

            stbQuery.AppendLine(" WHERE ss.cod_ss IN (@codSsList)");
            stbQuery.AppendLine(" AND   ss.Validita_inizio <= @dtFine");
            stbQuery.AppendLine(" AND   ss.Validita_Fine   >= @dtInizio");

            sqlParams.Add("@dtFine",   agrodataFine);
            sqlParams.Add("@dtInizio", agrodataInizio);
            sqlParamsIn.Add("@codSsList", FormatClauseIn(codSsList));

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), sqlParams, sqlParamsIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

    }
}
