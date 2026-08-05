using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.NewAgri
{
    public class RequestUtente
    {
        public string username { get; set; }
        public string cognome { get; set; }
        public string nome { get; set; }
        public string codice_Fiscale { get; set; }
        public string mail { get; set; }
        public bool bloccato { get; set; }
        public bool spid { get; set; }
        public string[] ruoli { get; set; }
    }
}
