using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Models;
using System.Data;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Base.DataLayer.LastCUAA
{
    public class LastCUAA : DAL_Base, ILastCUAA
    {
        public LastCUAA(IServiceProvider provider): base(provider)
        {
        }

        [Obsolete("Usare la versione asincrona del metodo. Questo metodo è presente per essere usato solo dal logger")]
        public string ReadCuaaForLog(AgronicaCoreParametriServer objParametriServer)
        {
            var queryAndParameters = CreateQueryAndParameters(objParametriServer);
            var dt = GetDataProvider(objParametriServer).ExecuteRead(queryAndParameters.Item1, queryAndParameters.Item2);
            return GetResultFromDataTable(dt);
            
            //In questo metodo è importante non fare il log dell'errore. Questo metodo viene usato dal log per leggere l'ultimo CUAA, se faccio il log dell'errore posso creare un loop infinito se la lettura del CUAA va in errore
        }

        public async Task<string> ReadCuaaAsync(AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                var queryAndParameters = CreateQueryAndParameters(objParametriServer);
                var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(queryAndParameters.Item1, queryAndParameters.Item2);
                return GetResultFromDataTable(dt);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        private (string, Dictionary<string, object>) CreateQueryAndParameters(AgronicaCoreParametriServer objParametriServer)
        {
            var sb = new StringBuilder();
            sb.AppendLine(" SELECT TOP 1 ");
            sb.AppendLine("        ISNULL(i.val_cod, '') AS CUAA, l.Piva ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("        Last_Impresa_Selezionata_Dashboard l WITH (NOLOCK) ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("        Imprese_Codici i WITH (NOLOCK) ");
            sb.AppendLine("        ON i.piva = l.piva");
            sb.AppendLine("        AND id_cod = @codiceCuaa");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("        Username = @username");
            sb.AppendLine(" ORDER BY ");
            sb.AppendLine("        l.datainvio DESC");

            var parametriSql = new Dictionary<string, object>
            {
                { "@codiceCuaa", (int)Enum_CodiciAnagrafe.CodiceCUAA },
                { "@username", objParametriServer.UtenteUsername }
            };

            return (sb.ToString(), parametriSql);
        }

        private string GetResultFromDataTable(DataTable dt)
        {
            var result = string.Empty;

            if (dt.Rows.Count > 0)
            {
                var cuaa = dt.Rows[0][0] as string;
                if (string.IsNullOrWhiteSpace(cuaa))
                {
                    result = (string)dt.Rows[0][1] + " [PIVA]";
                }
                else
                {
                    result = cuaa;
                }
            }

            return result;
        }
    }
}
