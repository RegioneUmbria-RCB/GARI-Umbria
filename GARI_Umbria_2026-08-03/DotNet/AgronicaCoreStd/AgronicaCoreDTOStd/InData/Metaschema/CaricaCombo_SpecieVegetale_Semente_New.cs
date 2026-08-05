using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class CaricaCombo_SpecieVegetale_Semente_New
    {  
        public int Sem_Cod { get; set; }
        public int Veg_Cod { get; set; } 
        public Boolean Flag_PrimaRiga { get; set; } 
        public string Testo_PrimaRiga { get; set; }
        public string Cod_PrimaRiga { get; set; } 
        public string FinestraTemp_Inizio { get; set; }
        public string FinestraTemp_Fine { get; set; } 
        public string FiltroAggiuntivo { get; set; }
        public string Ordinamento { get; set; }
        public Boolean Flag_FiltroUtente { get; set; } 
        public int Veg_Cod_daModificare { get; set; } 
        public int SemCod_Rif_VegCod_daModificare { get; set; } 

    }
}
