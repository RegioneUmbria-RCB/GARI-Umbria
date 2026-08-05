using System;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class LeggiDefault_DPI_QdC
    {
        public List<Lavorazione> operazioni { get; set; }

        public List<Impianto> impianti { get; set; }

        public List<Disciplinare> disciplinari { get; set; }

        public int tipoOperazioneDB { get; set; }

        public DateTime data { get; set; }

        public  Impresa impresa { get; set; }

        public  Attivita.Tipo_Ricetta tipoRicetta { get; set; }

        public Attivita.Tipo_Attivita tipoAttivita { get; set; }


        public List<CodiciXOperazione> codiciAttivita { get; set; }

    }
}