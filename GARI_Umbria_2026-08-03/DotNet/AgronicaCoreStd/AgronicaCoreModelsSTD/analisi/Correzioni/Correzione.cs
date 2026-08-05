using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.analisi.correzioni;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.analisi.Correzioni
{
    public class Correzione : BaseCodeDescr
    {
        public RifImpresa impreseRiferimento { get; set; }
        public bool pubblico { get; set; }
        public IntervalloTemporale validita { get; set; }
        public AnalisiParametro parametro { get; set; }
        public bool disabilitata { get; set; }
        public BaseCodeDescr definizioneScala { get; set; }
        public double valoreMax { get; set; }
        public double valoreMin { get; set; }
        public string note { get; set; }
        public List<ValoreCorrezione> correzioni { get; set; }
        public Correzione() : base() { }
    }
}
