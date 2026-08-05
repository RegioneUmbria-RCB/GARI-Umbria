using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class AnalisiParametro : BaseCodeDescr
    {

        public string simbolo { get; set; }
        public UnitaDiMisura unitaMisura { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public bool obbligatorio { get; set; } = false;

        public AnalisiParametro(int codice) : base(codice, "")
        {
        }public AnalisiParametro(int codice, string descrizione) : base(codice, descrizione)
        {
        }
        public AnalisiParametro() { }
    }
}
