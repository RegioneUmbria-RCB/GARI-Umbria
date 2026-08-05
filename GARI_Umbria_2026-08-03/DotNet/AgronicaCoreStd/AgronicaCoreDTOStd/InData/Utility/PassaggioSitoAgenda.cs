using System;
using System.Collections.Generic;
using System.Text;


namespace AgronicaCoreDTOStd.InData.Utility
{
    public class PassaggioSitoAgenda
    {
        public VariabiliInSessione_NG VariabiliInSessione { get; set; }
        public object InData { get; set; }
        public List<AgronicaCoreDTOStd.InData.Utility.ParametriAggiuntivi_QueryString> ParametriAggiuntivi { get; set; }
        public Boolean AggiungiSoloParametriAggiuntivi { get; set; }
        public int IDSezione { get; set; }
     
    }
}
