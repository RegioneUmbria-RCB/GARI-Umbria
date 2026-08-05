using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Agenda
{
    public class LeggiSpecieQdC
    {
        public Impresa impresa { get; set; }
        public CentroAziendale centroAziendale { get; set; }
        public DateTime data { get; set; }
        public bool consideraTerrenoNudo { get; set; }
        public bool soloAttiviAllaData { get; set;}
        public List<Lavorazione> lavorazioni { get; set; }
    }
}
