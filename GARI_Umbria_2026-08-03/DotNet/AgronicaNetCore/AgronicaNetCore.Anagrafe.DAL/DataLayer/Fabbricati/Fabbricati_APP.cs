using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;
using AgronicaNetCore.Base.Constants;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Fabbricati
{
    public class Fabbricati_APP : BaseDALAnagrafe, IFabbricati_APP
    {
        public Fabbricati_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrEmpty(piva))
                throw new ArgumentException("Specificare la partita iva.");

            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT")
                .AppendLine("    Fabbricati.piva,")
                .AppendLine("    Fabbricati.SA_COD,")
                .AppendLine("    Fabbricati.Fabbricato_Cod,")
                .AppendLine("    Fabbricati.Fabbricato_Des,")
                .AppendLine("    Fabbricati.Tipo_Fabbricato_Cod,")
                .AppendLine("    Fabbricati.Indirizzo_Cod,")
                .AppendLine("    COALESCE(usoDaTerzi.val_cod, 0) AS usoDaTerzi,")
                .AppendLine("    Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP,")
                .AppendLine("    Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat,")
                .AppendLine("    ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Indirizzi.stato, Indirizzi.note")
                .AppendLine("FROM Fabbricati")
                //Inner join su visibile da app, perchè sono gli unici fabbricati che devono essere letti da questa query                .AppendLine("INNER JOIN Fabbricati_Codici visibileAPP ON")
                .AppendLine("LEFT JOIN Fabbricati_Codici usoDaTerzi ON")
                .AppendLine("    usoDaTerzi.PIVA = Fabbricati.PIVA")
                .AppendLine("AND usoDaTerzi.sa_cod = Fabbricati.SA_COD")
                .AppendLine("AND usoDaTerzi.Fabbricato_cod = Fabbricati.Fabbricato_Cod")
                .AppendLine($"AND usoDaTerzi.id_cod = {(int)TipiEnumerativi.Enum_CodiciAnagrafe.Fabbricato_Uso_da_Terzi}")
                .AppendLine("LEFT JOIN Indirizzi ON Fabbricati.Indirizzo_Cod = Indirizzi.cod_indirizzo")
                .AppendLine("LEFT JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM")
                .AppendLine("WHERE")
                .AppendLine("    Fabbricati.PIVA = @piva")
                .AppendLine("    AND Fabbricati.Tipo_Fabbricato_Cod IN (20,50,120,121,122,123) ")
                .AppendLine("    AND Fabbricati.inviato >= 0 ")
                .AppendLine("	   AND EXISTS (SELECT 1 FROM Fabbricati_Codici visibileAPP ")
                .AppendLine("	   		       WHERE visibileAPP.PIVA = Fabbricati.PIVA ")
                .AppendLine("	   			   AND visibileAPP.sa_cod = Fabbricati.SA_COD ")
                .AppendLine("	   			   AND visibileAPP.Fabbricato_cod = Fabbricati.Fabbricato_Cod ")
                .AppendLine($"	   			   AND visibileAPP.id_cod = {(int)TipiEnumerativi.Enum_CodiciAnagrafe.Visibile_da_App} AND visibileAPP.val_cod = 1) ");

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
