using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.documenti
{
    public class DocumentoPerMacchina
    {
        public int ID_Tipologia { get; set; }
        public DateTime Data_Scadenza { get; set; }
        public String Descrizione { get; set; }
        public String Note { get; set; }
        public List<DocumentoAllegato> Allegati { get; set; }

        public DocumentoPerMacchina()
        {
            Allegati = new List<DocumentoAllegato>();
        }

    }


}
