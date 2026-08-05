using AgronicaCoreDTOStd.InData.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Demetra
{
    public class Squadra
    {
        public string descrizione { get; set; }
        public List<ElementiSquadra> capisquadra { get; set; }
        public List<ElementiSquadra> membri { get; set; }

        public Validita validita;

        public bool flag_cancellazione;
    }

    public class ElementiSquadra
    {
        public string codice { get; set; }
        public string codice_esterno { get; set; }
    }
}
