using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.metaschema.avversita;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class GiacenzeXProdotto
    { 

        public Lavorazione Operazione { get; set; }

        public int Categoria_Magazzino { get; set; }

        public DettaglioTrattamento dettaglioTrattamento { get; set; }

        public DettaglioFertilizzazione dettaglioFertilizzazione { get; set; }

        public DettaglioSemina dettaglioSemina { get; set; }

        public AvversitaGruppo avversitaGruppo { get; set; }
    }
}