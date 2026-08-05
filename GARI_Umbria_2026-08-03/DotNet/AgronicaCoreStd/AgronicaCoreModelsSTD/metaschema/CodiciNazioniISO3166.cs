using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class CodiciNazioniISO3166 : BaseCodeDescrStr
    {
        //public string codice { get; set; }
        //public string descrizione { get; set; }-
        public string codiceNumerico { get; set; }
        public string codiceAlpha3 { get; set; }
        public int gestioneGerarchia { get; set; }

        public CodiciNazioniISO3166() : base("", "")
        {

        }

        public CodiciNazioniISO3166(string codice) : base(codice, "")
        {

        }

        public CodiciNazioniISO3166(string codice, string descrizione) : base(codice, descrizione)
        {

        }
    }
}
