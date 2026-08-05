using System.Data;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.ContributiColtivazioni
{
    public class ContributiColtivazioni : BaseDALMetaschema, IContributiColtivazioni
    {
        public ContributiColtivazioni(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        public async Task<DataTable> LeggiDescrizioniAsync(
            List<string> codici,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (codici == null || codici.Count == 0)
                throw new ArgumentException("codici non può essere vuota.");

            var stbQuery = new StringBuilder();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT Contributo_Cod, Contributo_Des");
            stbQuery.AppendLine("FROM ContributiColtivazioni");
            stbQuery.AppendLine("WHERE Contributo_Cod IN (@codici)");
            parSqlIn.Add("@codici", FormatClauseIn(codici));

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), new Dictionary<string, object>(), parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
