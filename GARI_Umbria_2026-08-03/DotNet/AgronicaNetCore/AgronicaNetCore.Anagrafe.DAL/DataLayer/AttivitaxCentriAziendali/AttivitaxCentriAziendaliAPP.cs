using System.Data;
using System.Text;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.AttivitaxCentriAziendali
{
    public class AttivitaxCentriAziendaliAPP : BaseDALAnagrafe, IAttivitaxCentriAziendaliAPP
    {
        public AttivitaxCentriAziendaliAPP(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            if (string.IsNullOrEmpty(piva))
                throw new ArgumentException("Specificare la partita iva.");

            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery
                .AppendLine("SELECT")
                .AppendLine("    AxC.Piva,")
                .AppendLine("    AxC.Sa_Cod,")
                .AppendLine("    AxC.ID_Attivita,")
                .AppendLine("    AxC.Inclusa")
                .AppendLine("FROM AttivitaXCentri_Aziendali AxC")
                .AppendLine("INNER JOIN Attivita A")
                .AppendLine("    ON A.Piva_SuperUser = AxC.Piva_SuperUser")
                .AppendLine("    AND A.ID_Attivita = AxC.ID_Attivita")
                .AppendLine("    AND (A.Piva = @piva OR A.Sa_Cod = -1)")
                .AppendLine("WHERE")
                .AppendLine("    AxC.Piva = @piva")
                .AppendLine("    AND A.Utilizzo_GiasAPP = 1");

            parSql.Add("@piva", piva);

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
