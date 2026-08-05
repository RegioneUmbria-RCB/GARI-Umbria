using AgronicaCoreModelsSTD.Zoo;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Progetti
{
    public class Progetti_APP : BaseDALAnagrafe, IProgetti_APP
    {
        public Progetti_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrEmpty(piva))
                throw new ArgumentException("Specificare la partita iva.");

            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine(" SELECT")
                .AppendLine("   i.Imputazione_Cod, i_f.imputazione_Fase_Cod As Id_Attivita,")
                .AppendLine("   i.Imputazione_Nome + ' (' + i.Imputazione_Cod_Des + ')' AS Imputazione_Nome,")
                .AppendLine("   i.Piva")
                .AppendLine(" FROM Imputazioni_Fasi i_f")
                .AppendLine(" JOIN Imputazioni i")
                .AppendLine("   ON i.Imputazione_Cod = i_f.Imputazione_Cod")
                .AppendLine(" JOIN Attivita A")
                .AppendLine("   ON A.ID_Attivita = i_f.imputazione_Fase_Cod")
                .AppendLine(" WHERE A.Piva_SuperUser = @pivaSuperUser")
                .AppendLine(" AND i.Piva_SuperUser = @pivaSuperUser")
                .AppendLine(" AND i.Piva = @piva")
                .AppendLine(" AND A.Utilizzo_GiasAPP = 1")
                .AppendLine(" AND i_f.tipo_fase = -1")
                .AppendLine(" AND A.Inviato >= 0");

            parSql.Add("@piva", piva);
            parSql.Add("@pivaSuperUser", objParametriServer.PivaSuperUser);

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
