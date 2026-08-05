using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Vincolo : BaseCodeDescrStr
    {

        public Disciplinare disciplinare { get; set; }
        public Regolamenti regolamento { get; set; }
        public int IdEnte { get; set; }     //ente di riferimento del vincolo

        public Vincolo(string codice) : base(codice, "")
        {

        }

        public Vincolo() : base() { }
    }
}
