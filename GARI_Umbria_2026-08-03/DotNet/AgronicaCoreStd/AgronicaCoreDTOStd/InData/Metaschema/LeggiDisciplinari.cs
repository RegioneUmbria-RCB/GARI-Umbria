using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiDisciplinari
    {
        public Lavorazione[] lavorazioni { get; set; }

        public Specie specie { get; set; }

        public DateTime data { get; set; }

        public bool privato { get; set; }

        public Regolamenti regolamento { get; set; }

        public bool leggiPianoNutrizionale { get; set; }

        public IntervalloTemporale validita { get; set; }

        public bool IncludiNessunDisciplinare { get; set; } = false;
        public bool IncludiBiologico { get; set; } = false;
    }
}
