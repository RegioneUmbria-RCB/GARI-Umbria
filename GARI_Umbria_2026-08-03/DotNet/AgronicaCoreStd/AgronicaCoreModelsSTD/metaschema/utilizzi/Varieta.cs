using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema.utilizzi
{
    public class Varieta: UtilizzoTerreno 
    {
        public Specie specie { get; set; }

        public Varieta() : base () {
            classType = costanti.ClassType.Varieta;
        }

        public Varieta(int codice) : base(codice)
        {
            classType = costanti.ClassType.Varieta;
            descrizione = "";
        }

        public Varieta(int codice, string desc) : base(codice)
        {
            classType = costanti.ClassType.Varieta;
            descrizione = desc;
        }

        public Varieta(int codice, string descrizione, Specie specie) : base(codice)
        {
            classType = costanti.ClassType.Varieta;
            this.descrizione = descrizione;
            this.specie = specie;
        }
    }
}
