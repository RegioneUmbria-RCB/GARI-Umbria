using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.MateriePrime
{
    public class MateriePrimeDal : BaseDALAnagrafe, IMateriePrimeDal
    {
        public MateriePrimeDal(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, localizer, securityBypass)
        {

        }
        public async Task<DataTable> LeggiAsync(string piva, int elemCod, int matCod, AgronicaCoreParametriServer parametriServer)
        {
            var sb = new StringBuilder();

            var parametriSql = new Dictionary<string, object>
            {
                { "@pivaSuperUser", parametriServer.PivaSuperUser }
            };

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Cul_Des, Materie_Prime.Veg_Des ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     Materie_Prime ");
            sb.AppendLine(" JOIN ");
            sb.AppendLine("     UtentiXImprese WITH(NOLOCK) ");
            sb.AppendLine("     ON Materie_Prime.Piva = UtentiXImprese.PIVA ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     UtentiXImprese.[USER] = @pivaSuperUser ");
            if (piva != "")
            {
                sb.AppendLine("     AND (Materie_Prime.Piva = @piva OR Materie_Prime.Sa_Cod = -1) ");
                parametriSql.Add("@piva", piva);
            }
            else
            {
                sb.AppendLine("     AND (Materie_Prime.Sa_Cod = -1) ");
            }

            if (elemCod != 0)
            {
                sb.AppendLine("     AND Materie_Prime.Elem_Cod = @elemCod ");
                parametriSql.Add("@elemCod", elemCod);
            }

            if (matCod != 0)
            {
                sb.AppendLine("     AND Materie_Prime.Mat_Cod = @matCod ");
                parametriSql.Add("@matCod", matCod);
            }

            try
            {
                return await GetDataProvider(parametriServer).ExecuteReadAsync(sb.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriServer, ex);
                return null;
            }
        }
    }
}
