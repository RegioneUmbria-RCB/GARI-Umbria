using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Ricette_Dettaglio_Tecnico : APP_Agronica_Entity
    {
        public int Ricetta_Cod { get; set; }
        public int Ricetta_Operazione_Cod { get; set; }
        public int Ricetta_Tecnico_Cod { get; set; }
        public int Ricetta_Dettaglio_Cod { get; set; }
        public decimal Qta_Ril { get; set; }
        public int Av_Cod { get; set; }
        public int Av_Gru { get; set; }
        public int FF_Classe { get; set; }
        public decimal N { get; set; }
        public decimal P { get; set; }
        public decimal K { get; set; }
        public decimal CU { get; set; }
        public int Dett_Cod { get; set; }
        public decimal Dose { get; set; }
        public int Parziale { get; set; }
        public int Nitrati { get; set; }
        public decimal Freatimetro { get; set; }
        public DateTime Inn1_data { get; set; }
        public DateTime Inn2_data { get; set; }
        public decimal Piezo1 { get; set; }
        public decimal Piezo2 { get; set; }
        public decimal Piezo3 { get; set; }
        public decimal Piezo4 { get; set; }
        // public decimal Efficienza { get; set; }
        // public int Ditta_Cod { get; set; }
    }
}
