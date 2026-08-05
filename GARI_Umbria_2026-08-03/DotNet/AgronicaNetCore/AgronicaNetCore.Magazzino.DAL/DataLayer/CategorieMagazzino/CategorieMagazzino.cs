using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System.Data;
using System.Text;
using static AgronicaNetCore.Base.Models.AgronicaCoreParametri;

namespace AgronicaNetCore.Magazzino.DAL.DataLayer.CategorieMagazzino
{
    public class CategorieMagazzino : DAL_Base, ICategorieMagazzino
    {
        public CategorieMagazzino(IServiceProvider provider) : base(provider) { }

        public async Task<DataTable> LeggiAsync(
            int elemCod,
            string cauMov,
            bool flagCantina,
            AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>
            {
                { "@validitaFine",   objParametriServer.FinestraTemporaleFine },
                { "@validitaInizio", objParametriServer.FinestraTemporaleInizio },
            };

            stbQuery
                .AppendLine("SELECT *")
                .AppendLine("FROM   CategorieMagazzino")
                .AppendLine("WHERE  CategorieMagazzino.Validita_inizio <= @validitaFine")
                .AppendLine("AND    CategorieMagazzino.Validita_Fine   >= @validitaInizio");

            if (!flagCantina)
                stbQuery.AppendLine("AND    Elem_Cod > 0");

            if (elemCod != 0)
            {
                stbQuery.AppendLine("AND    Elem_Cod = @elemCod");
                sqlParams.Add("@elemCod", elemCod);
            }

            if (!string.IsNullOrEmpty(cauMov) && cauMov != CAU_MOV.CAU_ANIMALE)
            {
                stbQuery.AppendLine("AND    Elem_Cod <> @zooConsistenza");
                sqlParams.Add("@zooConsistenza", ELEM_COD.ZOO_CONSISTENZA);
            }
            
            stbQuery.AppendLine("ORDER BY NomeComune ASC");

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
