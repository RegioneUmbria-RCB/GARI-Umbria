using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Ricette_Dettagli : APP_Agronica_Entity
    {
        public int Ricetta_Cod { get; set; }
        public int Ricetta_Operazione_Cod { get; set; }
        public int Ricetta_Dettaglio_Cod { get; set; }
        public int Cau_Mov { get; set; }
        public int Elem_Cod { get; set; }
        public int Pro_Cod { get; set; }
        public int Mat_Cod { get; set; }
        public string Lotto { get; set; }
        public int Udm_Cod { get; set; }
        public int Extra_Int { get; set; }
        public decimal Qta { get; set; }
        public decimal Qta_Extra { get; set; }
        public decimal Qta_Extra_Totale { get; set; }
        public int Mezzo_Det { get; set; }
        public int Udm_Cod_Extra { get; set; }

    }
}
