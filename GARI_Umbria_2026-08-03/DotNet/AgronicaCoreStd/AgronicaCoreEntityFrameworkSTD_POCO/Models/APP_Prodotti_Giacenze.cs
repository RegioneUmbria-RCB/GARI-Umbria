using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Prodotti_Giacenze : APP_Agronica_Entity
    {

        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Tipo_Destinazione { get; set; }
        public int Fabbricato_Cod { get; set; }
        public string Fabbricato_Des { get; set; }
        public int Elem_Cod { get; set; }
        public int Prodotto_Cod { get; set; }
        public string Lotto { get; set; }
        public int Cal_Cod { get; set; }
        public int Cod_Progetto { get; set; }
        public decimal Giacenza { get; set; }
        public int Udm_Cod { get; set; }
        public int [] LavCodCompatibiliFormulati { get; set; }

    }
}
