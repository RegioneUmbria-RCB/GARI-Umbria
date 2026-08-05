using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class TipologiaSede
    {

        public TIPO codice { get; set; }
        public string descrizione { get; set; }

        public enum TIPO
        {
            SedeLegale = 101,
            SedeAziendale = 102,
            Stabilimento = 103
        }

        public TipologiaSede()
        {
        }

        public TipologiaSede(TIPO codice)
        {
            this.codice = codice;
        }
    }
}
