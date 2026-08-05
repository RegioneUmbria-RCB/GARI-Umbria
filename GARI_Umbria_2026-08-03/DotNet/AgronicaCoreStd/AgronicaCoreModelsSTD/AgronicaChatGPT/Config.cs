using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.AgronicaChatGPT
{
    public class Config
    {
        public string open_api_key { get; set; }

        public string model { get; set; }

        public List<Assistant_Config> assistants { get; set; }

        public Config() { 
        }
    }
}
