using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiIVA_Aliquote
    {
        public int Codice { get; set; }
        public decimal Aliquota { get; set; }
        public int Tipologia { get; set; }
        public int Flag_Credito_Imposta_Export { get; set; }
        public string NaturaEsclusione { get; set; }
    }
}
