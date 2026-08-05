using AgronicaCoreModelsSTD.anagrafiche;
using OutData.Kendo;
using System;
using System.Collections.Generic;
using System.Data;

namespace AgronicaCoreDTOStd.InData.FiltroRicerca
{
    public class ProseguiSelezionati
    {
        public int TipoMostra { get; set; }
        public List<string> Chiavi { get; set; }

        public Object ParametriFiltro { get; set; }

        public string JsonRichiesta { get; set; }
        public VariabiliInSessione_NG VariabiliInSessione { get; set; }
    }
}
