using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class ElencoAllegatiLayer_Out
    { 
        public List<AllegatoLayer> elencoAllegatiLayer { get; set; }
    }
    public class AllegatoLayer
    {
        public string descrizione { get; set; }
        public DateTime data_estrazione { get; set; }
        public string formato { get; set; }
        public DateTime inizio_validita { get; set; }
        public DateTime fine_validita { get; set; }
        public int allegati_Documenti_Cod { get; set; }
        public string Utente_Esportazione { get; set; }
    }

    public class AllegatoLayerModifica
    {
        public string descrizione { get; set; }
        public DateTime inizio_validita { get; set; }
        public DateTime fine_validita { get; set; }
        public int allegati_Documenti_Cod { get; set; }
    }


    public class AllegatoFile 
    { 
        public int allegati_Documenti_Cod { get; set; }
        public string fileName { get; set; }
        public byte[] file_Allegato_DB { get; set; }
    }
}
