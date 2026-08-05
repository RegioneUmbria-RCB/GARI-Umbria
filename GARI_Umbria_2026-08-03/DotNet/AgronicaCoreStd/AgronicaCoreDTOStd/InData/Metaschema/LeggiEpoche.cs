using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.avversita;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiEpoche
    {
        public Lavorazione lavorazione { get; set; }
        public Specie specie { get; set; }
        public Disciplinare disciplinare { get; set; }
    }
}