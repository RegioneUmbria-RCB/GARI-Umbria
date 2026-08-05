using System;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Agea
{
    public class Bundle
    {
        public string cuaa { get; set; }

        public int campaignYear { get; set; }

        public string creationUser { get; set; }

        public string farmDescription { get; set; }

        /// <summary>
        /// Contiene il Supply in formato JSON
        /// </summary>
        public string data { get; set; }
    }

    public class RequestBundleGias
    {
        public string linkCoreApi { get; set; }
        public string token { get; set; }
        public string exportBundleToAgea { get; set; }
    }
}
