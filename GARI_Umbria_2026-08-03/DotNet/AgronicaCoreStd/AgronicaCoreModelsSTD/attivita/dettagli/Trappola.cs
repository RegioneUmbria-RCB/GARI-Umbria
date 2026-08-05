using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.dettagli
{
    public class Trappola : BaseCodeDescr
    {
        public int DurataFeromone { get; set; }
        public Ditta Ditta { get; set; }
        public DateTime Scadenza { get; set; }

        public Trappola(int codice) : base(codice, "")
        {

        }

        public Trappola() : base(-1, "")
        {
        }
    }
}
