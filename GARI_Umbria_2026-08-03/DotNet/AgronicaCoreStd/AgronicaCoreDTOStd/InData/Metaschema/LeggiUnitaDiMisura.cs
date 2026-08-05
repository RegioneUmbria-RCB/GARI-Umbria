using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.metaschema.avversita;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiUnitaDiMisura
    {
        public Lavorazione lavorazione { get; set; }

        public int elem_cod { get; set; }

        public AvversitaGruppo avversita { get; set; }

        public Attivita.Tipo_Attivita tipo_Attivita { get; set; }

        public Attivita.Tipo_Ricetta tipo_Ricetta { get; set; }

        public DettaglioTrattamento dettaglioTrattamento { get; set; }

        public DettaglioFertilizzazione dettaglioFertilizzazione { get; set; }

        public DettaglioSemina dettaglioSemina { get; set; }

        public DoseEtichetta doseEtichetta { get; set; }

        public UnitaDiMisura unitaDiMisura { get; set; }
    }
}