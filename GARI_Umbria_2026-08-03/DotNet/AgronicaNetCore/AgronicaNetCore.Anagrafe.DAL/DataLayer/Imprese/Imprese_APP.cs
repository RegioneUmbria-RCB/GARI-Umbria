using System.Data;
using System.Text;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Imprese
{
    public class Imprese_APP : BaseDALAnagrafe, IImprese_APP
    {
        public Imprese_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer)
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
                .AppendLine(";WITH CTE_Top1ImpresaPadre AS (")
                .AppendLine("    SELECT TOP 1 padre impresaPadre, Figlio")
                .AppendLine("    FROM GerarchiaImprese")
                .AppendLine("    WHERE Figlio = @piva")
                .AppendLine(")")
                .AppendLine("SELECT")
                .AppendLine("    Imprese.piva,")
                .AppendLine("    Imprese.rag_soc,")
                .AppendLine("    Imprese.TipoImpresaGerarchia,")
                .AppendLine("    ImpresaPadre.impresaPadre,")
                .AppendLine("    Imprese.partitaIvaReale")
                .AppendLine("FROM Imprese")
                .AppendLine("    LEFT JOIN CTE_Top1ImpresaPadre ImpresaPadre")
                .AppendLine("        ON ImpresaPadre.figlio = Imprese.PIVA")
                .AppendLine("WHERE Imprese.Piva = @piva");

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
