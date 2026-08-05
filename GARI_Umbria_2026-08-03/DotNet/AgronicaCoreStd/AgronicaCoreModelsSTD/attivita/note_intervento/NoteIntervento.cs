using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.note_intervento
{
    public class NoteIntervento : BaseCodeDescr
    {

        //Note_Valore_Numerico, Note_Valore_Stringa, Visibile

        public decimal note_Valore_Numerico { get; set; }

        public string note_Valore_Stringa { get; set; }

        public int visibile { get; set; }

        public NoteInterventoGruppi noteInterventoGruppi { get; set; }

        public NoteIntervento(int codice, int gruppo) : base(codice, "") 
        {
            noteInterventoGruppi = new NoteInterventoGruppi(gruppo);
        }
        public NoteIntervento(int codice) : base(codice, "") { }
        public NoteIntervento(int codice, string descrizione) : base(codice, descrizione) { }
        public NoteIntervento() : base() { }
    }
}
