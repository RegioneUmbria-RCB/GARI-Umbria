using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Budget
{
    public class Budget : BaseDALAnagrafe, IBudget
    {
        public Budget(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> LeggiTestateBudgetAsync(string piva, int idBudget, bool includiPubblici, AgronicaCoreParametriServer objParametriServer)
        {

            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable dt;

            stbQuery.AppendLine(" SELECT Id_Budget, Nome_Budget, In_Uso ");
            stbQuery.AppendLine(" FROM Budget_Testata ");

            stbQuery.AppendLine(" WHERE 1 = 1");

            if ((piva == null || piva == string.Empty) && includiPubblici)
                stbQuery.AppendLine(" AND Budget_Testata.Sa_Cod = -1 ");
            else if (piva != null && piva != string.Empty && includiPubblici)
            {
                stbQuery.AppendLine(" AND (Budget_Testata.Piva = @piva OR Budget_Testata.Sa_Cod = -1 ) ");
                parametriSql.Add("@Piva", piva);
            }
            else
                //Caso non fattibile: o mi specifichi la piva, o mi fai caricare i  pubblici
                stbQuery.AppendLine(" AND 1 = 2 ");

            if (idBudget > 0)
                stbQuery.AppendLine(" AND Budget_Testata.ID_Budget = @IdBudget ");

            parametriSql.Add("@IdBudget", idBudget);

            try
            {
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return dt;
        }
    }
}
