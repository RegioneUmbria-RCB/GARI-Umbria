using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Operazione.DAL.DataLayer;
using AgronicaNetCore.Operazione.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazioni
{
    public class OperazioniCausali_APP : BaseDALOperazione, IOperazioniCausali_APP
    {
        public OperazioniCausali_APP(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer) { }

        public async Task<object> LeggiAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT Id AS id");
            stbQuery.AppendLine("     , Causale AS causale");
            stbQuery.AppendLine("     , Lav_Cod AS lavCod");
            stbQuery.AppendLine("     , Validita_Inizio AS validitaInizio");
            stbQuery.AppendLine("     , Validita_Fine AS validitaFine");
            stbQuery.AppendLine("FROM Operazione_Causale");
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
