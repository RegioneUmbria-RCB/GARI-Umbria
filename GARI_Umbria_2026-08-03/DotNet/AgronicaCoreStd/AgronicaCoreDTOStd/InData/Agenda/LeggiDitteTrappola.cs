using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.dettagli;
using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Agenda
{
    public class LeggiDitteTrappole
    {
        public Trappola trappola { get; set; }

        public LeggiDitteTrappole(int Trap_Cod)
        {
            trappola = new Trappola() { codice = Trap_Cod };
        }

    }
}
