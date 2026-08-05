using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.analisi.correzioni
{
    public class ValoreCorrezione
    {
        public AnalisiParametro parametroRiscontrato { get; set; }
        public double valoreRiscontrato { get; set; }
        public Farmaco farmacoSpecificato { get; set; }
        public PrincipioAttivo parametroCorrezione { get; set; }
        public double qtaCorrezione { get; set; }

        public ValoreCorrezione() : base() { }
    }
}
