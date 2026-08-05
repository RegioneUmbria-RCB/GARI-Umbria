using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.analisi
{
    public class AnalisiCorrezione
    {
        public Farmaco farmacoSpecificato { get; set; }
        public PrincipioAttivo parametroCorrezione { get; set; }
        public double qtaCorrezione { get; set; }

        public AnalisiCorrezione() { }
    }
}
