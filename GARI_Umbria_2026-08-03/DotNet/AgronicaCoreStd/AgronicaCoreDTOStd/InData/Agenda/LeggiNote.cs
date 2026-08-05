using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class LeggiNote
    {
        public Attivita.Tipo_Attivita tipoAttivita { get; set; }

        public Lavorazione[] lavorazioni { get; set; }

        public Specie specieVegetale { get; set; }

        public string piva { get; set; }

        public Tuple<string, string>[] parametriAggiuntivi { get; set; }

    }
}