using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.NewAgri
{
    public class RequestImportVisibilita
    {
        public string username { get; set; }
        public IEnumerable<string> aziendeConMandato { get; set; }
        public IEnumerable<string> aziendeSenzaMandato { get; set; }
        public IEnumerable<string> gruppiUtente { get; set; }
    }
}
