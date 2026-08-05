using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class ObjParams_Analisi2010
    {
        public int Tipo_Analisi { get; set; }
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Analisi_Testata_Cod { get; set; }
        public int ID_PDC_Campione { get; set; }
        public int ID_PDC_Dettagli { get; set; }
        public int ID_PDC_Testata { get; set; }
        public int Pagina_Richiesta { get; set; } //TipiEnumerativi.enum_PagineAnalisi_2010
        public int Tipo_Operazione { get; set; } //Inserimento 1 - Visualizzazione 2
        public int SitoOrigine { get; set; }
        public int Pagina_SitoOrigine { get; set; }
        public int Veg_Cod { get; set; }
    }
}
