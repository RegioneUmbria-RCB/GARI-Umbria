using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaCoreModelsSTD.metaschema.avversita;
using System;
using AgronicaCoreModelsSTD.attivita.dettagli;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class LeggiDoseConsentitaDiserbo
    {
        public Attivita attivita { get; set; }

        public DettaglioTrattamento dettaglioTrattamento { get; set; }

        public Fabbricato fabbricato { get; set; }

        public string lotto { get; set; }

        public AvversitaGruppo avversitaGruppo { get; set; }

    }
}