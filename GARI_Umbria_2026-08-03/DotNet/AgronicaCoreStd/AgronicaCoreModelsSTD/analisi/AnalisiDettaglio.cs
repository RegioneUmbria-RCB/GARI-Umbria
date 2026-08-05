using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.analisi
{
    public class AnalisiDettaglio : BaseCodeDescr
    {
        public AnalisiParametro parametro { get; set; }
        public double? valore1 { get; set; }
        public double margineErrore1 { get; set; }
        public double? valore2 { get; set; }
        public double margineErrore2 { get; set; }
        public Campione campione { get; set; }
        public List<AnalisiCorrezione> correzioni { get; set; }

        public AnalisiDettaglio(int code, string descr) : base(code, descr) { }
        public AnalisiDettaglio(int code) : base(code, "") { }
        public AnalisiDettaglio() : base() { }

    }
}
