using AgronicaCoreDTOStd.InData.Anagrafica;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Utility
{
    public class GestioneStampe
    {
        public VariabiliInSessione_NG VariabiliInSessione { get; set; }
        public Parametri_ObjParametriAgenda_NG_GestioneRichieste InData { get; set; }
        public List<ParametriAggiuntivi_QueryString> ParametriAggiuntivi { get; set; }
        public int report { get; set; }
        
    }
}
