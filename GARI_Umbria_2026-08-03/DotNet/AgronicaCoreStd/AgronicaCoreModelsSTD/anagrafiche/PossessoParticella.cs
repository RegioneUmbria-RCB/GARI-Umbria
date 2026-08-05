using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class PossessoParticella
    {
        public TitoloDiPossesso titolo_Di_Possesso { get; set; }
        public IntervalloTemporale validita { get; set; }
        public double Area { get; set; }
        public int codice { get; set; }
        public bool flag_cancellazione { get; set; }
        public string codice_particella { get; set; }

        public PossessoParticella(int codice)
        {
            this.codice = codice;
        }

        public PossessoParticella()
        {

        }

    }
}
