using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.PianoColturale
{
    public class PianoColturale : BaseDALAnagrafe, IPianoColturale
    {
        public PianoColturale(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable?> ReadPlanningsByCompanyAsync(string partitaIva, AgronicaCoreParametriServer objParametriServer)
        {
            var strSql = "";
            var parSql = new Dictionary<string, object>();
            DataTable? result = null;

            try
            {
                if (string.IsNullOrEmpty(partitaIva))
                    throw new Exception("Specificare la partita iva");

                strSql += "select \n";
                strSql += "Piva \n";
                strSql += ", Programmazione_Cod \n";
                strSql += ", Programmazione_Des \n";
                strSql += ", Note \n";
                strSql += ", Validita_Inizio \n";
                strSql += ", Validita_Fine \n";
                strSql += ", Username_Creazione \n";
                strSql += ", Username_Modifica \n";
                strSql += ", Data_Creazione \n";
                strSql += ", Data_Modifica \n";
                strSql += "from \n";
                strSql += "    Programmazione_Testata \n";
                strSql += "where Piva = @piva \n";
                strSql += "AND Tipo_Pianificazione = 0 \n";

                parSql.Add("@piva", partitaIva);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql, parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }
    }
}
