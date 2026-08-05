using System;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.anagrafiche;
using System.Collections.Generic;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaCoreModelsSTD.metaschema;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class LeggiInizializza_QdC
    {
        public List<Lavorazione> operazioni { get; set; }

        public int tipoOperazioneDB { get; set; }

        public DateTime data { get; set; }

        public  Impresa impresa { get; set; }

        public Attivita.Tipo_Attivita tipoAttivita { get; set; }

        public Attivita.Stati statoAttivita { get; set; }

        public Attivita.Tipo_Ricetta tipoRicetta { get; set; }

        public List<Attivita> lista_Attivita { get; set; }

        public List<CodiciXOperazione> codiciAttivita { get; set; }

        public Disciplinare disciplinare { get; set; }

    }
}