using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSDifesa.DAL.DataLayer;
using AgronicaNetCore.MeteoSuite.DAL.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.MeteoSuite.DAL.DataLayer.Stazioni
{
    public class TbRerQuadrantiDAL : BaseMeteoSuiteDAL, ITbRerQuadrantiDAL
    {
        public TbRerQuadrantiDAL(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, localizer, securityBypass)
        {
        }

        public async Task<DataTable?> LeggiElencoQuadrantiAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            try
            {
                stbQuery.AppendLine(" SELECT Q.ID_Quadrante AS ID, Quadrante_Des AS Descrizione, '' AS Fornitore, '' AS RifFornitore, replace(replace(Q.X_LON_GC, 'E', ''), ',', '.') AS Lat, replace(replace(Q.Y_LAT_GC, 'N', ''), ',', '.') AS Lng ");
                stbQuery.AppendLine(" FROM TB_RER_Quadranti Q ");
                
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                return null;
            }
        }

        public async Task<DataTable?> LeggiQuadrantePerIdAsync(int id, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            try
            {
                stbQuery.AppendLine(" SELECT Q.ID_Quadrante AS ID, Quadrante_Des AS Descrizione, '' AS Fornitore, '' AS RifFornitore, replace(replace(Q.X_LON_GC, 'E', ''), ',', '.') AS Lat, replace(replace(Q.Y_LAT_GC, 'N', ''), ',', '.') AS Lng ");
                stbQuery.AppendLine(" FROM TB_RER_Quadranti Q ");
                stbQuery.AppendLine(" WHERE Q.ID_Quadrante = @ID_Quadrante ");

                sqlParams.TryAdd("@ID_Quadrante", id);

                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                return null;
            }
        }
    }
}
