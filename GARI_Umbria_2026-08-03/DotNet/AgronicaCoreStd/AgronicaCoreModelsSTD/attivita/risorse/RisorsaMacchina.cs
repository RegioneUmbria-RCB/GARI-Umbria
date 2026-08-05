using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.risorse
{
    public class RisorsaMacchina: RisorsaTimeSheet
    {
        /**
         * Puntatore ad Anagrafica della macchina
         */
        public ParcoMacchine macchina { get; set; }

        public RisorsaMacchina()
        {
            classType = costanti.ClassType.RisorsaMacchina;
        }

    }
}
