using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.NewAgri

{
    public class Appezzamento
    {
        public string Chiave_Appezzamento { get; set; }

        public bool Ciclo_Principale { get; set; }
        public bool ZVN { get; set; }

        public decimal N_Mas { get; set; }
        public decimal N_Fabbisogno { get; set; }
        public decimal N_Pua { get; set; }
    }
}
