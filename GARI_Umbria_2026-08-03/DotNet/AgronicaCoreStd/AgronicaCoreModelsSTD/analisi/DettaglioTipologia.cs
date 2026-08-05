using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.analisi
{
    public class DettaglioTipologia
    {
        public AnalisiParametro parametro { get; set; }
        public UnitaDiMisura unitaMisura { get; set; }
        public int ordinamento { get; set; }
        public double LDM { get; set; }
        public DettaglioTipologia() { }
    }
}
