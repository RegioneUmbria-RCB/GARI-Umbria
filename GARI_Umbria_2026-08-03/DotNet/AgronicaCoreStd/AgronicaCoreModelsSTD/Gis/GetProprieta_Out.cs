using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class GetProprieta_Out
    {
        public int Tipo_EntitaCod { get; set; }     //TipiEnumerativi.enum_GIS2012_TipoEntita

        public string Piva { get; set; }
        public string Cul_Cod { get; set; }
        public string Grfi_Cod { get; set; }
        public string Grva_Cod { get; set; }

        public string Superficie { get; set; }
        public string Indirizzo { get; set; }
        public string Validita_Inizio { get; set; }
        public string Validita_Fine { get; set; }
        public string Flag_GPS { get; set; }
        public string Progetto_Nome { get; set; }
        public string Grfi_Des { get; set; }
        public string Grva_Des { get; set; }
        public string Cul_Des { get; set; }
        public string Veg_Des { get; set; }
        public string Rag_Soc { get; set; }
        public string Sa_Nome { get; set; }
        public string App_Nome { get; set; }
        public string Codice_Fiscale_Tecnico { get; set; }
        public string Destinazione_Uso { get; set; }
        public string Appezza_Validita_Inizio { get; set; }
        public string Appezza_Validita_Fine { get; set; }

        public string Extra_Info { get; set; }  //in questo caso contiene una tabella HTML
        public GiasPalm GiasPalm { get; set; }

    }
}
