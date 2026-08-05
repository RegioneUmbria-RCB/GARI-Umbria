using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.metaschema.avversita;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiDosiEtichetta
    {
        public Specie specie { get; set; }
        public DettaglioTrattamento dettaglioTrattamento { get; set; }
        public AvversitaGruppo avversitaGruppo { get; set; }
        public Impianto[] impianti { get; set; }
        public MovimentoDiMagazzino[] prodottiDaTrattare { get; set; }
        public DateTime data { get; set; }
    }
}