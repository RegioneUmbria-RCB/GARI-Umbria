using AgronicaCoreModelsSTD.attivita;
using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Agenda
{
    public class ScriviMovimenti
    {
        public List<MovimentoDiMagazzino> movimenti;

        public ScriviMovimenti(List<MovimentoDiMagazzino> listMovimenti)
        {
            movimenti = listMovimenti;
        }
    }
}
