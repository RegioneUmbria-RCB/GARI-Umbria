using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita.dettagli;
using System;

namespace InData.Anagrafica
{
    public class LeggiUltimo_Magazzino_Prodotto_Movimentato
    {

        public Impresa impresa { get; set; }

        public DettaglioTrattamento dettaglioTrattamento { get; set; }

        public DettaglioFertilizzazione dettaglioFertilizzazione { get; set; }

        public DettaglioSemina dettaglioSemina { get; set; }

        public DateTime data { get; set; }

    }
}
