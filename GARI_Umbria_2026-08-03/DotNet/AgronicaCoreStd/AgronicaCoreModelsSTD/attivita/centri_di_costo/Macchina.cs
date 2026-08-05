using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.centri_di_costo
{
    public class Macchina : CentroDiCosto
    {
        public Macchina()
        {
            classType = costanti.ClassType.Macchina;
            tipo = Tipo.Macchina;
        }

    }
}
