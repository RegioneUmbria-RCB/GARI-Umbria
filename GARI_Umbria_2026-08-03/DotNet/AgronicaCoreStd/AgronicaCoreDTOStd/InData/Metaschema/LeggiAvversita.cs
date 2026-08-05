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
    public class LeggiAvversita
    {
        public Attivita.Tipo_Attivita tipoAttivita { get; set; }
        public Attivita.Stati statoAttivita { get; set; }
        public Lavorazione lavorazione { get; set; }

        public Impianto[] impianti { get; set; }

        public Specie specie { get; set; }

        public Disciplinare disciplinare { get; set; }
        
        public Epoca epocaDPI { get; set; }

        public DettaglioTrattamento dettaglioTrattamento { get; set; }

        public DateTime data { get; set; }
        
        public AvversitaGruppo avversitaGruppo { get; set; }

        public Soglia soglia { get; set; }

        public Appezzamento[] appezzamenti { get; set; }

        public bool visualizzaMovimentiMagazzino { get; set; }

        public bool escludiGiacenzeZero { get; set; }

        public bool magazziniAgenzie { get; set; }
    }
}