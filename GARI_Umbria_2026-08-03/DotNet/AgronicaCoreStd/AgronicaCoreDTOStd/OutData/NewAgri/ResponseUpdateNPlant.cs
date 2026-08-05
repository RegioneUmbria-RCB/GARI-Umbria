using AgronicaCoreModelsSTD.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace OutData.NewAgri
{
    public class ResponseUpdateNPlant
    {
        public ICollection<UpdateNPlant> ElencoAppezzamenti { get; set; } = new List<UpdateNPlant>();
    }

    public class  UpdateNPlant : IApiResponse
    {
        public string message { get; set; } = "";
        public string errore { get; set; } = "";
        public string Chiave_Appezzamento { get; set; }

    }

}
