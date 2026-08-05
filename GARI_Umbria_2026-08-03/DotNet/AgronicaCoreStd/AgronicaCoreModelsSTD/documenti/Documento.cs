using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.documenti
{
    public class Documento
    {
        public int ID_Tipologia { get; set; }
        public DateTime Data_Scadenza { get; set; }
        public String Descrizione { get; set; }
        public String Note { get; set; }
        public EnteRilascio Ente_Rilascio { get; set; }
        public DateTime Data_Rilascio { get; set; }
        public string Numero { get; set; }
        public List<DocumentoAllegato> Allegati { get; set; }

    }


}
