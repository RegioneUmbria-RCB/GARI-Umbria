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
    public class TbRerStazioniDAL : BaseMeteoSuiteDAL, ITbRerStazioniDAL
    {
        public TbRerStazioniDAL(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, localizer, securityBypass)
        {
        }

        public async Task<DataTable?> LeggiElencoStazioniAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            try
            {
                stbQuery.AppendLine(" SELECT S.ID_Stazione AS ID, Stazione_Des AS Descrizione, '' AS Fornitore, '' AS RifFornitore, replace(replace(Q.X_LON_GC, 'E', ''), ',', '.') AS Lat, replace(replace(Q.Y_LAT_GC, 'N', ''), ',', '.') AS Lng ");
                stbQuery.AppendLine(" FROM TB_RER_Stazioni S ");
                stbQuery.AppendLine(" INNER JOIN TB_RER_QuadrantixStazioni QS ON S.ID_Stazione = QS.ID_Stazione ");
                stbQuery.AppendLine(" INNER JOIN TB_RER_Quadranti Q ON Q.ID_Quadrante = QS.ID_Quadrante ");

                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                return null;
            }
        }

        public async Task<DataTable?> LeggiStazionePerIdAsync(int id, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            try
            {
                stbQuery.AppendLine(" SELECT S.ID_Stazione AS ID, Stazione_Des AS Descrizione, '' AS Fornitore, '' AS RifFornitore, replace(replace(Q.X_LON_GC, 'E', ''), ',', '.') AS Lat, replace(replace(Q.Y_LAT_GC, 'N', ''), ',', '.') AS Lng ");
                stbQuery.AppendLine(" FROM TB_RER_Stazioni S ");
                stbQuery.AppendLine(" INNER JOIN TB_RER_QuadrantixStazioni QS ON S.ID_Stazione = QS.ID_Stazione ");
                stbQuery.AppendLine(" INNER JOIN TB_RER_Quadranti Q ON Q.ID_Quadrante = QS.ID_Quadrante ");
                stbQuery.AppendLine(" WHERE S.ID_Stazione = @ID_Stazione ");

                sqlParams.TryAdd("@ID_Stazione", id);

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
