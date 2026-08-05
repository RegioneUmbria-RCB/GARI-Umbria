using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Prodotti : APP_Agronica_Entity
    {
        public int Elem_Cod { get; set; }
        public string NomeComune { get; set; }
        public int Prodotto_Cod { get; set; }
        public string Prodotto_Des { get; set; }
        public decimal? Prodotto_Giacenza { get; set; }
        public decimal? N { get; set; }
        public decimal? P2O5 { get; set; }
        public decimal? K2O { get; set; }
        public decimal? Cu { get; set; }
        public int? Uso { get; set; }
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Udm_Cod { get; set; }

    }
}
