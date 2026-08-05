using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.risorse
{
    public class RisorsaAcqua : Risorsa {
        public decimal acqua { get; set; }
        public TipoDoseAcqua doseAcqua { get; set; }

        public enum TipoDoseAcqua
        {
            TOTALE,
            HA
        }
        public RisorsaAcqua()
        {
            classType = costanti.ClassType.RisorsaAcqua;
        }
    }
}
