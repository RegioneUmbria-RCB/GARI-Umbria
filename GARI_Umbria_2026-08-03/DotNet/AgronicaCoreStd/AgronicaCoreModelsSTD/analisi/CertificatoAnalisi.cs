using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.analisi
{
    public class CertificatoAnalisi : BaseCodeDescr
    {
        public string numero_certificato { get; set; }

        public CertificatoAnalisi(int codice) : base(codice, "") { }
        public CertificatoAnalisi() { }

    }
}
