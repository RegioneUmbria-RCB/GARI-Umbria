
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using Newtonsoft.Json;
using System;

namespace AgronicaCoreModelsSTD.AgronicaChatGPT
{

    public class Assistant_Config
    {
        public string assistant_id {  get; set; }

        public string assistant_api_key { get; set; }

        public string model {  get; set; }

        private string _assistant_name = "";

        public string assistant_name
        {
            get
            {
                return _assistant_name;
            }
            set
            {
                if (value == Assistant_Name.ProfitosanDPIRegionale)
                {
                    _assistant_name = value;
                }
                else
                {
                    throw new ArgumentException("Invalid value supplied");
                }
            }
        }
        public Assistant_Config()
        {

        }

    }



    public class Assistant_Name
    {
        public const string ProfitosanDPIRegionale = "ProfitosanDPIRegionale";
    }
}
