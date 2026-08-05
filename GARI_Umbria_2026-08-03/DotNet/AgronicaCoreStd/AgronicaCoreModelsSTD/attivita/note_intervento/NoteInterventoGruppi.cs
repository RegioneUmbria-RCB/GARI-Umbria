using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.note_intervento
{
    public class NoteInterventoGruppi : BaseCodeDescr
    {
        /// <summary>
        /// Indica le si stratta di note a aseleziona multipla o singola
        /// </summary>
        public Tipo_Gruppo_Note tipo { get; set; }

        public BaseCodeDescr[] presets { get; set; }

        public NoteInterventoGruppi(int codice) :base(codice, "")
        {
        }
        public NoteInterventoGruppi() :base() 
        {
        }

        public enum Tipo_Gruppo_Note
        {
            Multi = 0,
            Singolo = 1
        }

    }
}
