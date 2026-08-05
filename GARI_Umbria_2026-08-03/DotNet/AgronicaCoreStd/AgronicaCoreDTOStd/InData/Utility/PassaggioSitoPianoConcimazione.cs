using AgronicaCoreDTOStd.InData;
using AgronicaCoreDTOStd.InData.Anagrafica;
using AgronicaCoreDTOStd.InData.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Utility
{
   public class PassaggioSitoPianoConcimazione
    {
        public VariabiliInSessione_NG VariabiliInSessione { get; set; }
        public object InData { get; set; }
        public List<ParametriAggiuntivi_QueryString> ParametriAggiuntivi { get; set; }
        public Boolean AggiungiSoloParametriAggiuntivi { get; set; }
        public int IDSezione { get; set; }

    }
}
