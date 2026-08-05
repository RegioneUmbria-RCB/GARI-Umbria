using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.CentriAziendali
{
    public class CentriAziendali_APP : BaseDALAnagrafe, ICentriAziendali_APP
    {
        public CentriAziendali_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrEmpty(piva))
                throw new ArgumentException("Specificare la partita iva.");

            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT")
                .AppendLine("    piva, sa_cod, sa_nome, lat, long")
                .AppendLine("FROM Centri_Aziendali")
                .AppendLine("WHERE")
                .AppendLine("    Centri_Aziendali.PIVA = @piva")
                .AppendLine("    AND Validita_Inizio <= @finestraFine")
                .AppendLine("    AND Validita_Fine >= @finestraInizio")
                .AppendLine("    AND Inviato >= 0");

            parSql.Add("@piva", piva);
            parSql.Add("@finestraFine", objParametriServer.FinestraTemporaleFine);
            parSql.Add("@finestraInizio", objParametriServer.FinestraTemporaleInizio);

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
