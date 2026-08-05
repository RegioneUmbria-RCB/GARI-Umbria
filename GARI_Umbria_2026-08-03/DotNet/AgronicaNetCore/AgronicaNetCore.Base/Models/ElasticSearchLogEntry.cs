using Newtonsoft.Json;

namespace AgronicaNetCore.Base.Models
{
    public class ElasticSearchLogEntry
    {
        public string ambiente { get; set; }
        public string cuaa { get; set; }
        public string componente { get; set; }
        public string opType { get; set; }
        public string timestamp { get; set; }
        public string severity { get; set; }
        public ElasticSearchLogEntryMessage message { get; set; }
        public ElasticSearchLogEntry_Task task { get; set; }
    }

    public class ElasticSearchLogEntryMessage
    {
        public string response { get; set; }
        public object payload { get; set; }
    }

    public class ElasticSearchLogEntry_Task
    {
        public string id { get; set; }
        public string cuaaInit { get; set; }
        public string action { get; set; }
        public string cuaaEnd { get; set; }
        public List<string> category { get; set; }
        [JsonProperty("date")]
        public string dateRef { get; set; }
    }
}
