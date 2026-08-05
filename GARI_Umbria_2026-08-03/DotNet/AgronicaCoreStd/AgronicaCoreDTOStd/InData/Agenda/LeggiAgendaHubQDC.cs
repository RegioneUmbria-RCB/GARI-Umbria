using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Agenda
{
    public class LeggiAgendaHubQDC
    {
        public List<int> id_agenda { get; set; }
        public string piva { get; set; }
        public int saCod { get; set;}
        public int specieVegetale { get; set; }
        public Disciplinare disciplinare { get; set; }
        public IntervalloTemporale intervallo { get; set; }
    }

    public class LeggiAgendaHubQDCNew
    {
        /// <summary>
        /// Optional header identifier. Used only for update functions.
        /// </summary>
        public int? idTestata { get; set; }
        public string piva { get; set; }
        public List<int> saCod { get; set; } = new List<int>();
        public List<int> specieVegetale { get; set; } = new List<int>();
        public List<int> operazioni { get; set; } = new List<int>();
        public Disciplinare disciplinare { get; set; }
        public IntervalloTemporale intervallo { get; set; }
        public bool verificaIAF { get; set; } = false;
        public bool verificaSoloControlliUtente { get; set; } = false;
        public bool verificaMagazzino { get; set; } = false;
        public bool verificaNormative { get; set; } = true;
        public Boolean controlloRiduzioneDiserbo { get; set; }
        public int origin { get; set; } = 1; //rif Enum_OrigineRichiestaVerificaConformita.verifica_massiva_engine

        public bool IsValid()
        {
            if (piva == null || piva == "") return false;
            if (saCod == null || saCod.Count == 0) return false;
            if (specieVegetale == null || specieVegetale.Count == 0) return false;
            if (operazioni == null || operazioni.Count == 0) return false;
            if (disciplinare == null) return false;
            if (intervallo == null) return false;
            return true;
        }

        public LeggiAgendaHubQDCNew Copy()
        {
            LeggiAgendaHubQDCNew copy = new LeggiAgendaHubQDCNew();
            copy.piva = piva;
            copy.saCod = new List<int>(saCod);
            copy.specieVegetale = new List<int>(specieVegetale);
            copy.operazioni = new List<int>(operazioni);
            copy.disciplinare = new Disciplinare(disciplinare.codice);
            copy.intervallo = new IntervalloTemporale(intervallo.inizio, intervallo.fine);
            copy.verificaIAF = verificaIAF;
            copy.verificaSoloControlliUtente = verificaSoloControlliUtente;
            copy.verificaMagazzino = verificaMagazzino;
            copy.verificaNormative = verificaNormative;
            copy.controlloRiduzioneDiserbo = controlloRiduzioneDiserbo;
            copy.origin = origin;
            return copy;
        }
    }
}
