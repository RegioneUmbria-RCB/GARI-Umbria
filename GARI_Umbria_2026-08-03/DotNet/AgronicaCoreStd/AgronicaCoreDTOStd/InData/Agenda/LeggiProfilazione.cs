using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class LeggiProfilazione
    {
        public Impresa impresa { get; set; }

        public List<Lavorazione> operazioni { get; set; }

        public Specie specie { get; set; }

        public DateTime data { get; set; }

        public List<ParcoMacchine> macchine { get; set; }

        public List<Contatto> contatti { get; set; }

    }
}
