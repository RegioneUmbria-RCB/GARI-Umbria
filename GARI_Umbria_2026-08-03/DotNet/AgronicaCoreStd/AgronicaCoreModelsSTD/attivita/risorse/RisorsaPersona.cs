using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.risorse
{
    public class RisorsaPersona: RisorsaTimeSheet
    {
        public RisorseUmane risorsaUmana { get; set; }

        public RisorsaPersona()
        {
            classType = costanti.ClassType.RisorsaPersona;
        }
    }
}
