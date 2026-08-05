using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.SmartTractors_HubIoT
{
    public class Settings
    {
        private string _wellKnownUrl;
        private string _apiUrl;
        public string BaseUrl { get; set; } = "";
        public string WellKnownUrl
        {
            get
            {
                if (_wellKnownUrl.StartsWith("http"))
                    return _wellKnownUrl;
                else
                    return BaseUrl + "/" + _wellKnownUrl;
            }
            set
            {
                _wellKnownUrl = value;
            }
        }
        public string ApiUrl
        {
            get
            {
                if (_apiUrl.StartsWith("http"))
                    return _apiUrl;
                else
                    return BaseUrl + _apiUrl;
            }
            set
            {
                _apiUrl = value;
            }
        }
        public List<ElencoColonneDBFPrescriptionMap> mappaturaDatiMappaPrescrizione { get; set; } = new List<ElencoColonneDBFPrescriptionMap>();
    }

    public class ElencoColonneDBFPrescriptionMap
    {
        public string CampoDaRimappare { get; set; } = "";
        public string NuovoCampo { get; set; } = "";
        public string Operazione { get; set; } = "";
        public string ValoreDefault { get; set; } = "";
    }
}
