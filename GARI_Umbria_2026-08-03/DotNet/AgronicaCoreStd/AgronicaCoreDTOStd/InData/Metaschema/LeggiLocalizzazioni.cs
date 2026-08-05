using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaCoreModelsSTD.metaschema.avversita;
using System;
using AgronicaCoreModelsSTD.attivita.dettagli;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiLocalizzazioni
    {
        public Lavorazione lavorazione { get; set; }

        public DettaglioTrattamento dettaglioTrattamento { get; set; }

        public Disciplinare disciplinare { get; set; }

        public AvversitaGruppo avversitaGruppo { get; set; }

        public Specie specie { get; set; }
    }
}