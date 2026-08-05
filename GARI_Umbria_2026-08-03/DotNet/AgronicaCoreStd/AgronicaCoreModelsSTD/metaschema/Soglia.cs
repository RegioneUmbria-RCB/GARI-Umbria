using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Soglia : BaseCodeDescr
    {
        public decimal quantita { get; set; }

        public avversita.Avversita avversita { get; set; }
        public UnitaDiMisura udm { get; set; }
        public attivita.Lavorazione lavorazione { get; set; }


        public Soglia(int codice) : base(codice, "")
        {
        }
        public Soglia() : base() { }
    }
}

