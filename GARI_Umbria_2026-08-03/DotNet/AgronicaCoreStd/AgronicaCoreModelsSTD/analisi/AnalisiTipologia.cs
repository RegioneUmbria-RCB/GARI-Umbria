using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.analisi
{
    public class AnalisiTipologia : BaseCodeDescr
    {
        public string descrizioneLunga { get; set; }
        public string numeroDeterminazioni { get; set; }
        public List<DettaglioTipologia> dettagli { get; set; }
        public AnalisiTipo tipo { get; set; }

        public AnalisiTipologia(int codice, string descrizione) : base(codice, descrizione) { }
        public AnalisiTipologia(int codice) : base(codice, "") { }
        public AnalisiTipologia() { }

    }
}
