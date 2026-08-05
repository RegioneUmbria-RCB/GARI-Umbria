using System;
using System.Collections.Generic;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.metaschema;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class LeggiPUA
    {
        public Impresa impresa { get; set; }
        public DateTime data { get; set; }
        public List<Lavorazione> operazioni { get; set; }
        public Attivita.Tipo_Attivita tipo_Attivita { get; set; }
        public Attivita.Tipo_Ricetta tipo_Ricetta { get; set; }
        public Attivita.Stati stato { get; set; }
        public Disciplinare disciplinare { get; set; }
    }
}