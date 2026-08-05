using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.metaschema;
using System;

namespace InData.Agenda
{
    public class LeggiDisponibilitaAttualeFertilizzante
    {
        public DettaglioFertilizzazione dettaglioFertilizzazione { get; set; }

        public Pua pua { get; set; }

        public DateTime data { get; set; }
    }
}
