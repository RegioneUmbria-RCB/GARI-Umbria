using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Models;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Base.DataLayer.ConfigurazioneElasticSearch
{
    public class ConfigurazioneElasticSearchDAL : DAL_Base, IConfigurazioneElasticSearchDAL
    {
        private readonly List<string> _databaseDaNonConsiderare = new List<string>()
        {
            "_super_server",
            "_utenti",
            "_matrice"
        };
        public ConfigurazioneElasticSearchDAL(IServiceProvider provider) : base(provider)
        {
        }

        [Obsolete("Metodo che effettua lettura sincrona")]
        public Models.ConfigurazioneElasticSearch LeggiConfigurazione(AgronicaCoreParametriServer objParametriServer)
        {
            if (_databaseDaNonConsiderare.Any(d => objParametriServer.StringaConnessione.ToLower().Contains(d)))
            {
                return null;
            }

            var sb = new StringBuilder();
            sb.AppendLine(" SELECT ");
            sb.AppendLine("         *");
            sb.AppendLine(" FROM ");
            sb.AppendLine("         Configurazione_Siti");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("         Chiave IN ('Coldiretti_ElasticSearchUrl','ParametriElasticSearch') ");

            var dt = GetDataProvider(objParametriServer).ExecuteRead(sb.ToString(),new Dictionary<string, object>());
            if (dt is not null && dt.Rows.Count > 0)
            {
                var chiavi = dt.AsEnumerable()
                    .Select(r => new
                    {
                        Nome = r.Field<string>("Chiave"),
                        Valore = r.Field<string>("Valore")
                    })
                    .ToList();

                var cfg = new Models.ConfigurazioneElasticSearch();
                var url = chiavi.FirstOrDefault(c => c.Nome == "Coldiretti_ElasticSearchUrl");
                if (url is not null && !string.IsNullOrWhiteSpace(url.Valore))
                {
                    cfg.ElasticSearchUrl = url.Valore;
                }

                var parametri = chiavi.FirstOrDefault(c => c.Nome == "ParametriElasticSearch");
                if (parametri is not null && !string.IsNullOrWhiteSpace(parametri.Valore))
                {
                    cfg.Parametri = JsonConvert.DeserializeObject<ParametriElasticSearch>(parametri.Valore);
                }

                return cfg;
            }

            return null;
        }
    }
}
