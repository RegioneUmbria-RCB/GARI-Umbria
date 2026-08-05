using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Webhook.DAL.DataLayer.Models
{
    public class FmisContextDataNutrizione : FmisContextDataBase
    {
        public int? RaccoglitoreCod { get; set; } = 0;

        public FmisContextDataNutrizione(int idDb, int idTestata, int? raccoglitoreCod = 0)
            : base(idDb, idTestata,WebhookTipo.EsitoConsiglioNutrizione)
        {
            RaccoglitoreCod = raccoglitoreCod;
        }
    }
}
