using System.Collections.Generic;
using AgronicaCoreModelsSTD.NewAgri;

namespace AgronicaCoreDTOStd.InData.NewAgri
{
    public class RequestNDistribuito
    {
        public string CUAA { get; set; }
        public int Regolamento_Cod { get; set; }

        public List<Appezzamento> ElencoAppezzamenti { get; set; }

    }
}
