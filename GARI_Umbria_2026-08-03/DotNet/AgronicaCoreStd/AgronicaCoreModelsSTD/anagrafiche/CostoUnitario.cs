using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class CostoUnitario
    {

        public int codice { get; set; }

        public UnitaDiMisura unitaDiMisura { get; set; }

        public double prezzo { get; set; }

        public IntervalloTemporale validita { get; set; }

        public bool flag_cancellazione { get; set; }

        public CostoUnitario(int codice)
        {
            this.codice = codice;
            this.flag_cancellazione = false;
        }

        public CostoUnitario() { }

    }
}
