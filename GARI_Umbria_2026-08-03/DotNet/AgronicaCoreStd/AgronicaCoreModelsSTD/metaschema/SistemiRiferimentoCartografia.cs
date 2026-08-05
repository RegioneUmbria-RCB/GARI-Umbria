using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{

    public class SistemiRiferimentoCartografia : BaseCodeDescr
    {

        public string CSFrom { get; set; }
        public string CSTo { get; set; }
        public string CStoGEO { get; set; }
        public string Note { get; set; }
        public double AgronicaLatOffSet { get; set; }
        public double AgronicaLonOffSet { get; set; }
        public string LibreriaDaUsare { get; set; }

        public SistemiRiferimentoCartografia(int codice) : base(codice,"")
        {
        }

        public SistemiRiferimentoCartografia() : base()
        {

        }
    }
}
