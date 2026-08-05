namespace AgronicaNetCore.Base.Models
{
    public class ConfigurazioneElasticSearch
    {
        public string ElasticSearchUrl { get; set; } = string.Empty;
        public ParametriElasticSearch Parametri { get; set; } = new ParametriElasticSearch();
    }

    public class ParametriElasticSearch
    {
        public string Ambiente { get; set; }
    }
}
