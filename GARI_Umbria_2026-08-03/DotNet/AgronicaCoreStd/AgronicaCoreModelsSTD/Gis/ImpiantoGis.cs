using System;
using System.Collections.Generic;
using System.Text;
namespace AgronicaCoreModelsSTD.Gis
{
    /// <summary>
    /// Classe implementata per poter utilizzare AgronicaCoreGestioneRichieste.Impianto, 
    /// sucessivamente è necessario Serializzarla in stringa e deserializzarla nel tipo AgronicaCoreGestioneRichieste.Impianto
    /// </summary>
    public class ImpiantoGis
    {
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Appezza { get; set; }
        public int Id_Reg { get; set; }
        public int Campo_Cod { get; set; }

        public int Veg_Cod { get; set; }
        public int Cul_Cod { get; set; }
        public int Grfi_Cod { get; set; }

        public string Rag_Soc { get; set; }
        public string Sa_Nome { get; set; }
        public string App_Nome { get; set; }
        public string Veg_Des { get; set; }
        public string Cul_Des { get; set; }
        public string Sup_Imp { get; set; }
        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public DateTime Data_Raccolta { get; set; }
        public int Progetto_Cod { get; set; }
    }
}
