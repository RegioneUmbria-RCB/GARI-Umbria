using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema.avversita
{
    public class Avversita : AvversitaGruppo
    {
        public GruppoAvversita gruppo { get; set; }
        public Avversita(int codice) : base(codice)
        {
            classType = costanti.ClassType.Avversita;
        }

        public Avversita() : base()
        {
            classType = costanti.ClassType.Avversita;
        }
    }
}
