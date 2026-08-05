using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.AuthDispatcher
{
    public class ElencoUrlFirmati
    {
        public List<UrlFirmato> elencoUrlFirmati { get; set; }
    }

    public class UrlFirmato
    {
        public string Key { get; set; }
        public string SignedUrl { get; set; }
    }
}
