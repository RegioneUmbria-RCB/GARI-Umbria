using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Campi
{
    public class Campi_APP : BaseDALAnagrafe, ICampi_APP
    {
        public Campi_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrEmpty(piva))
                throw new ArgumentException("Specificare la partita iva.");

            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT piva, sa_cod, campo_cod, campo_des")
                .AppendLine("FROM Campi")
                .AppendLine("WHERE")
                .AppendLine("    Campi.Piva = @piva")
                .AppendLine("    AND Campi.Inviato >= 0")
                .AppendLine("    ORDER BY campo_des ASC");

            parSql.Add("@piva", piva);

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
