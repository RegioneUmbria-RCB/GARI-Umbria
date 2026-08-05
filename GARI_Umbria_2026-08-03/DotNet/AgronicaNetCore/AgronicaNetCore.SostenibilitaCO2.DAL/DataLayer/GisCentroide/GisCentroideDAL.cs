//using AgronicaNetCore.Base.Models;
//using AgronicaNetCore.SostenibilitaCO2.DAL.Resources;
//using Microsoft.Extensions.Localization;
//using System.Data;

//namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.GisCentroide
//{
//    /// <summary>
//    /// Recupera il centroide WKT di un appezzamento dalle tabelle GIS tramite join su
//    /// <c>GIS_ElementiGrafici_Clustering</c>, <c>GIS_ElementiGrafici</c> e <c>GIS_Entita</c>.
//    /// Riferimento spec: DS04-BL AssemblyPayloadM4AppezzamentoImpianto — campo <c>centroide</c>.
//    /// </summary>
//    public class GisCentroideDAL : BaseDALSostenibilitaCO2, IGisCentroideDAL
//    {
//        public GisCentroideDAL(IServiceProvider provider, IStringLocalizer<Messages> localizer)
//            : base(provider, localizer)
//        {
//        }

//        /// <inheritdoc/>
//        public async Task<string?> GetCentroideWktAsync(
//            string piva,
//            string saCod,
//            string appezza,
//            AgronicaCoreParametriServer objParametriServer)
//        {
//            if (string.IsNullOrWhiteSpace(piva))    throw new ArgumentException("Specificare la partita IVA.",    nameof(piva));
//            if (string.IsNullOrWhiteSpace(saCod))   throw new ArgumentException("Specificare il codice Sa_Cod.",  nameof(saCod));
//            if (string.IsNullOrWhiteSpace(appezza)) throw new ArgumentException("Specificare il codice Appezza.", nameof(appezza));

//            const string sql = @"
//                SELECT TOP 1 cl.Centroide_GeoEntity_WKT
//                FROM GIS_ElementiGrafici_Clustering cl
//                INNER JOIN GIS_ElementiGrafici g ON g.ElementoGrafico_Cod = cl.ElementoGrafico_Cod
//                INNER JOIN GIS_Entita e           ON e.Entita_Cod          = g.Entita_Cod
//                WHERE e.Piva    = @piva
//                  AND e.Sa_Cod  = @saCod
//                  AND e.Appezza = @appezza";

//            var sqlParams = new Dictionary<string, object>
//            {
//                ["@piva"]    = piva,
//                ["@saCod"]   = saCod,
//                ["@appezza"] = appezza
//            };

//            try
//            {
//                var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, sqlParams);

//                if (dt.Rows.Count == 0)
//                    return null;

//                var value = dt.Rows[0]["Centroide_GeoEntity_WKT"];
//                return value == DBNull.Value ? null : value?.ToString();
//            }
//            catch (Exception ex)
//            {
//                LogError(ex.Message, objParametriServer, ex);
//                throw;
//            }
//        }
//    }
//}
